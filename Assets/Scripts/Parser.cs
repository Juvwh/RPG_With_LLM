using UnityEngine;
using System.Text.RegularExpressions;
using static Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Windows;
using static UI_Manager;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System.Text;
public class Parser
{
    public static string[] SYNOPSYS(string _input)
    {
        _input = CLEAN_JSON(_input);
        string _synopsis = string.Empty;
        string _resume = string.Empty;
        string _salle_1 = string.Empty;
        string _salle_2 = string.Empty;
        string _salle_3 = string.Empty;

        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["synopsis"] == null || data["summary"] == null || data["salle_1"] == null || data["salle_2"] == null || data["salle_3"] == null)
            {
                return new string[0];
            }
            string[] toReturn = new string[5];
            toReturn[0] = data["synopsis"].ToString();
            toReturn[1] = data["summary"].ToString();
            toReturn[2] = data["salle_1"].ToString();
            toReturn[3] = data["salle_2"].ToString();
            toReturn[4] = data["salle_3"].ToString();

            return toReturn; // Parsing réussi
        }
        catch (Exception e)
        {
            return new string[0];
        }
    }
    public static bool STARTER(string _input, GameManager _gm, out string _caseDescr)
    {

        string _summary = string.Empty;
        string _text = string.Empty;
        _input = CLEAN_JSON(_input);
        _caseDescr = string.Empty;
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["case"] == null || data["summary"] == null || data["histoire"] == null)
            {
                return false;
            }
;
            _caseDescr = data["case"].ToString();
            _summary = data["summary"].ToString();
            _text = data["histoire"].ToString();

            _gm._groupsManager.AddHistory(0, UI_Manager.TextType.Histoire, _text);
            _gm._groupsManager.AddHistoryResume(0, _summary);

            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public static bool FREE(string _input, out string _text, out string _summary, out string _caseDescr)
    {
        _summary = string.Empty;
        _text = string.Empty;
        _input = CLEAN_JSON(_input);
        _caseDescr = string.Empty;
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["case"] == null || data["summary"] == null || data["histoire"] == null)
            {
                return false;
            }
;
            _caseDescr = data["case"].ToString();
            _summary = data["summary"].ToString();
            _text = data["histoire"].ToString();

            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public static bool  FREE_COMBAT(string _input, out string text, out string summary)
    {
        summary = string.Empty;
        text = string.Empty;
        _input = CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["text"] == null || data["summary"] == null )
            {
                return false;
            }
