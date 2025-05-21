using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Unity.VisualScripting;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using System;
using UnityEngine.Networking;

public class Automatic_Launch : MonoBehaviour
{
    public PlayerData[] m_playerData = new PlayerData[4];
    private string savedTexturesFolder = "SavedTextures/";
    public bool Done = false;
    public IEnumerator InitializePlayerData()
    {
        CharactersManagers manager = gameObject.AddComponent<CharactersManagers>();
        manager.m_CurrentTheme = "Donjon et Dragon";
        string[] listHerosToLoad = new string[4] { "Hero_1", "Hero_2", "Hero_3", "Hero_4" };
        string folderPath = FixWebGLPath(Path.Combine(Application.streamingAssetsPath, savedTexturesFolder));
        Texture2D texture = null;
        for (int i = 0; i < 4; i++)
        {
            yield return StartCoroutine(LoadPlayerData(listHerosToLoad[i], i));// load json content
            yield return StartCoroutine(LoadTextureFromFile(folderPath + $"Hero_{i+1}.png", (tex) => texture = tex));
            m_playerData[i].playerSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            PlayerData newPlayerData = manager.gameObject.AddComponent<PlayerData>();
            newPlayerData.playerName = m_playerData[i].playerName;
            newPlayerData.playerAge = m_playerData[i].playerAge;
            newPlayerData.playerRace = m_playerData[i].playerRace;
            newPlayerData.playerClass = m_playerData[i].playerClass;
            newPlayerData.playerAdvantage = m_playerData[i].playerAdvantage;
            newPlayerData.playerDisadvantage = m_playerData[i].playerDisadvantage;
            newPlayerData.playerDescription = m_playerData[i].playerDescription;
            newPlayerData.playerResume = m_playerData[i].playerResume;
            newPlayerData.playerSprite = m_playerData[i].playerSprite;
            newPlayerData.playerArme = m_playerData[i].playerArme;
            newPlayerData.playerArmePuissance = m_playerData[i].playerArmePuissance;

            newPlayerData.playerInventaire = new List<string>(); // Cloner la liste
            newPlayerData.playerVision = m_playerData[i].playerVision;
            newPlayerData.playerHealth = "";
            newPlayerData.playerHealthNbr = 25;
            newPlayerData.playerActions = new List<string>(); // Cloner la liste
            newPlayerData.playerTags = new List<Tags.Tag>(); // Cloner la liste

            manager.SetCharacters(newPlayerData, i);
        }
        Done = true;
    
    }
    public IEnumerator LoadTextureFromFile(string filePath, Action<Texture2D> callback)
    {
        // Charger le fichier image en bytes
        byte[] fileData = null;
        yield return StartCoroutine(DownloadFile(filePath, (data) => fileData = data));
        if (fileData == null)
        {
            callback?.Invoke(null);
            yield break;
        }
        // Créer une nouvelle Texture2D et charger les données
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData))
        {
            callback?.Invoke(texture);
            yield break;
        }
        else
        {
            texture = null;
            callback?.Invoke(texture);
        }
    }
    public IEnumerator LoadPlayerData(string spriteName, int i)
    {
        // Construire le chemin complet vers le fichier JSON
        string path = FixWebGLPath(Path.Combine(Application.streamingAssetsPath, "SavedPlayerInfo", spriteName + ".json"));

        PlayerData data = new PlayerData();
        yield return StartCoroutine(ReadPlayerData(path, (playerData) =>
        {
            data = playerData;
        }));
        m_playerData[i] = data;
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
                Debug.LogError("Erreur lors du téléchargement du fichier : " + request.error);
                callback?.Invoke(null);
            }
        }
    }
    IEnumerator ReadPlayerData(string filePath, Action<PlayerData> callback)
    {

        string jsonContent = null;
        while (jsonContent == null)
        {
            yield return StartCoroutine(DownloadFile(filePath, (responseData) =>
            {
                if (responseData != null)
                    jsonContent = System.Text.Encoding.UTF8.GetString(responseData);
            }));
        }


        // Désérialiser le JSON en objet SerializablePlayerData
        SerializablePlayerData serializedData = JsonUtility.FromJson<SerializablePlayerData>(jsonContent);

        // Copier les données depuis SerializablePlayerData vers PlayerData
        PlayerData playerData = new PlayerData();

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
}
