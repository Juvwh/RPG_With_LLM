//Code by Justin Vanwichelen and Gaetan Berlaimont
//2025
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using static Jobs;
using static AgentManager;
using static Tags;
using System.Collections.Generic;

// This script manages the different steps of the plot.
// It handles the generation of text for different rooms, images, and voices.
// It manages the different steps of the game.
// When a button is clicked, it manages what happens next.
public class HistoryManager : MonoBehaviour
{
    [Header("Managers")]
    private GameManager _gm;
    private UI_Manager _UI_Manager;
    private MemoryManager _memoryManager;
    private ImagesManager _imagesManager;
    private CombatManager _combatManager;
    private CharactersManagers _charactersManager;
    private GroupsManager _groupsManager;
    private LocalizationManager _language;
    public MesureInfluences _mesure;

    [Header("Info current state")]
    public string _synopsis = "";
    public int _currentRoom = 0;
    public int _currentCharacterIndex = -1;
    public string _currentRoomDescription = "";

    [Header("Others")]
    private bool _isRunning = false;

    /// <summary>
    /// Initializes the HistoryManager and starts the campaign generation.
    /// </summary>
    void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        _UI_Manager = _gm._UI_Manager;
        _memoryManager = _gm._memoryManager;
        _imagesManager = _gm._imagesManager;
        _combatManager = _gm._combatManager;
        _charactersManager = _gm._charactersManagers;
        _groupsManager = _gm._groupsManager;
        _currentCharacterIndex = -1;
        _language = _gm._language;
        _isRunning = true;
        StartCoroutine(Generate_Campagn()); // Start the campaign generation
    }

    /// <summary>
    /// Handles cleanup when the application quits.
    /// </summary>
    void OnApplicationQuit()
    {
        _isRunning = false;
    }

    /// <summary>
    /// Generates the entire campaign, including text for each room, synopsis, and player presentation.
    /// </summary>
    public IEnumerator Generate_Campagn()
    {
        // UI
        _memoryManager.SetTheme(_charactersManager.m_CurrentTheme);
        _UI_Manager.Show_Loading_Screen();       // Start by displaying the loading screen

        // Wait for groups to be formed and exist
        while (_gm._groupsManager.m_Groups.Length == 0)
        {
            // Wait for the group to be loaded.
            yield return new WaitForSeconds(1); // Wait 1 second
        }

        // If the language is not French, we need to translate the different characters present:
        if (_gm._language.currentLanguage != "fr")
        {
            // Translate
            PlayerData[] players = _gm._charactersManagers.GetAllPlayers();
            string prompt = $"{{\n";
            int i = 0;
            foreach (PlayerData player in players)
            {
                // We will translate the name and description of the player
                prompt += $"\"personnage\" : [\n";
                prompt += $"{{\"description\" : \"{player.playerResume}\",\n" +
                            $"\"race\":\"{player.playerRace}\",\n" +
                            $"\"classe\":\"{player.playerClass}\",\n" +
                            $"\"arme\":\"{player.playerArme}\"\n}}";
                i++;
                if (i < players.Length)
                {
                    prompt += $",\n";
                }
            }
            string langue = "français";
            switch (_gm._language.currentLanguage)
            {
                case "en":
                    langue = "anglais";
                    break;
                default:
                    break;
            }

            prompt += $"\n]\n\"langue cible\":\"{langue}\"\n}}";

            string text_Genere = "";
            bool done = false;
            while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
            {
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Traducteur, prompt, (response) =>
                {
                    text_Genere = response;
                }));
                done = Parser.TRANSLATE(text_Genere, _gm);
            }
        }

        // We will launch the first generation
        yield return StartCoroutine(_memoryManager.Generate_Intrigue());   // Call the SendPrompt method of the selected LLM

        string _synopsis = _memoryManager.GetMemory(MemoryManager.memoryType.Synopsis);

        // Reading the synopsis audio
        //await GenererAudio(_synopsis);

        // We remove the loading screen and display the different buttons when everything has been generated.
        _UI_Manager.Hide_Loading_Screen();
        _UI_Manager.Afficher_Bouton_Commencer(true);
        _UI_Manager.Afficher_Bouton_Recommencer(true);
        _UI_Manager.Afficher_Texte_Contexte(true);
        _UI_Manager.Changer_Texte_Contexte(_synopsis);

        //StartCoroutine(autoregenerate());

        yield break; // End of the coroutine.
    }


    /// <summary>
    /// Regenerate the plot.
    /// </summary>
    public IEnumerator RegenerateCampagn()
    {
        // UI
        _UI_Manager.Show_Loading_Screen();
        // We will launch the first generation
        yield return StartCoroutine(_memoryManager.Generate_Intrigue_mesure());   // Call the SendPrompt method of the selected LLM

        string _synopsis = _memoryManager.GetMemory(MemoryManager.memoryType.Synopsis);
        // We remove the loading screen and display the different buttons when everything has been generated.
        _UI_Manager.Hide_Loading_Screen();
        _UI_Manager.Afficher_Bouton_Commencer(true);
        _UI_Manager.Afficher_Bouton_Recommencer(true);
        _UI_Manager.Afficher_Texte_Contexte(true);
        _UI_Manager.Changer_Texte_Contexte(_synopsis);

        yield break; // End of the coroutine.
    }

    /// <summary>
    /// Continue the story.
    /// </summary>
    public IEnumerator StartGame()
    {
        _UI_Manager.Show_Loading_Screen();
        string _synopsis = _memoryManager.GetMemory(MemoryManager.memoryType.Synopsis);
        // We generate the first phase:
        string resumeDeLaSalle = _memoryManager.GetMemory(MemoryManager.memoryType.Room1);
        var values = new Dictionary<string, string>
        {
            { "theme", _memoryManager.GetTheme() },
            { "synopsis", _synopsis },
            { "characters", _gm._charactersManagers.GetAllCharactersNameAndDescriptions() },
            { "environment", resumeDeLaSalle }
        };

        string prompt = _gm._language.GetPrompt("__createPrompt__room_1_Starter__", values);

        string text_Genere = "";
        bool done = false;
        string caseText = "";
        while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
        {
            text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Starter_Room1, prompt, (response) =>
            {
                text_Genere = response;
            }));

            done = Parser.STARTER(text_Genere, _gm, out caseText);
        }

        _gm._groupsManager.SetReady(0, true);
        yield return StartCoroutine(Wait_Group_Is_Ready(0));
        _gm._groupsManager.SetCoordDescription(0, caseText);

        _gm._groupsManager.UpdateAllCanvasInventory();
        if ((!_groupsManager.IsReady(0) && _isRunning) || (!_gm._mapManager.mapGenerated && _isRunning))
        {
            yield return StartCoroutine(Wait_Group_Is_Ready(0));
        }
        _UI_Manager.SwitchGroupCanvas(0);
        _UI_Manager.Hide_Loading_Screen();
        // By checking if the characters have moved, we fix the bug of the map not displaying directly
        _gm._groupsManager.PlayerHasMoved(0);

        _UI_Manager.JustifierLeCodeVersLeBas();
        StartCoroutine(waitBeforeJustification());
        _UI_Manager.m_Canvas_Tutoriel.SetActive(true);
    }

    /// <summary>
    /// Waits briefly before justifying the text.
    /// </summary>
    IEnumerator waitBeforeJustification()
    {
        yield return new WaitForSeconds(.1f);
        _UI_Manager.JustifierLeCodeVersLeBas();
    }

    /// <summary>
    /// Generates the next sequence for a group.
    /// </summary>
    public IEnumerator Generate_Next_Sequence(int groupIndex)
    {
        string roomAmbiance = _memoryManager.GetMemoryRoom(_currentRoom);
        string memoryGroup = Parser.CLEAN(_groupsManager.m_Groups[groupIndex].GetAllHistoryResumeInString());

        string resumeGroup = _groupsManager.m_Groups[groupIndex].GetResume();

        Group group = _groupsManager.m_Groups[groupIndex];

        var values = new Dictionary<string, string>
        {
            { "theme", _memoryManager.GetTheme() },
            { "synopsis", _memoryManager._synopsisResumed },
            { "environment", roomAmbiance },
            { "group_names", group.GetResume_Names() },
            { "last_position", group.GetResume_LastPosition() },
            { "vision", group.GetResume_InVision() },
            { "history", group._penultimatePenultimatePhaseResume + " ; " + group._penultimatePhaseResume },
            { "other_groups", GetOtherGroupResume(groupIndex) },
            { "focus", group.GetAllNames() },
            { "action", group._lastPhaseResume }
        };

        string prompt_Free = _gm._language.GetPrompt("__createPrompt__Generate_Prompt_Free", values);

        // Check if an enemy has spawned on the map to test whether to start combat or not
        if (Check_Enemy_Around(groupIndex))
        {
            yield break;
        }

        StartCoroutine(_gm._free.Generate(prompt_Free, groupIndex));
    }

    /// <summary>
    /// Checks if combat should start.
    /// </summary>
    public bool Check_Enemy_Around(int groupIndex)
    {
        if (!_combatManager._isCombat)
        {
            if (_gm._groupsManager.m_Groups[groupIndex].IsEnemyNearbyTheGroup())
            {
                // An enemy is on the map -> test if it notices us
                bool HeroStart = new System.Random().Next(1, 11) <= 3;
                StartCoroutine(_combatManager.StartCombat(groupIndex, HeroStart));
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Waits until the group is ready.
    /// </summary>
    private IEnumerator Wait_Group_Is_Ready(int i)
    {
        while ((!_groupsManager.IsReady(i) && _isRunning) || (!_gm._mapManager.mapGenerated && _isRunning))
        {
            yield return new WaitForSeconds(1); // Wait 1 second
        }
    }

    /// <summary>
    /// Gets the current room index.
    /// </summary>
    public int GetCurrentRoom()
    {
        return _currentRoom;
    }

    /// <summary>
    /// Generates audio for the given text.
    /// </summary>
    private async Task GenererAudio(string _synopsis)
    {
        if (_gm.generateVoice)
        {
            AudioClip voiceSynopsis = await _gm.Generate_voice(_gm.llm_Voice, _synopsis);
            _gm.SetCurrentVoice(voiceSynopsis);
            _gm.PlayAudio(voiceSynopsis);
        } // We generate a voice that reads the synopsis if the option is enabled
    }

    /// <summary>
    /// Gets the current character index.
    /// </summary>
    public int GetCurrentCharacterIndex()
    {
        return _currentCharacterIndex;
    }

    /// <summary>
    /// Moves to the next room.
    /// </summary>
    public IEnumerator GoToNextRoom()
    {
        _UI_Manager.Show_Loading_Screen();       // Start by displaying the loading screen

        // We start by generating a large summary of the history.
        string resumePreviousRoom = "Erreur";
        while (resumePreviousRoom.Contains("Erreur") || resumePreviousRoom.Contains("Error")) // While the text has not been generated (for example, there was an error), we resend the prompt
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.GenererResumeRoom, _groupsManager.m_Groups[0].GetAllHistoryInString(), (reponsellm) => { resumePreviousRoom = reponsellm; }));
            yield return new WaitForSeconds(1); // Wait 1 second
        }
        _currentRoom++;

        if (_currentRoom == 3)
        {
            StartCoroutine(_gm._historyManager.GenerateGoodEnd(resumePreviousRoom));
            yield break;
        }

        foreach (PlayerData p in _gm._charactersManagers.GetAllPlayers())
        {
            p.playerInventory_Keys = 0;
            p.playerInventory_Compass = 0;
        }

        if (_currentRoom == 2)
        {
            foreach (PlayerData p in _gm._charactersManagers.GetAllPlayers())
            {
                p.playerInventory_Keys = 0;
                p.playerInventory_Compass = 0;
                p.playerVision = 2;
            }
        }

        _gm._mapManager.mapGenerated = false;

        yield return StartCoroutine(_gm._mapManager.GenerateMap(_gm._memoryManager.GetMemoryRoom(_currentRoom), _currentRoom));

        _gm._groupsManager.m_Groups[0].SetReady(true);
        _gm._groupsManager.m_Groups[0].SetMoved(false);

        var values = new Dictionary<string, string>
        {
            { "theme", _memoryManager.GetTheme() },
            { "synopsis", _memoryManager._synopsisResumed },
            { "characters", _gm._charactersManagers.GetAllCharactersNameAndDescriptions() },
            { "resume", resumePreviousRoom },
            { "count", _currentRoom.ToString() },
            { "resumeNewRoom", _gm._memoryManager.GetMemoryRoom(_currentRoom) }
        };

        string prompt = _gm._language.GetPrompt("__prompt__next_room__", values);

        string text_Genere = "";
        bool done = false;
        string caseText = "";
        Job _salle;

        // Form a single group
        _groupsManager.Move();

        switch (_currentRoom)
        {
            case 1: _salle = Job.Starter_Room2; break;
            case 2: _salle = Job.Starter_Room3; break;
            case 3: _salle = Job.Starter_Room4; break;
            case 4: _salle = Job.Starter_Room5; break;
            default: _salle = Job.Starter_Room2; break;
        }
        while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, _salle, prompt, (response) => { text_Genere = response; }));
            done = Parser.STARTER(text_Genere, _gm, out caseText);
            if (!done)
            {
                yield return new WaitForSeconds(1); // Wait 1 second
            }
        }

        _gm._groupsManager.SetReady(0, true);
        //yield return StartCoroutine(WaitIsReady(0));
        _gm._groupsManager.SetCoordDescription(0, caseText);
        _gm._groupsManager.UpdateAllCanvasInventory();
        _UI_Manager.SwitchGroupCanvas(0);
        _UI_Manager.Hide_Loading_Screen();
        _UI_Manager.JustifierLeCodeVersLeBas();
        StartCoroutine(waitBeforeJustification());
    }

    /// <summary>
    /// Gets the resume of other groups.
    /// </summary>
    public string GetOtherGroupResume(int indexGroup)
    {
        int count = 1;
        string toReturn = "";
        if (_groupsManager.m_GroupsCount == 1)
        {
            return _gm._language.GetText("__other_group_informations_without__");
        }
        for (int i = 0; i < _groupsManager.m_GroupsCount; i++)
        {
            if (i == indexGroup || _groupsManager.m_Groups[i].isDead)
            {
                continue;
            }
            count++;
            var values = new Dictionary<string, string>
            {
                {"count",count.ToString()},
                {"group_name",_groupsManager.m_Groups[i].GetResume_Names()}
            };
            toReturn += _gm._language.GetPrompt("__other_group_informations_unknow__", values);
        }
        if (toReturn == "")
        {
            return _gm._language.GetText("__other_group_informations_without__");
        }
        else
        {
            toReturn += _gm._language.GetText("__other_group_informations_with__");
        }
        return toReturn;
    }

    #region END

    /// <summary>
    /// Generates a bad ending.
    /// </summary>
    public IEnumerator GenerateBadEnd(string lastSummaryCombat)
    {
        _gm._UI_Manager.Show_Loading_Screen();

        var values = new Dictionary<string, string>
        {
            { "theme", _memoryManager.GetTheme() },
            { "synopsis", _gm._memoryManager._synopsis },
            { "characters", _gm._charactersManagers.GetAllCharactersNameAndDescriptions() },
            { "historique", _groupsManager.m_Groups[0]._penultimatePenultimatePhaseResume + " ; " + _groupsManager.m_Groups[0]._penultimatePhaseResume },
            { "dernier_combat", lastSummaryCombat }
        };

        string prompt = _gm._language.GetPrompt("__prompt__bad_end", values);
        string text_Genere = "";
        bool done = false;
        string toReturn = "";
        while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
        {
            text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.BadEnd, prompt, (response) =>
            {
                text_Genere = response;
            }));
            done = Parser.END(text_Genere, out toReturn);
            if (!done)
            {
                yield return new WaitForSeconds(1); // Wait 1 second
            }
        }

        if (_gm.generateImage)
        {
            // We will generate an end image
            var valuesImage = new Dictionary<string, string>
            {
                { "theme", _memoryManager.GetTheme() },
                { "synopsis", _gm._memoryManager._synopsis },
                { "characters", _gm._charactersManagers.GetAllCharactersNameAndDescriptions() },
                { "final", lastSummaryCombat}
            };
            string promptImage = _gm._language.GetPrompt("__prompt__generation_image__bad_end", valuesImage);
            text_Genere = "";
            done = false;
            string prompt_Image = "";
            while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
            {
                text_Genere = "";
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.PromptImageMauvaiseFin, promptImage, (response) =>
                {
                    text_Genere = response;
                }));
                done = Parser.END_IMAGE(text_Genere, out prompt_Image);
                if (!done)
                {
                    yield return new WaitForSeconds(1); // Wait 1 second
                }
            }

            // We will generate an end image
            Texture2D image = null;

            yield return StartCoroutine(_gm.Generate_Image(GameManager.LLM_image.BlackForestFast, prompt_Image, (responseImage) => { image = responseImage; }));

            Sprite currentSprite = null;
            if (image == null)
            {
                currentSprite = _UI_Manager.m_ImageMauvaiseFinParDefault;
            }
            else
            {
                currentSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            }
            _UI_Manager.m_ImageFin.sprite = currentSprite;
        }

        _UI_Manager.Hide_Loading_Screen();
        _UI_Manager.m_Canvas_Fin.SetActive(true);
        _UI_Manager.m_txt_TitreFin.text = _gm._language.GetText("__title_fin_tragique");
        _UI_Manager.m_txt_SousTitreFin.text = _gm._language.GetText("__description_fin_tragique");
        _UI_Manager.m_txt_NarrationFin.text = toReturn;
    }

    /// <summary>
    /// Generates a good ending.
    /// </summary>
    public IEnumerator GenerateGoodEnd(string lastSalleSummary)
    {
        _gm._UI_Manager.Show_Loading_Screen();

        var values = new Dictionary<string, string>
        {
            { "theme", _memoryManager.GetTheme() },
            { "synopsis", _gm._memoryManager._synopsis },
            { "characters", _gm._charactersManagers.GetAllCharactersNameAndDescriptions() },
            { "historique", _groupsManager.m_Groups[0]._penultimatePenultimatePhaseResume + " ; " + _groupsManager.m_Groups[0]._penultimatePhaseResume },
            { "dernier_combat", lastSalleSummary }
        };

        string prompt = _gm._language.GetPrompt("__prompt__good_end", values);

        string text_Genere = "";
        bool done = false;
        string toReturn = "";
        while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
        {
            text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.GoodEnd, prompt, (response) =>
            {
                text_Genere = response;
            }));
            done = Parser.END(text_Genere, out toReturn);
            if (!done)
            {
                yield return new WaitForSeconds(1); // Wait 1 second
            }
        }

        if (_gm.generateImage)
        {
            // We will generate an end image
            var valuesImage = new Dictionary<string, string>
            {
                { "theme", _memoryManager.GetTheme() },
                { "synopsis", _gm._memoryManager._synopsis },
                { "characters", _gm._charactersManagers.GetAllCharactersNameAndDescriptions() },
                { "final", lastSalleSummary}
            };
            string promptImage = _gm._language.GetPrompt("__prompt__generation_image__good_end", valuesImage);

            text_Genere = "";
            done = false;
            string prompt_Image = "";
            while (!done) // While the text has not been generated (for example, there was an error), we resend the prompt
            {
                text_Genere = "";
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.PromptImageBonneFin, promptImage, (response) =>
                {
                    text_Genere = response;
                }));
                done = Parser.END_IMAGE(text_Genere, out prompt_Image);
                if (!done)
                {
                    yield return new WaitForSeconds(1); // Wait 1 second
                }
            }

            // We will generate an end image
            Texture2D image = null;

            yield return StartCoroutine(_gm.Generate_Image(GameManager.LLM_image.BlackForestFast, prompt_Image, (responseImage) => { image = responseImage; }));

            Sprite currentSprite = null;
            if (image == null)
            {
                currentSprite = _UI_Manager.m_ImageMauvaiseFinParDefault;
            }
            else
            {
                currentSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            }
            _UI_Manager.m_ImageFin.sprite = currentSprite;
        }

        _UI_Manager.Hide_Loading_Screen();
        _UI_Manager.m_Canvas_Fin.SetActive(true);
        _UI_Manager.m_txt_TitreFin.text = _gm._language.GetText("__title_fin_heureuse");
        _UI_Manager.m_txt_SousTitreFin.text = _gm._language.GetText("__description_fin_heureuse");
        _UI_Manager.m_txt_NarrationFin.text = toReturn;
    }

    #endregion
}
