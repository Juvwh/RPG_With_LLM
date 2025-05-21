using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using static UI_Manager;
using static Jobs;
using static AgentManager;
using static Coffre;
using System.Buffers;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using Unity.VisualScripting.FullSerializer;
using TMPro;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Windows;
using System.Collections;
using System.Collections.Generic;
using static Models;
using System.IO;
using System.Text;

public class Free : Evenement
{
    public GameManager _gm;

    /// <summary>
    /// Initializes the component by setting up the GameManager reference.
    /// </summary>
    private void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        _gm._free = this;
    }

    /// <summary>
    /// Generates the next part of the story based on the given prompt and group index.
    /// </summary>
    /// <param name="prompt">The prompt to generate the story.</param>
    /// <param name="groupIndex">The index of the group for which to generate the story.</param>
    public IEnumerator Generate(string prompt, int groupIndex)
    {
        bool done = false;
        string txt_Generated = "";
        string txt_Free = "";
        string txt_Resume = "";
        string txt_Case = "";
        string groupID = _gm._groupsManager.m_Groups[groupIndex]._groupID;
        PlayerData player = _gm._groupsManager.m_Groups[groupIndex].m_Players[0]; // Choose the first player of the group by default

        while (!done)
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Salle1_Generer_Free, prompt, (response) =>
            {
                txt_Generated = response;
            }));

            done = Parser.FREE(txt_Generated, out txt_Free, out txt_Resume, out txt_Case);
            if (!done)
            {
                yield return new WaitForSeconds(1); // Wait 1 second before retrying
            }
        }

        // Check if the group has not changed in the meantime
        if (CheckSameGroup(_gm, groupIndex, groupID))
        {
            // Has the group changed index?
            int new_groupIndex = CheckChangeIndex(_gm, groupIndex, groupID);
            if (new_groupIndex != -1)
            {
                groupIndex = new_groupIndex;
            }
            else
            {
                // Do not add the story and restart generation with the character(s) who joined the group
                int indexGroup = _gm._groupsManager.GetGroupIndexByPlayer(player);
                prompt += _gm._language.GetText("__coherence_input_becareful") +
                            _gm._groupsManager.m_Groups[indexGroup].GetAllHistoryResume().Last().GetResume();
                StartCoroutine(Generate(prompt, groupIndex));
                yield break;
            }
        }

        // If not, continue normally
        _gm._groupsManager.AddHistory(groupIndex, TextType.Histoire, txt_Free);
        _gm._groupsManager.m_Groups[groupIndex].AddHistoryResume(txt_Resume);
        _gm._groupsManager.m_Groups[groupIndex].SetCoordDescription(txt_Case);

        // Reauthorize the group to move
        // Display their new vision on the map
        _gm._groupsManager.m_Groups[groupIndex].SetReady(true);
        _gm._groupsManager.m_Groups[groupIndex].SetMoved(false);
        foreach (PlayerData playerData in _gm._groupsManager.m_Groups[groupIndex].m_Players)
        {
            _gm._mapManager.visitFloorTile(playerData.playerPosition, playerData.playerVision);
            _gm._mapManager.rebuildVector(new Vector2Int(0, 0));
        }

        if (_gm._UI_Manager.m_IndexCurrentGroupOnDisplay == groupIndex)
        {
            _gm._UI_Manager.SwitchGroupCanvas(groupIndex);
        }
    }

    /// <summary>
    /// Validates the player's input using LLM and processes the result.
    /// </summary>
    public IEnumerator LLM_Validation_Input_Player()
    {
        UI_Manager ui = _gm._UI_Manager;
        int currentGroupIndex = _gm._UI_Manager.m_IndexCurrentGroupOnDisplay;
        ui.m_ChargementInput.SetActive(true);
        _gm._groupsManager.SetIsCheckingCoherence(currentGroupIndex, true);
        ui.Afficher_InputNonCoherent(false);
        ui.Afficher_BoutonEnvoie(false);
        ui.Afficher_InputManager(false);
        GroupsManager _groupmanager = _gm._groupsManager;

        string input = _gm._UI_Manager.m_InputText.text;

        var values = new Dictionary<string, string>()
        {
            {"theme", _gm._memoryManager.GetTheme()},
            {"synopsys", _gm._memoryManager._synopsisResumed},
            {"groupe_names", _groupmanager.m_Groups[currentGroupIndex].GetResume_Names()},
            {"ancienne_position", _groupmanager.m_Groups[currentGroupIndex].GetResume_LastPosition()},
            {"vision", _groupmanager.m_Groups[currentGroupIndex].GetResume_InVision(true)},
            {"historique", _groupmanager.m_Groups[currentGroupIndex]._lastPhaseResume},
            {"other", _gm._historyManager.GetOtherGroupResume(currentGroupIndex)},
            {"focus", $"Focus du prompt sur le groupe 1 ({_groupmanager.m_Groups[currentGroupIndex].GetAllNames()})"},
            {"choix", input}
        };

        string prompt = _gm._language.GetPrompt("__coherence_input_create_prompt__", values);
        prompt += input.Trim().Length < 4 ? _gm._language.GetText("__coherence_input_create_prompt__ia_choose") : "\",\n";

        // Check if the 3 conditions to open the door are met: 1) a door is 1 tile away. 2) the group has a key. 3) there is only one group.
        bool _doorIsNearby = false;
        bool _lootIsNearby = false;
        bool _haveKey = false;

        // Condition 1
        _gm._groupsManager.m_Groups[currentGroupIndex].CheckIfDoorIsNearby(out _doorIsNearby, out _haveKey);
        _gm._groupsManager.m_Groups[currentGroupIndex].CheckIfChestIsNearby(out _lootIsNearby);

        if (_lootIsNearby)
        {
            prompt += _gm._language.GetText("__coherence_input_add_info_coffre_1");
        }
        else
        {
            prompt += _gm._language.GetText("__coherence_input_add_info_coffre_2");
        }

        if (_doorIsNearby)
        {
            if (_haveKey)
            {
                if (_gm._groupsManager.m_Groups.Length == 1)
                {
                    prompt += _gm._language.GetText("__coherence_input_add_info_A");
                }
                else
                {
                    bool aGroupIsNotDead = false;
                    foreach (Group group in _gm._groupsManager.m_Groups)
                    {
                        if (group._groupID != _gm._groupsManager.m_Groups[currentGroupIndex]._groupID)
                        {
                            if (!group.isDead)
                            {
                                aGroupIsNotDead = true;
                                break;
                            }
                        }
                    }
                    if (aGroupIsNotDead)
                    {
                        prompt += _gm._language.GetText("__coherence_input_add_info_B");
                    }
                    else
                    {
                        prompt += _gm._language.GetText("__coherence_input_add_info_C");
                    }
                }
            }
            else
            {
                prompt += _gm._language.GetText("__coherence_input_add_info_D");
            }
        }
        else
        {
            prompt += _gm._language.GetText("__coherence_input_add_info_E");
        }

        Debug.Log(prompt);
        string statut = "[ERROR]";
        while (statut == "[ERROR]")
        {
            string txt_generated = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.CheckCoherenceInput, prompt, (response) => { txt_generated = response; }));
            statut = Parsing_LLM_Validation_Input_Player(_gm, txt_generated, currentGroupIndex);
            if (statut == "[ERROR]")
            {
                yield return new WaitForSeconds(1);
            }
        }

        if (statut == "True")
        {
            _gm._groupsManager.SetIsCheckingCoherence(currentGroupIndex, false);
            Continue(currentGroupIndex);
            yield break;
        }
        else if (statut == "Coffre") // If combat, no chest opening
        {
            StartCoroutine(_gm._coffre.Open_Chest(currentGroupIndex));
            yield break;
        }
        else if (statut == "Porte")
        {
            StartCoroutine(_gm._historyManager.GoToNextRoom());
            yield break;
        }
        else
        {
            Show_UI(currentGroupIndex);
            yield break;
        }
    }

    /// <summary>
    /// Parses the LLM validation input for the player.
    /// </summary>
    /// <param name="_gameManager">The game manager instance.</param>
    /// <param name="_input">The input text to parse.</param>
    /// <param name="currentGroupIndex">The index of the current group.</param>
    /// <returns>A string indicating the parsing result.</returns>
    public string Parsing_LLM_Validation_Input_Player(GameManager _gameManager, string _input, int currentGroupIndex)
    {
        bool _coherent = false;
        bool _coffre = false;
        bool _porte = false;
        string _summary = string.Empty;
        string _justification = string.Empty;
        _input = Parser.CLEAN_JSON(_input);

        try
        {
            // Parse the JSON
            JObject data = JObject.Parse(_input);

            // Check if the expected fields are present
            if (data["logique"] == null || data["coffre"] == null || data["porte"] == null || data["summary"] == null || data["justification"] == null)
            {
                return "[ERROR]";
            }

            _coherent = data["logique"].ToObject<bool>();

            if (!_coherent)
            {
                _justification = data["justification"].ToString();
                _gameManager._UI_Manager.SetIncoherenceInfo(_justification);
                return "False";
            }

            _summary = data["summary"].ToString();
            _gameManager._groupsManager.AddHistory(currentGroupIndex, TextType.Event, _summary);
            _gameManager._groupsManager.m_Groups[currentGroupIndex].AddHistoryResume(_summary);

            _coffre = data["coffre"].ToObject<bool>();
            _porte = data["porte"].ToObject<bool>();

            if (_coffre)
            {
                return "Coffre";
            }

            if (_porte)
            {
                return "Porte";
            }

            return "True"; // Parsing successful
        }
        catch (Exception e)
        {
            return "[ERROR]";
        }
    }

    /// <summary>
    /// Continues the game after a successful input validation.
    /// </summary>
    /// <param name="groupIndex">The index of the group to continue with.</param>
    public void Continue(int groupIndex)
    {
        _gm._UI_Manager.m_InputText.text = "";
        _gm._UI_Manager.m_Input_Object_Glob_.text = "";
        _gm._UI_Manager.Afficher_InputNonCoherent(false);
        _gm._UI_Manager.SetCheckCoherenceInput(false);
        _gm._groupsManager.m_Groups[groupIndex].SetReady(false);
        _gm._UI_Manager.SwitchGroupCanvas(groupIndex);
        StartCoroutine(_gm._historyManager.Generate_Next_Sequence(groupIndex));
    }

    /// <summary>
    /// Displays the UI for the player to input their next action.
    /// </summary>
    /// <param name="currentGroupIndex">The index of the current group.</param>
    public void Show_UI(int currentGroupIndex)
    {
        UI_Manager ui = _gm._UI_Manager;
        ui.Afficher_InputNonCoherent(true);
        ui.Afficher_BoutonEnvoie(true);
        ui.Afficher_InputManager(true);
        ui.SetCheckCoherenceInput(false);

        ui.m_ChargementInput.SetActive(false);
        ui.SwitchGroupCanvas(currentGroupIndex);
        ui.Hide_Loading_Screen();
        ui.JustifierLeCodeVersLeBas();
    }
}