;
            text = data["text"].ToString();
            summary = data["summary"].ToString();

            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public static string CLEAN(string input)
    {
        if (input.StartsWith(":"))
        {
            input = input.Substring(1);
        }
        if (input == "")
        {
            return input;
        }
        // Remplacer chaque caractère spécial par une chaîne vide
        return input
            .Replace("\"", "")  // Retirer les guillemets
            .Replace("\n", "")  // Retirer les retours à la ligne
            .Replace("*", "")   // Retirer les étoiles
            .Replace("<", "")   // Retirer les étoiles
            .Replace(">", "")   // Retirer les étoiles
            .Replace("#", "");  // Retirer les symboles dièse
    }
    public static bool THEME(string _input, out string textToReturn)
    {
        bool _coherent = false;
        textToReturn = string.Empty;
        _input = Parser.CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["logique"] == null || data["summary"] == null || data["justification"] == null)
            {
                if (data["justification"] != null)
                {
                    textToReturn = data["justification"].ToString();
                }
                return false;
            }

            _coherent = data["logique"].ToObject<bool>();

            if (!_coherent)
            {
                textToReturn = data["justification"].ToString();
                return false;

            }
            textToReturn = data["summary"].ToString();

            return true; // Parsing réussi

        }

        catch (Exception e)
        {

            textToReturn = "Erreur";

            return false;
        }
    }
    public static bool TRANSLATE(string _input, GameManager _gm)
    {
        // Nettoyer le format du JSON si nécessaire
        _input = CLEAN_JSON(_input);

        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si le tableau "personnage" est présent et contient au moins un élément
            JArray personnages = (JArray)data["personnage"];
            if (personnages == null || personnages.Count == 0)
            {
                return false;
            }

            // Utilisation de StringBuilder pour une concatenation plus performante
            StringBuilder result = new StringBuilder();

            // Itère sur chaque personnage
            int i = 0;
            foreach (JObject personnage in personnages)
            {
                // Vérifie si les champs attendus sont présents
                string description = personnage["description"]?.ToString();
                string race = personnage["race"]?.ToString();
                string classe = personnage["classe"]?.ToString();
                string arme = personnage["arme"]?.ToString();

                if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(race) || string.IsNullOrEmpty(classe) || string.IsNullOrEmpty(arme))
                {
                    return false;
                }

                PlayerData pd = _gm._charactersManagers._Characters[i];

                // Ajoute les informations du personnage au résultat
                pd.playerResume = description;
                pd.playerRace = race;
                pd.playerClass = classe;
                pd.playerArme = arme;
                i++;
            }
            return true; // Parsing réussi
        }
        catch
        {
            return false;
        }

    }
    public static EnemyData[] COMBAT(string jsonInput, int nbEnnemies)
    {
        EnemyData[] ennemies = new EnemyData[nbEnnemies];
        jsonInput = CLEAN_JSON(jsonInput);
        try
        {
            // Désérialise le JSON en une liste d'ennemis
            var data = JsonConvert.DeserializeObject<RootEnemies>(jsonInput);

            if (data?.enemies != null && data.enemies.Count > 0)
            {
                for (int i = 0; i < nbEnnemies; i++)
                {
                    ennemies[i] = new EnemyData(
                        data.enemies[i].name,
                        100, // hp
                        0, //armure
                        0, // index
                        data.enemies[i].description, // description
                        data.enemies[i].physical_description, // physique
                        data.enemies[i].size, // type
                        null // ui
                    );
                }
            }
            else
            {
                return new EnemyData[0];
            }
        }
        catch (JsonException e)
        {
            return new EnemyData[0];
        }

        return ennemies;

    }
    public static EnemyData[] COMBAT_BOSS(string jsonInput, int nbEnnemies)
    {
        EnemyData[] ennemies = new EnemyData[nbEnnemies];
        jsonInput = CLEAN_JSON(jsonInput);
        try
        {

            JObject boss = JObject.Parse(jsonInput);

            if (boss["name"] == null || boss["physical_description"] == null || boss["description"] == null)
            {
                return new EnemyData[0];
            }

            ennemies[0] = new EnemyData(
                boss["name"].ToString(),
                100,
                0,
                0,
                boss["description"].ToString(),
                boss["physical_description"].ToString(),
                "Boss",
                null
            );
        }
        catch (JsonException e)
        {
            return new EnemyData[0];
        }

        return ennemies;
    }
    public static string CLEAN_JSON(string input)
    {
        // Supprime les balises de code JSON
        input = Regex.Replace(input, @"^```json\s*|\s*```$", "").Trim();

        // Trouve la position du premier '{'
        int firstBraceIndex = input.IndexOf('{');

        // Trouve la position du dernier '}'
        int lastBraceIndex = input.LastIndexOf('}');

        // Vérifie si les deux caractères ont été trouvés
        if (firstBraceIndex != -1 && lastBraceIndex != -1 && firstBraceIndex < lastBraceIndex)
        {
            // Extrait la sous-chaîne entre le premier '{' et le dernier '}'
            input = input.Substring(firstBraceIndex, lastBraceIndex - firstBraceIndex + 1);
        }
        else
        {
            // Si les caractères ne sont pas trouvés, retourne une chaîne vide ou gère l'erreur
            input = string.Empty;
        }

        return input.Trim();
    }
    public static bool COMBAT_SEQUENCE(string _input, out string _narration, out string _etat)
    {
        _narration = string.Empty;
        _etat = string.Empty;
        _input = CLEAN_JSON(_input);
        try
        {
            // Parse la réponse JSON
            JObject jsonResponse = JObject.Parse(_input);

            // Récupère les valeurs attendues
            _narration = jsonResponse["narration"]?.ToString() ?? string.Empty;
            _etat = jsonResponse["etat"]?.ToString() ?? string.Empty;

            // Vérifie que les deux champs ne sont pas vides
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
    public static string LLM_VALIDATION_COMBAT(string _input, out string _txt_Incoherence, out string _txt_Summary)
    {
        _txt_Incoherence = string.Empty;
        _txt_Summary = string.Empty;
        _input = CLEAN_JSON(_input);

        try
        {
            // Parse le string en objet JSON
            JObject jsonResponse = JObject.Parse(_input);

            // Récupère les valeurs
            bool isCoherent = jsonResponse["cohérence"]?.ToObject<bool>() ?? false;
            _txt_Incoherence = jsonResponse["justification"]?.ToString() ?? string.Empty;
            _txt_Summary = jsonResponse["summary"]?.ToString() ?? string.Empty;

            // Retourne le résultat sous forme de string [True] ou [False]
            return isCoherent ? "[True]" : "[False]";
        }
        catch (Exception ex)
        {
            return "[Error]";
        }
    }
    public static bool COMBAT_ENEMY_SEQUENCE(string _input, out bool _isAttack, out string _cible, out string _strategie)
    {
        _isAttack = false;
        _cible = string.Empty;
        _input = CLEAN_JSON(_input);
        _strategie = string.Empty;
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["attaquer"] == null || data["soigner"] == null || data["cible"] == null || data["justification"] == null)
            {
                return false;
            }

            // Récupère les valeurs des champs
            bool attaquer = data["attaquer"].ToObject<bool>();
            bool soigner = data["soigner"].ToObject<bool>();
            _cible = data["cible"].ToString();
            _strategie = data["justification"].ToString();

            // Vérifie que "attaquer" et "soigner" ne sont pas tous les deux vrais ou faux en même temps
            if (attaquer == soigner)
            {
                return false;
            }

            // Stocke la décision d'attaque
            _isAttack = attaquer;

            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public static bool BEGGING(string _input, out string _text)
    {
        _text = string.Empty;
        _input = CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);
            // Vérifie si les champs attendus sont bien présents
            if (data["texte"] == null)
            {
                return false;
            }
            _text = data["texte"].ToString();
            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public static bool MAP_TAG(string _input, out bool _hall, out bool _couloir, out bool _grand)
    {
        _input = CLEAN_JSON(_input);
        _hall = false;
        _couloir = false;
        _grand = false;
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);
            // Vérifie si les champs attendus sont bien présents
            if (data["hall"] == null || data["couloir"] == null || data["grand"] == null)
            {
                return false;
            }
            _hall = data["hall"].ToObject<bool>();
            _couloir = data["couloir"].ToObject<bool>();
            _grand = data["grand"].ToObject<bool>();
            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }

    }
    public static bool END(string _input, out string _output)
    {
        _output = string.Empty;
        _input = CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);
            // Vérifie si les champs attendus sont bien présents
            if (data["fin"] == null)
            {
                return false;
            }
            _output = data["fin"].ToString();
            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
    public static bool END_IMAGE(string _input, out string _output)
    {
        _output = string.Empty;
        _input = CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);
            // Vérifie si les champs attendus sont bien présents
            if (data["prompt"] == null)
            {
                return false;
            }
            _output = data["prompt"].ToString();
            return true; // Parsing réussi
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
[System.Serializable]
public class Enemy
{
    public string name;
    public string size;
    public string physical_description;
    public string description;
}
[System.Serializable]
public class RootEnemies
{
    public List<Enemy> enemies;
}
public class RootBoss
{
    public List<Enemy> boss;
}
