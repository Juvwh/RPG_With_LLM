using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using static UI_Manager;
using static Jobs;
using static AgentManager;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine.Windows;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;

public class Coffre : Evenement
{
    #region Managers
    public GameManager _gm;
    #endregion
    #region Methods
    private void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        _gm._coffre = this;
    }
    public IEnumerator Open_Chest(int groupIndex)
    {
        bool done = false;
        string _txtGenerated = "";
        string groupID = _gm._groupsManager.m_Groups[groupIndex]._groupID;
        List<(string Nom, string Description)> all_items = new List<(string Nom, string Description)>();
        List<string> all_tags;
        PlayerData[] playerDatas = _gm._groupsManager.m_Groups[groupIndex].m_Players;

        bool descriptionCase = false;


        //On va regarder à côte de chaque joueur si il y a un coffre, et si il y a en effet un coffre on ajoute la coordonnée à une liste de Coord.
        HashSet<Coord> coffresAccessibles = new HashSet<Coord>(); // Remplace List par HashSet

        foreach (PlayerData player in playerDatas)
        {
            HashSet<Coord> voisins = player.playerPosition.GetNeighbours(1);
            foreach (Coord voisin in voisins)
            {
                if (voisin._event == Events.EventCoord.Loot && !voisin._isVisited && voisin._isVisible)
                {
                    coffresAccessibles.Add(voisin); // HashSet empêche les doublons automatiquement
                }
            }
        }


        //Maintenant qu'on a récupérer tous les coffres voisins accessibles, on va les regarder un à un.
        foreach (Coord coffre in coffresAccessibles)
        {
            //Lancer l'animation d'ouverture de coffre.
            _gm._UI_Inventaire_Buttons.LaunchAnimationCoffre();
            //TODO

            //On créer le prompt en regardant ce qui a das le coffre
            done = false;
            string txt_Coffre = coffre._description;
            var (count, tags) = ParseLootString(txt_Coffre);
            all_tags = tags;
            var values = new Dictionary<string, string>()
            {
                {"theme", _gm._memoryManager.GetTheme()},
                {"synopsys", _gm._memoryManager.GetSynopsis()},
                {"count", count.ToString()}
            };
            string prompt = _gm._language.GetPrompt("__createPrompt__Generate_Loot__", values);
            foreach (var tag in tags)
            {
                prompt += tag;
            }

            //On demande à un agent de générer les descriptions des objets
            while(!done)
            {
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Inventaire, prompt, (response) => { _txtGenerated = response;}));


                done = ParseItems(_txtGenerated, out List<(string Nom, string Description)> items, count);
                all_items = items;
                if (!done)
                {
                    yield return new WaitForSeconds(1); // Attendre 1 secondes avant de réessayer
                }
            }
            
            //On a maintenant tous les noms et descriptions dans "all_items"
            //On va maintenant ajouter les objets dans l'inventaire du groupe et créer l'UI
            _gm._UI_Manager.SwitchGroupCanvas(groupIndex);
            _gm._UI_Manager.m_ChargementInput.SetActive(false);
            _gm._UI_Inventaire_Buttons.countItemToAttribute = all_items.Count;
            _gm._UI_Inventaire_Buttons.isAttributingItem = true;
            _gm._UI_Inventaire_Buttons.Create_UI(all_tags, all_items);

            //On va générer l'information qu'on va mettre sur la case.
            if (!descriptionCase) //On vérifie qu'on n'a pas encore générer quelque chose pour cette case (pour éviter de générer plusieurs fois la même chose)
            {
                var values2 = new Dictionary<string, string>()
                {
                    {"theme", _gm._memoryManager.GetTheme()},
                    {"synopsys", _gm._memoryManager._synopsisResumed},
                    {"environnement", _gm._memoryManager.GetMemoryRoom(_gm._historyManager._currentRoom)},
                    {"cases", _gm._groupsManager.m_Groups[groupIndex].GetResume_InVision(true)}
                };
                string txt_Prompt_Case = _gm._language.GetPrompt("__createPrompt__Generate_case_description__", values2);

                string txt_Case = "Erreur";
                while(txt_Case.Contains("Erreur") || txt_Case.Contains("Error"))
                {
                    yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Case_Description, txt_Prompt_Case, (responsellm) => { txt_Case = responsellm; }));
                    if(txt_Case.Contains("Erreur") || txt_Case.Contains("Error"))
                    {
                        yield return new WaitForSeconds(1);
                    }
                }
                
                _gm._groupsManager.m_Groups[groupIndex].SetCoordDescription(txt_Case);
                descriptionCase = true;
            }



            //On va maintenant attendre que le joueur ait attribué tous les objets
            while (_gm._UI_Inventaire_Buttons.isAttributingItem)
            {
                //Cette ligne de code est un peu moche, mais elle permet de ne pas bloquer le thread principal
                yield return new WaitForSeconds(1); // Attends 3 secondes avant de réessayer
            }

        }

        //On a fini d'attribuer tous les objets, on peut maintenant fermer le coffre ou les coffre.
        foreach (Coord coffre in coffresAccessibles)
        {
            coffre._isVisited = true;
            coffre._event = Events.EventCoord.Nothing;
            coffre._description = _gm._language.GetText("__case__non_visitee__");
        }

        

        //On peut maintenant continuer l'histoire pour ce groupe.
        _gm._groupsManager.SetReady(groupIndex, true);
        //Reautorisé le groupe à se déplacer.
        //Afficher sur la carte leur nouvelle vision.
        _gm._groupsManager.m_Groups[groupIndex].SetReady(true);
        _gm._groupsManager.m_Groups[groupIndex].SetMoved(false);
        foreach (PlayerData playerData in _gm._groupsManager.m_Groups[groupIndex].m_Players)
        {
            _gm._mapManager.visitFloorTile(playerData.playerPosition, playerData.playerVision);        
        }
        _gm._mapManager.rebuildVector(new Vector2Int(0, 0)); 

        _gm._groupsManager.SetIsCheckingCoherence(groupIndex, false);
        _gm._UI_Manager.SwitchGroupCanvas(groupIndex);

        _gm._historyManager.Check_Enemy_Around(groupIndex);
    }
    static (int, List<string>) ParseLootString(string input)
    {
        List<string> tags = new List<string>();
        string pattern = "\\[(.*?)\\]";

        MatchCollection matches = Regex.Matches(input, pattern);

        foreach (Match match in matches)
        {
            tags.Add(match.Value);
        }

        return (tags.Count, tags);
    }
    public static bool ParseItems(string _input, out List<(string, string)> items, int count)
    {
        items = new List<(string, string)>();
        if (string.IsNullOrWhiteSpace(_input)) { return false; }
        _input = Parser.CLEAN_JSON(_input);

        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie que le tableau d'objets existe
            if (data["items"] == null || !data["items"].HasValues)

            {
                UnityEngine.Debug.LogError("JSON incomplet ou vide : pas d'objets trouvés !");
                return false;
            }

            // Parcourt chaque objet et le convertit en ItemData
            foreach (JToken item in data["items"])
            {
                if (item["label"] == null || item["name"] == null || item["description"] == null)
                {
                    return false;
                }

                string label = item["label"].ToString();   // Extracts the label inside [] instead of NOM
                string nom = item["name"].ToString();
                string description = item["description"].ToString();
                items.Add((nom, description));

            }
            if(items.Count != count)
            {
                return false;
            }
            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"Erreur de parsing JSON : {e.Message}");
            return false;
        }
    }
    #endregion
}
