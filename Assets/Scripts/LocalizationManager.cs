using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using System;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    private Dictionary<string, Dictionary<string, string>> localizedTexts;
    public string currentLanguage = "fr";

    private string localFilePath => Path.Combine(Application.persistentDataPath, "language.json");
    private string remoteUrl = "https://rpgwithllm.z28.web.core.windows.net/StreamingAssets/language.json";

    private bool isReady = false;

    [Header("OtherFileToTakeIntoAccount")]
    private S_LoadingAnimationEvent _LoadingAnimationEvent;
    private UI_Inventaire_Buttons _Inventaire_Buttons;
    private Language_Panneau _Langue_Panneau;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitializeLocalization());
        }
        else
        {
            Instance.SetLanguage(Instance.currentLanguage);
            Instance.OnValueChangeDropDownMenu(-1);
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeLocalization()
    {
        // 1. Charger depuis le fichier local s’il existe
        if (File.Exists(localFilePath))
        {
            string localJson = File.ReadAllText(localFilePath);
            ParseLocalization(localJson);
        }

        // 2. Télécharger depuis le serveur (et écraser le cache local si succès)
        UnityWebRequest www = UnityWebRequest.Get(remoteUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string downloadedJson = www.downloadHandler.text;
            ParseLocalization(downloadedJson);

            File.WriteAllText(localFilePath, downloadedJson);
        }
    }

    private void ParseLocalization(string json)
    {
        localizedTexts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
        isReady = true;
    }

    public string GetText(string key)
    {
        if (localizedTexts.TryGetValue(key, out var translations))
        {
            if (translations.TryGetValue(currentLanguage, out var translated))
                return translated;
        }

        return $"[MISSING KEY: {key}]"; // Clé non trouvée
    }


    public void SetLanguage(string lang)
    {
        currentLanguage = lang;

        foreach (var loc in FindObjectsOfType<LocalizedText>())
        {
            loc.UpdateText();
        }

        //Pour tous les autres scripts à mettre à jour
        UpdateLanguageComponent(ref _LoadingAnimationEvent, comp => comp.UpdateLanguage(currentLanguage));
        UpdateLanguageComponent(ref _Inventaire_Buttons, comp => comp.UpdateLanguage(currentLanguage));
        UpdateLanguageComponent(ref _Langue_Panneau, comp => comp.UpdateLanguage(currentLanguage));


    }


    private void UpdateLanguageComponent<T>(ref T component, Action<T> updateAction) where T : MonoBehaviour
    {
        if (component == null)
        {
            component = FindFirstObjectByType<T>();
        }

        if (component != null)
        {
            updateAction(component);
        }
    }

    public string GetPrompt(string key, Dictionary<string, string> variables)
    {
        string rawTemplate = GetText(key); // Récupère la version traduite selon la langue

        foreach (var pair in variables)
        {
            rawTemplate = rawTemplate.Replace($"{{{pair.Key}}}", pair.Value);
        }

        return rawTemplate;
    }

    public void OnValueChangeDropDownMenu(int i)
    {
        switch (i)
        {
            case -1:
                switch (currentLanguage)
                {
                    case "fr":
                        i = 0;
                        break;
                    case "en":
                        i = 1;
                        break;
                    default:
                        break;
                }
                break;
            case 0:
                SetLanguage("fr");
                break;
            case 1:
                SetLanguage("en");
                break;
            default:
                break;
        }

        MainMenu _mm = FindObjectOfType<MainMenu>();
        if (_mm != null)
        {
            _mm.ChangeUIOptionLanguage(i);
        }
        UI_SelectPerso _sp = FindObjectOfType<UI_SelectPerso>();
        if (_sp != null)
        {
            _sp.ChangeUIOptionLanguage(i);
        }

    }

}
