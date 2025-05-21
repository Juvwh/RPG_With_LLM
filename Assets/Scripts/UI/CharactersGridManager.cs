using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CharacterGridManager : MonoBehaviour
{
    // Références publiques pour les objets nécessaires
    public Transform content; // Le Transform de l'objet "Content" dans lequel nous ajoutons les lignes
    public GameObject lineCharactersPrefab; // Le prefab "LineCharacters"
    public GameObject imageCharacterPrefab; // Le prefab "ImageCharacter"
    public UI_SelectPerso m_UI_SelectPerso;
    public bool m_CreateGrid = true;
    public CharacterSelection[] m_AllCharacterSelection;

    // Le dossier contenant les images (ajuste le chemin si nécessaire)
    private string savedTexturesFolder = "SavedTextures/";

    public void Start()
    {
        OnClickLoadImage();
    }
    public void OnClickLoadImage()
    {
        if (m_CreateGrid)
        {
            m_CreateGrid = false;
            StartCoroutine(LoadImagesAndPopulateGrid());
        }
    }

    IEnumerator LoadImagesAndPopulateGrid()
    {
        // Construire le chemin complet pour accéder au dossier dans StreamingAssets
        string folderPath = FixWebGLPath(Path.Combine(Application.streamingAssetsPath, savedTexturesFolder));
        int iteration = 0;
        //récupérer toutes les images du dossier
        Queue<Texture2D> allTextures = new Queue<Texture2D>();
        while (true)
        {
            Texture2D texture = null;
            yield return StartCoroutine(LoadTexture(folderPath+$"Hero_{iteration}.png", (tex) => texture = tex));
            if(texture == null) {  break; }
            allTextures.Enqueue(texture);
            iteration++;
        }
        if(iteration == 0)
        {
            yield break;
        }
        // Récupérer les chemins des fichiers PNG dans le dossier

        m_UI_SelectPerso.m_NombrePersonnages = iteration;
        m_AllCharacterSelection = new CharacterSelection[iteration];
        int count = 0;

        bool done = false;
        for (int i = 0; i < iteration; i += 3)
        {
            // Créer une nouvelle ligne pour les personnages
            GameObject lineObject = Instantiate(lineCharactersPrefab, content);

            for (int j = i; j < i + 3 && j < iteration; j++)
            {
                Texture2D texture = allTextures.Dequeue();

                GameObject imageCharacterObject = Instantiate(imageCharacterPrefab, lineObject.transform);
                ActivateAllParents(imageCharacterObject.transform); // Active tous les parents
                imageCharacterObject.SetActive(true);
                Image imageComponent = imageCharacterObject.transform.GetChild(0).GetComponent<Image>();
                imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                string spriteName = $"Hero_{j}";
                yield return StartCoroutine(imageCharacterObject.GetComponent<CharacterSelection>().Initialise(spriteName, (response) => { done = response; }));
                if (!done)
                {
                    break;
                }
                m_AllCharacterSelection[j] = imageCharacterObject.GetComponent<CharacterSelection>();
                count++;
            }
            if (!done)
            {
                break;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        UI_SelectPerso _ui = FindFirstObjectByType<UI_SelectPerso>();
        yield return new WaitWhile(() => !_ui._llmIsSelected); // Tant que le joueur n'a pas choisi son LLM, on attend.
        _ui.m_LoadingCanvas.SetActive(false);
        _ui.m_CanvasEditCharacters.SetActive(false);
        _ui.m_ThemeBackground.SetActive(true);
        _ui.m_CanvasThemeSelection.SetActive(true);
        _ui.m_LastCanvasOpened = _ui.m_CanvasThemeSelection;
        _ui._allIsCharged = true;
        _ui.m_ButtonChangeIA.SetActive(true);


    }

    void ActivateAllParents(Transform obj, bool tf = true)
    {
        while (obj != null)
        {
            obj.gameObject.SetActive(tf);
            obj = obj.parent; // Passer au parent suivant
        }
    }

    public string FixWebGLPath(string path)
    {

            // Si le chemin commence par "/https:/", retirer le premier "/"
            if (path.StartsWith("/https:/"))
            {
                path = path.Substring(1); // Retire le premier caractère "/"
            }

        return path;
    }

    int ExtractNumberFromFileName(string filePath)
    {
        string fileName = Path.GetFileNameWithoutExtension(filePath);
        string numberPart = fileName.Substring(fileName.LastIndexOf('_') + 1);
        int.TryParse(numberPart, out int number);
        return number;
    }


    IEnumerator LoadTexture(string filePath, Action<Texture2D> callback)
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


    public void AddImageCharacter(Texture2D texture, string spriteName, SerializablePlayerData serializedData)
    {
        if (texture == null) return;

        // Obtenir la dernière ligne de "content"
        Transform lastLine = content.childCount > 0 ? content.GetChild(content.childCount - 1) : null;

        // Créer une nouvelle ligne si la dernière est pleine ou s'il n'y en a pas encore
        if (lastLine == null || lastLine.childCount >= 3)
        {
            lastLine = Instantiate(lineCharactersPrefab, content).transform;
        }

        // Créer un nouvel "ImageCharacter" et l'ajouter à la dernière ligne
        GameObject imageCharacterObject = Instantiate(imageCharacterPrefab, lastLine);

        // Assigner la texture au composant Image de l'objet "ImageCharacter"
        Image imageComponent = imageCharacterObject.transform.GetChild(0).GetComponent<Image>();
        imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        imageCharacterObject.GetComponent<CharacterSelection>().Initialise(spriteName, serializedData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        imageCharacterObject.GetComponent<CharacterSelection>().OnClick();
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
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
                callback?.Invoke(null);
            }
        }
    }

}
