using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UI_SelectPerso;
using System;
using UnityEngine.Networking;

public class CharacterSelection : MonoBehaviour
{
    #region Variables
    public PlayerData m_PlayerData;
    public UI_SelectPerso m_UI_SelectPerso;
    public string _NameImage;
    public string _name;
    public string _age;
    public string _race;
    public string _class;
    public string _atouts;
    public string _handicaps;
    public string _description;
    public string _arme;
    public GameObject me;
    public Image m_Cadre;
    public Image m_ImagePlayer;
    public bool m_isSelected = false;
    #endregion
    #region Methods
    public IEnumerator Initialise(string spriteName, Action<bool> callback)
    {
        me = gameObject;
        m_UI_SelectPerso = FindFirstObjectByType<UI_SelectPerso>();
        _NameImage = spriteName;
        m_Cadre = transform.GetChild(1).GetComponent<Image>();
        m_ImagePlayer = transform.GetChild(0).GetComponent<Image>();
        bool done = false;
        yield return StartCoroutine(LoadPlayerData((response) => { done = response; }));
        callback?.Invoke(done);

    }
    public void Initialise(string spriteName, SerializablePlayerData serializablePlayerData)
    {
        me = gameObject;
        m_UI_SelectPerso = FindFirstObjectByType<UI_SelectPerso>();
        _NameImage = spriteName;
        m_Cadre = transform.GetChild(1).GetComponent<Image>();
        m_ImagePlayer = transform.GetChild(0).GetComponent<Image>();
        LoadPlayerDataAfterGeneration(serializablePlayerData);
    }
    public IEnumerator LoadPlayerData(Action<bool> callback)
    {
        int index = _NameImage.IndexOf('_');

        // Extraire la sous-chaîne après '_'
        string numberString = _NameImage.Substring(index + 1);

        // Convertir la sous-chaîne en un entier
        int number = int.Parse(numberString);

        // Construire le chemin complet vers le fichier JSON
        string path = FixWebGLPath(Path.Combine(Application.streamingAssetsPath, "SavedPlayerInfo", _NameImage + ".json"));

        yield return StartCoroutine(ReadPlayerData(path, (playerData) =>
        {
            m_PlayerData = playerData;
            if(m_PlayerData == null)
            {
                callback?.Invoke(false);    
            }
        }));


        _name = m_PlayerData.playerName;
        _age = m_PlayerData.playerAge;
        _race = m_PlayerData.playerRace;
        _class = m_PlayerData.playerClass;
        _atouts = m_PlayerData.playerAdvantage;
        _handicaps = m_PlayerData.playerDisadvantage;
        _description = m_PlayerData.playerDescription;
        _arme = m_PlayerData.playerArme;
        callback?.Invoke(true);
    }
    public void LoadPlayerDataAfterGeneration(SerializablePlayerData serializedData)
    {
        
        PlayerData playerData = me.AddComponent<PlayerData>();

        playerData.playerName = serializedData.playerName;
        playerData.playerAge = serializedData.playerAge;
        playerData.playerRace = serializedData.playerRace;
        playerData.playerClass = serializedData.playerClass;
        playerData.playerAdvantage = serializedData.playerAdvantage;
        playerData.playerDisadvantage = serializedData.playerDisadvantage;
        playerData.playerDescription = serializedData.playerDescription;
        playerData.playerResume = serializedData.playerResume;
        playerData.playerArme = serializedData.playerArme;
        playerData.playerSprite = serializedData.playerSprite;
        playerData.playerInventaire = serializedData.playerInventaire;
        playerData.playerHealth = serializedData.playerHealth;
        playerData.playerHealthNbr = serializedData.playerHealthNbr;
        playerData.playerActions = serializedData.playerActions;
        playerData.playerTags = serializedData.playerTags;
        playerData.playerVision = serializedData.playerVision;
        try { playerData.playerArmePuissance = int.Parse(serializedData.playerArmePuissance); } catch { playerData.playerArmePuissance = 1; }

        m_PlayerData = playerData;
        _name = m_PlayerData.playerName;
        _age = m_PlayerData.playerAge;
        _race = m_PlayerData.playerRace;
        _class = m_PlayerData.playerClass;
        _atouts = m_PlayerData.playerAdvantage;
        _handicaps = m_PlayerData.playerDisadvantage;
        _description = m_PlayerData.playerDescription;
        _arme = m_PlayerData.playerArme;
    }
    public string FixWebGLPath(string path)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Si le chemin commence par "/https:/", retirer le premier "/"
            if (path.StartsWith("/https:/"))
            {
                path = path.Substring(1); // Retire le premier caractère "/"
            }
        }
        return path;
    }
    public IEnumerator DownloadFile(string url, Action<byte[]> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(request.downloadHandler.data);
            }
            else
            {
                Debug.LogError($"Erreur lors du téléchargement du fichier : {url} " + request.error);
                callback?.Invoke(null);
            }
        }
    }
    IEnumerator ReadPlayerData(string filePath, Action<PlayerData> callback)
    {
        int count = 0;
        string jsonContent = null;
        while (jsonContent == null && count < 5)
        {
            yield return StartCoroutine(DownloadFile(filePath, (responseData) =>
            {
                if (responseData != null)
                    jsonContent = System.Text.Encoding.UTF8.GetString(responseData);
            }));
            count++;
        }


            // Désérialiser le JSON en objet SerializablePlayerData
            SerializablePlayerData serializedData = JsonUtility.FromJson<SerializablePlayerData>(jsonContent);

            // Copier les données depuis SerializablePlayerData vers PlayerData
            PlayerData playerData = me.AddComponent<PlayerData>();

            playerData.playerName = serializedData.playerName;
            playerData.playerAge = serializedData.playerAge;
            playerData.playerRace = serializedData.playerRace;
            playerData.playerClass = serializedData.playerClass;
            playerData.playerAdvantage = serializedData.playerAdvantage;
            playerData.playerDisadvantage = serializedData.playerDisadvantage;
            playerData.playerDescription = serializedData.playerDescription;
            playerData.playerResume = serializedData.playerResume;
            playerData.playerArme = serializedData.playerArme;
            playerData.playerSprite = serializedData.playerSprite;
            playerData.playerInventaire = serializedData.playerInventaire;
            playerData.playerHealth = serializedData.playerHealth;
            playerData.playerHealthNbr = serializedData.playerHealthNbr;
            playerData.playerActions = serializedData.playerActions;
            playerData.playerTags = serializedData.playerTags;
            playerData.playerVision = serializedData.playerVision;
            try { playerData.playerArmePuissance = int.Parse(serializedData.playerArmePuissance); } catch { playerData.playerArmePuissance = 1; }
            callback?.Invoke(playerData);

    }
    public void PointerEnter()
    {

        m_UI_SelectPerso.m_isTrigger = true;
        m_UI_SelectPerso.ManageInputFields();
        SetInfoUI();
    }
    public void SetInfoUI()
    {

        m_UI_SelectPerso.SetInfoUI(_name, _age, _race, _class, _atouts, _handicaps, _description, _arme);
    }
    public void PointerExit()
    {
        m_UI_SelectPerso.ResetInfoToCharacterSelected();
    }
    public void OnClick()
    {
        if(m_Cadre.color != Color.white)
        {
            return;
        }
        m_UI_SelectPerso.m_lastCreationInputField = null;
        m_UI_SelectPerso.changeCurrentCharacterSelection(this);
        switch (m_UI_SelectPerso.m_CurrentCharacter)
        {
            case 1:
                m_Cadre.color = Color.red;
                break;
            case 2:
                m_Cadre.color = Color.green;
                break;
            case 3:
                m_Cadre.color = Color.blue;
                break;
            case 4:
                m_Cadre.color = Color.yellow;
                break;
            default:
                break;
        }
    }
    #endregion
}
