using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using static Jobs;
using static MesureInfluences;
using static Models;

public class MesureInfluences : MonoBehaviour
{

    public ScenarioInfluence[] scenario_basic;
    public ScenarioInfluence[] scenario_coffre;
    public ScenarioInfluence[] scenario_poffre;
    public ScenarioInfluence[] scenario_combat;

    public int _countRepet = 10;
    public int _countTot = 10;

    public GameManager _gm;
    public bool _haveToRecord = false;

    public enum influencabilite
    {
        Flatterie,
        Supplication,
        Autorite,
        Relativisation,
        Normalisation,
        reformulation,
        contextualisation,
        humour,
        triche,
        Pression,
        Mensonge,
        aucune
    }
    public influencabilite m_influencabilite = influencabilite.aucune;

    public IEnumerator StartTests(Model model)
    {
        _gm.m_currentModel = model;
        int totalCount = 0;
        string modelString = model.ToString();

        // D'abord les tests basiques.
        foreach (ScenarioInfluence scenario in scenario_basic)
        {

            string prompt = scenario.m_prompt;

            string flatterie = scenario.m_Flatterie;
            string supplication = scenario.m_Supplication;
            string autorite = scenario.m_Autorite;
            string relativisation = scenario.m_Relativisation;
            string normalisation = scenario.m_Normalisation;
            string reformulation = scenario.m_Reformulation;
            string contextualisation = scenario.m_Contextualisation;
            string humour = scenario.m_Humour;
            string triche = scenario.m_Triche;
            string pression = scenario.m_Pression;
            string menssonge = scenario.m_Menssonge;
            string aucune = scenario.m_aucune;

            // TODO : On fait tourner le test avec aucun 10 fois
            int countLogique = 0;
            int countPasLogique = 0;

            for (int i = 0; i < _countRepet; i++)
            {
                yield return new WaitForSeconds(1);
                string _prompt = AddToPrompt(prompt, aucune);
                string justification = string.Empty;
                bool success = false;
                yield return StartCoroutine(callLLMCheckCoherence((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                if (success)
                {
                    countLogique++;
                }
                else
                {
                    countPasLogique++;
                }
                m_influencabilite = influencabilite.aucune;
                SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                totalCount++;
                Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
                
            }
            if(countLogique >+ countPasLogique)
            {
                continue;
            }
            // Création d'un tableau avec des valeurs directement assignées
            string[] allScenario = new string[] { flatterie, supplication, autorite, relativisation, normalisation, reformulation, contextualisation, humour, triche, pression, menssonge };

            // On fait tourner le test avec les autres choix
            int index = 0;
            foreach (string choix in allScenario)
            {

                for (int i = 0; i < _countRepet; i++)
                {
                    switch (index)
                    {
                        case 0:
                            m_influencabilite = influencabilite.Flatterie;
                            break;
                        case 1:
                            m_influencabilite = influencabilite.Supplication;
                            break;
                        case 2:
                            m_influencabilite = influencabilite.Autorite;
                            break;
                        case 3:
                            m_influencabilite = influencabilite.Relativisation;
                            break;
                        case 4:
                            m_influencabilite = influencabilite.Normalisation;
                            break;
                        case 5:
                            m_influencabilite = influencabilite.reformulation;
                            break;
                        case 6:
                            m_influencabilite = influencabilite.contextualisation;
                            break;
                        case 7:
                            m_influencabilite = influencabilite.humour;
                            break;
                        case 8:
                            m_influencabilite = influencabilite.triche;
                            break;
                        case 9:
                            m_influencabilite = influencabilite.Pression;
                            break;
                        case 10:
                            m_influencabilite = influencabilite.Mensonge;
                            break;
                        default:
                            break;
                    }
                    yield return new WaitForSeconds(1);
                    string _prompt = AddToPrompt(prompt, choix);
                    string justification = string.Empty;
                    bool success = false;
                    yield return StartCoroutine(callLLMCheckCoherence((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                    totalCount++;
                    Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
                    SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                }
                index++;
            }

        }



        // Les tests coffre
        foreach (ScenarioInfluence scenario in scenario_coffre)
        {

            string prompt = scenario.m_prompt;

            string flatterie = scenario.m_Flatterie;
            string supplication = scenario.m_Supplication;
            string autorite = scenario.m_Autorite;
            string relativisation = scenario.m_Relativisation;
            string normalisation = scenario.m_Normalisation;
            string reformulation = scenario.m_Reformulation;
            string contextualisation = scenario.m_Contextualisation;
            string humour = scenario.m_Humour;
            string triche = scenario.m_Triche;
            string pression = scenario.m_Pression;
            string menssonge = scenario.m_Menssonge;
            string aucune = scenario.m_aucune;

            // TODO : On fait tourner le test avec aucun 10 fois
            int countLogique = 0;
            int countPasLogique = 0;

            for (int i = 0; i < _countRepet; i++)
            {
                yield return new WaitForSeconds(1);
                string _prompt = AddToPrompt(prompt, aucune);
                string justification = string.Empty;
                bool success = false;
                yield return StartCoroutine(callLLMCheckCoherenceCoffre((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                if (success)
                {
                    countLogique++;
                }
                else
                {
                    countPasLogique++;
                }
                m_influencabilite = influencabilite.aucune;
                SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                totalCount++;
                Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
            }
            if (countLogique > +countPasLogique)
            {
                continue;
            }
            // Création d'un tableau avec des valeurs directement assignées
            string[] allScenario = new string[] { flatterie, supplication, autorite, relativisation, normalisation, reformulation, contextualisation, humour, triche, pression, menssonge };

            // On fait tourner le test avec les autres choix
            int index = 0;
            foreach (string choix in allScenario)
            {
                for (int i = 0; i < _countRepet; i++)
                {
                    yield return new WaitForSeconds(1);
                    switch (index)
                    {
                        case 0:
                            m_influencabilite = influencabilite.Flatterie;
                            break;
                        case 1:
                            m_influencabilite = influencabilite.Supplication;
                            break;
                        case 2:
                            m_influencabilite = influencabilite.Autorite;
                            break;
                        case 3:
                            m_influencabilite = influencabilite.Relativisation;
                            break;
                        case 4:
                            m_influencabilite = influencabilite.Normalisation;
                            break;
                        case 5:
                            m_influencabilite = influencabilite.reformulation;
                            break;
                        case 6:
                            m_influencabilite = influencabilite.contextualisation;
                            break;
                        case 7:
                            m_influencabilite = influencabilite.humour;
                            break;
                        case 8:
                            m_influencabilite = influencabilite.triche;
                            break;
                        case 9:
                            m_influencabilite = influencabilite.Pression;
                            break;
                        case 10:
                            m_influencabilite = influencabilite.Mensonge;
                            break;
                        default:
                            break;
                    }
                    string _prompt = AddToPrompt(prompt, choix);
                    string justification = string.Empty;
                    bool success = false;
                    yield return StartCoroutine(callLLMCheckCoherenceCoffre((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                    totalCount++;
                    Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
                    SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                }
                index++;
            }

        }

        // Les tests porte
        foreach (ScenarioInfluence scenario in scenario_coffre)
        {

            string prompt = scenario.m_prompt;

            string flatterie = scenario.m_Flatterie;
            string supplication = scenario.m_Supplication;
            string autorite = scenario.m_Autorite;
            string relativisation = scenario.m_Relativisation;
            string normalisation = scenario.m_Normalisation;
            string reformulation = scenario.m_Reformulation;
            string contextualisation = scenario.m_Contextualisation;
            string humour = scenario.m_Humour;
            string triche = scenario.m_Triche;
            string pression = scenario.m_Pression;
            string menssonge = scenario.m_Menssonge;
            string aucune = scenario.m_aucune;

            // TODO : On fait tourner le test avec aucun 10 fois
            int countLogique = 0;
            int countPasLogique = 0;

            for (int i = 0; i < _countRepet; i++)
            {
                yield return new WaitForSeconds(1);
                string _prompt = AddToPrompt(prompt, aucune);
                string justification = string.Empty;
                bool success = false;
                yield return StartCoroutine(callLLMCheckCoherencePorte((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                if (success)
                {
                    countLogique++;
                }
                else
                {
                    countPasLogique++;
                }
                m_influencabilite = influencabilite.aucune;
                SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                totalCount++;
                Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
            }
            if (countLogique > +countPasLogique)
            {
                continue;
            }
            // Création d'un tableau avec des valeurs directement assignées
            string[] allScenario = new string[] { flatterie, supplication, autorite, relativisation, normalisation, reformulation, contextualisation, humour, triche, pression, menssonge };

            // On fait tourner le test avec les autres choix
            int index = 0;
            foreach (string choix in allScenario)
            {
                for (int i = 0; i < _countRepet; i++)
                {
                    yield return new WaitForSeconds(1);
                    switch (index)
                    {
                        case 0:
                            m_influencabilite = influencabilite.Flatterie;
                            break;
                        case 1:
                            m_influencabilite = influencabilite.Supplication;
                            break;
                        case 2:
                            m_influencabilite = influencabilite.Autorite;
                            break;
                        case 3:
                            m_influencabilite = influencabilite.Relativisation;
                            break;
                        case 4:
                            m_influencabilite = influencabilite.Normalisation;
                            break;
                        case 5:
                            m_influencabilite = influencabilite.reformulation;
                            break;
                        case 6:
                            m_influencabilite = influencabilite.contextualisation;
                            break;
                        case 7:
                            m_influencabilite = influencabilite.humour;
                            break;
                        case 8:
                            m_influencabilite = influencabilite.triche;
                            break;
                        case 9:
                            m_influencabilite = influencabilite.Pression;
                            break;
                        case 10:
                            m_influencabilite = influencabilite.Mensonge;
                            break;
                        default:
                            break;
                    }
                    string _prompt = AddToPrompt(prompt, choix);
                    string justification = string.Empty;
                    bool success = false;
                    yield return StartCoroutine(callLLMCheckCoherencePorte((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                    totalCount++;
                    Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
                    SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                }
                index++;
            }

        }

        // Les tests combat
        foreach (ScenarioInfluence scenario in scenario_combat)
        {

            string prompt = scenario.m_prompt;

            string flatterie = scenario.m_Flatterie;
            string supplication = scenario.m_Supplication;
            string autorite = scenario.m_Autorite;
            string relativisation = scenario.m_Relativisation;
            string normalisation = scenario.m_Normalisation;
            string reformulation = scenario.m_Reformulation;
            string contextualisation = scenario.m_Contextualisation;
            string humour = scenario.m_Humour;
            string triche = scenario.m_Triche;
            string pression = scenario.m_Pression;
            string menssonge = scenario.m_Menssonge;
            string aucune = scenario.m_aucune;

            // TODO : On fait tourner le test avec aucun 10 fois
            int countLogique = 0;
            int countPasLogique = 0;

            for (int i = 0; i < _countRepet; i++)
            {
                yield return new WaitForSeconds(1);
                string _prompt = AddToPrompt(prompt, aucune);
                string justification = string.Empty;
                bool success = false;
                yield return StartCoroutine(callLLMCheckCoherenceCombat((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                if (success)
                {
                    countLogique++;
                }
                else
                {
                    countPasLogique++;
                }
                m_influencabilite = influencabilite.aucune;
                SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                totalCount++;
                Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
            }
            if (countLogique > +countPasLogique)
            {
                continue;
            }
            // Création d'un tableau avec des valeurs directement assignées
            string[] allScenario = new string[] { flatterie, supplication, autorite, relativisation, normalisation, reformulation, contextualisation, humour, triche, pression, menssonge };

            // On fait tourner le test avec les autres choix
            int index = 0;
            foreach (string choix in allScenario)
            {

                for (int i = 0; i < _countRepet; i++)
                {
                    switch (index)
                    {
                        case 0:
                            m_influencabilite = influencabilite.Flatterie;
                            break;
                        case 1:
                            m_influencabilite = influencabilite.Supplication;
                            break;
                        case 2:
                            m_influencabilite = influencabilite.Autorite;
                            break;
                        case 3:
                            m_influencabilite = influencabilite.Relativisation;
                            break;
                        case 4:
                            m_influencabilite = influencabilite.Normalisation;
                            break;
                        case 5:
                            m_influencabilite = influencabilite.reformulation;
                            break;
                        case 6:
                            m_influencabilite = influencabilite.contextualisation;
                            break;
                        case 7:
                            m_influencabilite = influencabilite.humour;
                            break;
                        case 8:
                            m_influencabilite = influencabilite.triche;
                            break;
                        case 9:
                            m_influencabilite = influencabilite.Pression;
                            break;
                        case 10:
                            m_influencabilite = influencabilite.Mensonge;
                            break;
                        default:
                            break;
                    }
                    yield return new WaitForSeconds(1);
                    string _prompt = AddToPrompt(prompt, choix);
                    string justification = string.Empty;
                    bool success = false;
                    yield return StartCoroutine(callLLMCheckCoherenceCombat((response) => { justification = response; }, (booleanR) => { success = booleanR; }, _prompt));
                    totalCount++;
                    Debug.Log($"Update for {modelString} : {totalCount} / {_countTot}");
                    SaveToCSV(_gm, _prompt, justification, _gm.m_currentModel, success, m_influencabilite);
                }
                index++;
            }

        }


        
    }

    private static void SaveToCSV(GameManager _gm, string prompt, string response, Model model, bool success, influencabilite type)
    {
        string debugFilePath = Path.Combine(Application.streamingAssetsPath, "Prompts_Mesures_Influencabilite.csv");
        // Utilisation de `;` comme séparateur pour Excel FR
        string formattedLine = $"\"{prompt.Replace("\"", "\"\"")}\";\"{response.Replace("\"", "\"\"")}\";\"{model}\";\"{success}\";{type}\n";

        AppendTextToFile(debugFilePath, formattedLine);



    }
    private static void AppendTextToFile(string filePath, string content)
    {
        // Vérifie si le fichier existe
        using (StreamWriter writer = new StreamWriter(filePath, true, new UTF8Encoding(true)))
        {
            writer.WriteLine(content);
        }
    }


    public IEnumerator callLLMCheckCoherence(Action<string> justification, Action<bool> logique, string prompt)
    {
        string statut = "[ERROR]";
        string txt_generated = "";
        string _justification = "";
        while (statut == "[ERROR]")
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.CheckCoherenceInput, prompt, (response) => { txt_generated = response; }));
            statut = ParserCoherenceInputMesureInfluenceStandard(_gm, txt_generated, out _justification);
            if (statut == "[ERROR]")
            {
                yield return new WaitForSeconds(1);
            }
        }
        bool success = statut == "True" ? true : false;
        justification?.Invoke(_justification);
        logique?.Invoke(success);
    }

    public string ParserCoherenceInputMesureInfluenceStandard(GameManager _gameManager, string _input, out string justifi)
    {

        bool _coherent = false;
        bool _coffre = false;
        bool _porte = false;
        string _summary = string.Empty;
        string _justification = string.Empty;
        justifi = string.Empty;
        _input = Parser.CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["logique"] == null || data["coffre"] == null || data["porte"] == null || data["summary"] == null || data["justification"] == null)
            {
                return "[ERROR]";
            }

            _coherent = data["logique"].ToObject<bool>();

            if (!_coherent)
            {
                _justification = data["justification"].ToString();
                justifi = _justification;
                _gameManager._UI_Manager.SetIncoherenceInfo(_justification);
                return "False";

            }
            _summary = data["summary"].ToString();

            _coffre = data["coffre"].ToObject<bool>();
            _porte = data["porte"].ToObject<bool>();
            _justification = data["justification"].ToString();
            justifi = _justification;
            if (_coffre)
            {
                return "True";
            }
            if (_porte)
            {
                return "True";
            }

            return "True"; // Parsing réussi

        }

        catch (Exception e)
        {
            return "[ERROR]";
        }
    }



    public IEnumerator callLLMCheckCoherenceCoffre(Action<string> justification, Action<bool> logique, string prompt)
    {
        string statut = "[ERROR]";
        string txt_generated = "";
        string _justification = "";
        while (statut == "[ERROR]")
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.CheckCoherenceInput, prompt, (response) => { txt_generated = response; }));
            statut = ParserCoherenceInputMesureInfluenceStandardCoffre(_gm, txt_generated, out _justification);
            if (statut == "[ERROR]")
            {
                yield return new WaitForSeconds(1);
            }
        }
        bool success = statut == "True" ? true : false;
        justification?.Invoke(_justification);
        logique?.Invoke(success);
    }


    public string ParserCoherenceInputMesureInfluenceStandardCoffre(GameManager _gameManager, string _input, out string justifi)
    {

        bool _coherent = false;
        bool _coffre = false;
        bool _porte = false;
        string _summary = string.Empty;
        string _justification = string.Empty;
        justifi = string.Empty;
        _input = Parser.CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["logique"] == null || data["coffre"] == null || data["porte"] == null || data["summary"] == null || data["justification"] == null)
            {
                return "[ERROR]";
            }

            _coherent = data["logique"].ToObject<bool>();

            if (!_coherent)
            {
                _justification = data["justification"].ToString();
                justifi = _justification;
                _gameManager._UI_Manager.SetIncoherenceInfo(_justification);
                return "False";

            }
            _summary = data["summary"].ToString();

            _coffre = data["coffre"].ToObject<bool>();
            _porte = data["porte"].ToObject<bool>();
            _justification = data["justification"].ToString();
            justifi = _justification;
            if (_coffre)
            {
                return "True";
            }
            return "False"; // Parsing réussi

        }

        catch (Exception e)
        {
            return "[ERROR]";
        }
    }


    public IEnumerator callLLMCheckCoherencePorte(Action<string> justification, Action<bool> logique, string prompt)
    {
        string statut = "[ERROR]";
        string txt_generated = "";
        string _justification = "";
        while (statut == "[ERROR]")
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.CheckCoherenceInput, prompt, (response) => { txt_generated = response; }));
            statut = ParserCoherenceInputMesureInfluenceStandardPorte(_gm, txt_generated, out _justification);
            if (statut == "[ERROR]")
            {
                yield return new WaitForSeconds(1);
            }
        }
        bool success = statut == "True" ? true : false;
        justification?.Invoke(_justification);
        logique?.Invoke(success);
    }


    public string ParserCoherenceInputMesureInfluenceStandardPorte(GameManager _gameManager, string _input, out string justifi)
    {

        bool _coherent = false;
        bool _coffre = false;
        bool _porte = false;
        string _summary = string.Empty;
        string _justification = string.Empty;
        justifi = string.Empty;
        _input = Parser.CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["logique"] == null || data["coffre"] == null || data["porte"] == null || data["summary"] == null || data["justification"] == null)
            {
                return "[ERROR]";
            }

            _coherent = data["logique"].ToObject<bool>();

            if (!_coherent)
            {
                _justification = data["justification"].ToString();
                justifi = _justification;
                _gameManager._UI_Manager.SetIncoherenceInfo(_justification);
                return "False";

            }
            _summary = data["summary"].ToString();

            _coffre = data["coffre"].ToObject<bool>();
            _porte = data["porte"].ToObject<bool>();
            _justification = data["justification"].ToString();
            justifi = _justification;
            if (_porte)
            {
                return "True";
            }
            return "False"; // Parsing réussi

        }

        catch (Exception e)
        {
            return "[ERROR]";
        }
    }








    public IEnumerator callLLMCheckCoherenceCombat(Action<string> justification, Action<bool> logique, string prompt)
    {
        string statut = "[ERROR]";
        string txt_generated = "";
        string _justification = "";
        while (statut == "[ERROR]")
        {
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.ChechCoherenceInputCombat, prompt, (response) => { txt_generated = response; }));
            statut = CheckCoherenceInputCombat(txt_generated, out _justification);
            if (statut == "[ERROR]")
            {
                yield return new WaitForSeconds(1);
            }
        }
        bool success = statut == "True" ? true : false;
        justification?.Invoke(_justification);
        logique?.Invoke(success);
    }

    public string CheckCoherenceInputCombat(string _input, out string _justification)
    {
        _justification = string.Empty;
        _input = Parser.CLEAN_JSON(_input);

        try
        {
            // Parse le string en objet JSON
            JObject jsonResponse = JObject.Parse(_input);

            // Récupère les valeurs
            bool isCoherent = jsonResponse["cohérence"]?.ToObject<bool>() ?? false;
            _justification = jsonResponse["justification"]?.ToString() ?? string.Empty;

            // Retourne le résultat sous forme de string [True] ou [False]
            return isCoherent ? "True" : "False";
        }
        catch (Exception ex)
        {
            return "[ERROR]";
        }
    }







    public string AddToPrompt(string prompt, string choix)
    {
        return prompt + choix + "\n}}";
        
    }
}
