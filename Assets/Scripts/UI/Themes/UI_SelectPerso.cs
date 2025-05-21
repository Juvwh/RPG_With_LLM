
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

using static Jobs;
using static AgentManager;
using System;

using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine.Networking;
using DG.Tweening.Plugins.Core.PathCore;
using Path = System.IO.Path;
using System.Linq;
using Unity.VisualScripting;
using System.Diagnostics;
public class UI_SelectPerso : MonoBehaviour
{
    #region Variables
    public CharacterGridManager CharacterGridManager;
    public GameManager _gameManager;
    public CharactersManagers _charactersManagers;
    public GameObject _charactersManagerObject;
    public Image m_BackgroundTheme;
    public GameObject m_CanvasCharacterSelection;
    public GameObject m_CanvasEditCharacters;
    public GameObject m_CanvasThemeSelection;
    public GameObject m_GenererButton;
    public GameObject m_EnregistrerButton;
    public GameObject m_Canvas_SelectectionPersonnageCarres;
    public GameObject m_CloseButton;
    public GameObject m_LoadingAnimation;
    public GameObject m_LoadingCanvas;
    public GameObject m_ThemeBackground;
    public GameObject m_CanvasChoixLLM;
    public bool _llmIsSelected = false;
    public bool _allIsCharged = false;
    public GameObject m_LastCanvasOpened;
    public GameObject m_ButtonChangeIA;
    public GameObject m_ButtonConfirmerLLm;
    public TMP_Dropdown _tpm;


    [Header("Personnages")]
    public int m_CurrentCharacter = 1;
    public SelectedCharacter[] m_SelectedCharacters = new SelectedCharacter[4];
    public SelectedCharacter _CurrentSelectedCharacter;

    [Header("Informations")]
    public TMP_InputField m_InputName;
    public TMP_InputField m_InputAge;
    public TMP_InputField m_InputRace;
    public TMP_InputField m_InputClass;
    public TMP_InputField m_InputAtouts;
    public TMP_InputField m_InputHandicaps;
    public TMP_InputField m_InputDescription;
    public TextMeshProUGUI m_ArmeText;
    public InputFieldPersonnages m_lastCreationInputField;

    [Header("Personnage selectionner")]
    public CharacterSelection m_currentCharacterSelection_P1;
    public CharacterSelection m_currentCharacterSelection_P2;
    public CharacterSelection m_currentCharacterSelection_P3;
    public CharacterSelection m_currentCharacterSelection_P4;
    public CharacterSelection m_firstCharacterSelection;
    public bool m_isTrigger = false;

    [Header("Nouveau Personnage")]
    public int m_NombrePersonnages = 0;

    [Header("Create Theme")]
    public GameObject m_CanvasCreateTheme;
    public GameObject m_ButtonRetour;
    public GameObject m_ButtonContinuer;
    public GameObject m_LoadingCreateTheme;
    public GameObject m_FrameInputCreateTheme;
    public TMP_InputField m_InputCreateTheme;
    public string m_CreatedTheme;
    public string m_CreatedTheme_Before;
    public GameObject m_Incoherence;
    public TextMeshProUGUI m_IncoherenceText;

    private string savedTexturesFolder = "Background/UI_Background_";
    private string streamingAssetWeb = "https://rpgwithllm.z28.web.core.windows.net/";
    public enum Theme
    {
        StarWars,
        HarryPotter,
        OnePiece,
        SeigneurDesAnneaux,
        CodeLyoko,
        Pokemon,
        StrangerThings,
        SpongeBob,
        GameOfThrone,
        Marvel,
        DungeonsAndDragons,
        Fantasy,
        Cyberpunk,
        MyChoice,
        Default
    }
    public Theme m_CurrentTheme = Theme.Default;
    #endregion
    #region Methods
    private void Start()
    {
        m_LoadingCanvas.SetActive(true);
        m_ThemeBackground.SetActive(false);
        m_CanvasThemeSelection.SetActive(false);
        m_CanvasChoixLLM.SetActive(true);
        StartCoroutine(SwitchTheme(Theme.Default));
    }
    public IEnumerator SwitchTheme(UI_SelectPerso.Theme theme)
    {
        m_CurrentTheme = theme;

        // Construire l'URL complète
        string filePath = _gameManager.FixWebGLPath(Path.Combine(Application.streamingAssetsPath, savedTexturesFolder + m_CurrentTheme.ToString() + ".png"));
        byte[] fileData = null;
        while(fileData == null)
        {
            yield return StartCoroutine(_gameManager.DownloadFile(filePath, (responsedata) => { fileData = responsedata; }));
        }
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);
        m_BackgroundTheme.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Load predefined characters based on the selected theme
        switch (theme)
        {
            case Theme.StarWars:
                LoadPredefinedCharacters(16, 15);
                break;
            case Theme.HarryPotter:
                LoadPredefinedCharacters(7, 8, 9);
                break;
            case Theme.OnePiece:
                LoadPredefinedCharacters(40, 41, 42, 43);
                break;
            case Theme.SeigneurDesAnneaux:
                LoadPredefinedCharacters(21, 22, 23, 24);
                break;
            case Theme.CodeLyoko:
                LoadPredefinedCharacters(25, 26, 27, 28);
                break;
            case Theme.Pokemon:
                LoadPredefinedCharacters(17, 18, 19, 20);
                break;
            case Theme.StrangerThings:
                LoadPredefinedCharacters(11, 12, 13, 14);
                break;
            case Theme.SpongeBob:
                LoadPredefinedCharacters(32, 33, 34, 35);
                break;
            case Theme.GameOfThrone:
                LoadPredefinedCharacters(29, 30, 31);
                break;
            case Theme.Marvel:
                LoadPredefinedCharacters(36, 37, 38, 39);
                break;
            case Theme.DungeonsAndDragons:
                LoadPredefinedCharacters(1, 2, 3, 4);
                break;
            case Theme.Cyberpunk:
                LoadPredefinedCharacters(-1);
                break;
            case Theme.Fantasy:
            case Theme.MyChoice:
            case Theme.Default:
            default:
                break;
        }
    }
    public void Afficher_CharacterSelection()
    {
        m_LastCanvasOpened = m_CanvasCharacterSelection;
        m_CanvasCharacterSelection.SetActive(true);
        m_CanvasCreateTheme.SetActive(false);
        m_CanvasEditCharacters.SetActive(false);
        m_CanvasThemeSelection.SetActive(false);
    }
    public void Afficher_EditCharacters()
    {
        m_LastCanvasOpened = m_CanvasEditCharacters;
        m_CanvasCharacterSelection.SetActive(true);
        m_CanvasEditCharacters.SetActive(true);
        m_CanvasCreateTheme.SetActive(false);
        m_CanvasThemeSelection.SetActive(false);
        _CurrentSelectedCharacter = m_SelectedCharacters[m_CurrentCharacter - 1];
    }
    public void Afficher_ThemeSelection()
    {
        RemoveAllCharacter();
        m_LastCanvasOpened = m_CanvasThemeSelection;
        m_CanvasCharacterSelection.SetActive(false);
        m_CanvasCreateTheme.SetActive(false);
        m_CanvasEditCharacters.SetActive(false);
        m_CanvasThemeSelection.SetActive(true);
    }
    public void Afficher_CreateTheme()
    {
        m_CanvasCreateTheme.SetActive(true);
        m_CanvasEditCharacters.SetActive(false);
        m_CanvasThemeSelection.SetActive(false);
        m_CanvasCharacterSelection.SetActive(false);
        m_LastCanvasOpened = m_CanvasCreateTheme;

        if (m_CreatedTheme != null)
        {
            m_InputCreateTheme.text = m_CreatedTheme;
        }
    }
    public Theme StringToTheme(string theme)
    {
        switch (theme)
        {
            case "StarWars":
                return Theme.StarWars;
            case "HarryPotter":
                return Theme.HarryPotter;
            case "OnePiece":
                return Theme.OnePiece;
            case "SeigneurDesAnneaux":
                return Theme.SeigneurDesAnneaux;
            case "CodeLyoko":
                return Theme.CodeLyoko;
            case "Pokemon":
                return Theme.Pokemon;
            case "StrangerThings":
                return Theme.StrangerThings;
            case "SpongeBob":
                return Theme.SpongeBob;
            case "GameOfThrone":
                return Theme.GameOfThrone;
            case "Marvel":
                return Theme.Marvel;
            case "DungeonsAndDragons":
                return Theme.DungeonsAndDragons;
            case "Fantasy":
                return Theme.Fantasy;
            case "Cyberpunk":
                return Theme.Cyberpunk;
            case "MyChoice":
                return Theme.MyChoice;
            case "Default":
                return Theme.Default;
            default:
                return Theme.Default;
        }
    }
    IEnumerator LoadTexture(string filePath, Action<Texture2D> callback)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(filePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                callback?.Invoke(texture); // Exécute le callback avec la texture chargée
            }
            else
            {
                callback?.Invoke(null); // Envoie `null` en cas d'erreur
            }
        }
    }
    public void SetInfoUI(string name, string age, string race, string classe, string atouts, string handicaps, string description, string arme)
    {
        m_InputName.text = name;
        m_InputAge.text = age;
        m_InputRace.text = race;
        m_InputClass.text = classe;
        m_InputAtouts.text = atouts;
        m_InputHandicaps.text = handicaps;
        m_InputDescription.text = description;
        m_ArmeText.text = arme;
    }
    public void ResetInfoToCharacterSelected()
    {
        m_isTrigger = false;

        switch (m_CurrentCharacter)
        {
            case 1:
                if (m_currentCharacterSelection_P1 != null)
                {
                    m_currentCharacterSelection_P1.SetInfoUI();
                }
                else
                {
                    SetInputField();
                }
                break;
            case 2:
                if (m_currentCharacterSelection_P2 != null)
                {
                    m_currentCharacterSelection_P2.SetInfoUI();
                }
                else
                {
                    SetInputField();
                }
                break;
            case 3:
                if (m_currentCharacterSelection_P3 != null)
                {
                    m_currentCharacterSelection_P3.SetInfoUI();
                }
                else
                {
                    SetInputField();
                }
                break;
            case 4:
                if (m_currentCharacterSelection_P4 != null)
                {
                    m_currentCharacterSelection_P4.SetInfoUI();
                }
                else
                {
                    SetInputField();
                }
                break;
            default:
                break;
        }

    }
    public void changeCurrentCharacterSelection(CharacterSelection Cs)
    {
        m_GenererButton.SetActive(false);
        //On va regarder le numéro du personnage : 
        // Trouver l'index du caractère '_'
        if(Cs != null)
        {
            int index = Cs._NameImage.IndexOf('_');

            // Extraire la sous-chaîne après '_'
            string numberString = Cs._NameImage.Substring(index + 1);

            // Convertir la sous-chaîne en un entier
            int number = int.Parse(numberString);
            m_EnregistrerButton.SetActive(number > 45);
        }


        if (m_CurrentCharacter == 1)
        {
            if (m_currentCharacterSelection_P1 == null)
            {
                SetInfoUI("", "", "", "", "", "", "", "");
            }
            else
            {
                m_currentCharacterSelection_P1.m_Cadre.color = Color.white;
            }
            m_currentCharacterSelection_P1 = Cs;
        }
        else if (m_CurrentCharacter == 2)
        {
            if (m_currentCharacterSelection_P2 == null)
            {
                SetInfoUI("", "", "", "", "", "", "", "");
            }
            else
            {
                m_currentCharacterSelection_P2.m_Cadre.color = Color.white;
            }
            m_currentCharacterSelection_P2 = Cs;
        }
        else if (m_CurrentCharacter == 3)
        {
            if (m_currentCharacterSelection_P3 == null)
            {
                SetInfoUI("", "", "", "", "", "", "", "");
            }
            else
            {
                m_currentCharacterSelection_P3.m_Cadre.color = Color.white;
            }
            m_currentCharacterSelection_P3 = Cs;
        }
        else if (m_CurrentCharacter == 4)
        {
            if (m_currentCharacterSelection_P4 == null)
            {
                SetInfoUI("", "", "", "", "", "", "", "");
            }
            else
            {
                m_currentCharacterSelection_P4.m_Cadre.color = Color.white;
            }
            m_currentCharacterSelection_P4 = Cs;
        }

        if (Cs == null)
        {
            _CurrentSelectedCharacter.RemoveSelectedCharacter();
        }
        else
        {
            _CurrentSelectedCharacter.SetSelectedCharacter(Cs.m_PlayerData, Cs.m_ImagePlayer);

        }

    }
    public void CreateNewCharacter()
    {
        SetInfoUI("", "", "", "", "", "", "", "");
        m_lastCreationInputField = null;
        changeCurrentCharacterSelection(null);
        m_GenererButton.SetActive(true);
        m_EnregistrerButton.SetActive(false);
    }
    public void LoadCharacterSelected()
    {
        CharacterSelection ccs = m_currentCharacterSelection_P1;
        switch (m_CurrentCharacter)
        {
            case 1:
                ccs = m_currentCharacterSelection_P1;
                break;
            case 2:
                ccs = m_currentCharacterSelection_P2;
                break;
            case 3:
                ccs = m_currentCharacterSelection_P3;
                break;
            case 4:
                ccs = m_currentCharacterSelection_P4;
                break;
            default:
                break;
        }

        if (ccs == null)
        {
            SetInfoUI("", "", "", "", "", "", "", "");
        }
        else
        {
            SetInfoUI(ccs._name, ccs._age, ccs._race, ccs._class, ccs._atouts, ccs._handicaps, ccs._description, ccs._arme);
        }

    }
    public void SaveCharacter()
    {
        StartCoroutine(SaveCharacter_Coroutine());
    }
    public IEnumerator SaveCharacter_Coroutine()
    {
        // Sélectionner le personnage courant
        CharacterSelection cs = m_CurrentCharacter switch
        {
            1 => m_currentCharacterSelection_P1,
            2 => m_currentCharacterSelection_P2,
            3 => m_currentCharacterSelection_P3,
            4 => m_currentCharacterSelection_P4,
            _ => m_currentCharacterSelection_P1
        };



        // Vérifier si des modifications ont été effectuées
        if (m_InputName.text == cs.m_PlayerData.playerName &&
            m_InputAge.text == cs.m_PlayerData.playerAge &&
            m_InputRace.text == cs.m_PlayerData.playerRace &&
            m_InputClass.text == cs.m_PlayerData.playerClass &&
            m_InputAtouts.text == cs.m_PlayerData.playerAdvantage &&
            m_InputHandicaps.text == cs.m_PlayerData.playerDisadvantage &&
            m_InputDescription.text == cs.m_PlayerData.playerDescription)
        {
            _CurrentSelectedCharacter.SetSelectedCharacter(cs.m_PlayerData, cs.m_ImagePlayer);
            SetInteractable(true);
            Afficher_CharacterSelection();
            yield break;
        }

        // Désactiver les interactions pendant la sauvegarde
        m_EnregistrerButton.SetActive(false);
        SetInteractable(false);

        // Vérifier si une nouvelle image est nécessaire
        bool generateNewImage = m_InputDescription.text != cs.m_PlayerData.playerDescription;

            // Appeler l'API pour générer de nouvelles informations
            string[] _response = null;
            yield return StartCoroutine(GenererInformationsPlayers((responseStringTab) => { _response = responseStringTab; }));
        try
        {

            // Créer un objet SerializablePlayerData avec les nouvelles données
            SerializablePlayerData serializedData = new SerializablePlayerData
            {
                playerName = _response[0],
                playerAge = _response[1],
                playerRace = _response[2],
                playerClass = _response[3],
                playerAdvantage = _response[4],
                playerDisadvantage = _response[5],
                playerDescription = _response[6],
                playerResume = _response[7],
                playerArme = _response[8],
                playerArmePuissance = _response[9],
                playerSprite = cs.m_PlayerData.playerSprite,
                playerInventaire = cs.m_PlayerData.playerInventaire,
                playerPosition = cs.m_PlayerData.playerPosition,
                playerHealth = cs.m_PlayerData.playerHealth,
                playerHealthNbr = cs.m_PlayerData.playerHealthNbr,
                playerActions = cs.m_PlayerData.playerActions,
                playerTags = cs.m_PlayerData.playerTags
            };

            string folderPath = _gameManager.FixWebGLPath(Path.Combine(Application.streamingAssetsPath, "SavedPlayerInfo"));
            string path = Path.Combine(folderPath, cs._NameImage + ".json");
            string jsonContent = JsonUtility.ToJson(serializedData, true);
            //Si on n'est pas sur Azur, on utilise le path dynamique
            if (!_gameManager.m_GenerateLogOnAzur)
            {
                // Chemin dynamique pour sauvegarder les fichiers JSON
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Sauvegarder les données au format JSON
                File.WriteAllText(path, jsonContent);
            }
            else
            {
                //Sauvegarder les données au format JSON sur Azur
                _gameManager._azurManager.UploadJSON(cs._NameImage + ".json", jsonContent);
            }
        }

        catch (Exception ex)
        {
            //Debug.LogError($"Erreur lors de la sauvegarde du personnage : {ex.Message}");
        }
        // Générer et mettre à jour l'image si nécessaire
        if (generateNewImage)
            {
                SetInfoUI(_response[0], _response[1], _response[2], _response[3], _response[4], _response[5], _response[6], _response[8]);
                Texture2D texture = null;
                yield return StartCoroutine(GenererImagePlayer((response) => { texture = response; }));
                UpdateImage(texture, cs);
            }

        // Recharger les données pour ce personnage
        cs.LoadPlayerData((response) => { bool tf = response; });
        // Réactiver les interactions
        SetInteractable(true);
        m_EnregistrerButton.SetActive(true);
        _CurrentSelectedCharacter.SetSelectedCharacter(cs.m_PlayerData, cs.m_ImagePlayer);
        Afficher_CharacterSelection();
        
    }
    private string[] Parser_Infos(string _input)
    {
        _input = Parser.CLEAN_JSON(_input);
        try
        {
            // Parse le JSON
            JObject data = JObject.Parse(_input);

            // Vérifie si les champs attendus sont bien présents
            if (data["nom"] == null || data["age"] == null || data["race"] == null || data["classe"] == null || data["atout"] == null
                 || data["handicap"] == null || data["physique"] == null || data["summary"] == null || data["arme"] == null || data["puissance"] == null)
            {
                return new string[0];
            }
            string[] toReturn = new string[10];
            toReturn[0] = data["nom"].ToString();
            toReturn[1] = data["age"].ToString();
            toReturn[2] = data["race"].ToString();
            toReturn[3] = data["classe"].ToString();
            toReturn[4] = data["atout"].ToString();
            toReturn[5] = data["handicap"].ToString();
            toReturn[6] = data["physique"].ToString();
            toReturn[7] = data["summary"].ToString();
            toReturn[8] = data["arme"].ToString();
            toReturn[9] = data["puissance"].ToString();

            return toReturn; // Parsing réussi
        }
        catch (Exception e)
        {
            return new string[0];
        }

    }
    private string ExtractText(string txt, int startIndex, int endIndex)
    {
        return (startIndex < endIndex && startIndex >= 0 && endIndex <= txt.Length)
            ? txt.Substring(startIndex, endIndex - startIndex).Trim()
            : string.Empty;
    }
    public IEnumerator GenererInformationsPlayers(Action<string[]>callback)
    {
        m_lastCreationInputField = null;
        string[] _responses = new string[8];
        bool _ok = false;

        var value = new Dictionary<string, string>
        {
            {"nom", m_InputName.text },
            {"age",m_InputAge.text  },
            {"race", m_InputRace.text },
            {"classe", m_InputClass.text },
            {"atout", m_InputAtouts.text },
            {"handicap", m_InputHandicaps.text },
            {"description", m_InputDescription.text }
        };
        LocalizationManager _language = FindFirstObjectByType<LocalizationManager>();

        while (!_ok)
        {
            string prompt = _language.GetPrompt("_generation_hero", value);

            string _reponse = "";
            yield return StartCoroutine(_gameManager._agent.Generate_Text(_gameManager.m_currentModel, Job.GenererHero, prompt, (_response) => { _reponse = _response; }));
            _responses = Parser_Infos(_reponse);
            _ok = _responses.Length == 10;
            if(!_ok)
            {
                yield return new WaitForSeconds(1);
            }

        }
        callback?.Invoke(_responses);


    }
    public IEnumerator GenererImagePlayer(Action<Texture2D> callback)
    {
        string imageprompt = "Error";
        while (imageprompt.Contains("Error") || imageprompt.Contains("Erreur"))
        {
            yield return StartCoroutine( _gameManager._agent.Generate_Text(_gameManager.m_currentModel, Job.GenererDescriptionImageHero, m_InputDescription.text, (response) => { imageprompt = response; }));
            if (imageprompt.Contains("Error") || imageprompt.Contains("Erreur"))
            {
                yield return new WaitForSeconds (1);
            }
        }
        Texture2D texture = null;
        yield return StartCoroutine( _gameManager.Generate_Image(GameManager.LLM_image.BlackForestFast, $"{imageprompt}, RPG style, Ultra Detailled.", (response) => { texture = response; }));
        callback?.Invoke(texture);
    }
    public void UpdateImage(Texture2D newTexture, CharacterSelection cs)
    {
        if (newTexture == null || string.IsNullOrEmpty(cs._NameImage))
        {
            return;
        }

        // Créer le chemin du dossier "SavedTextures" dans persistentDataPath
        string texturesFolder = _gameManager.FixWebGLPath(Path.Combine(Application.streamingAssetsPath, "SavedTextures"));


        // Chemin complet pour l'image
        string imagePath = Path.Combine(texturesFolder, cs._NameImage + ".png");

        // Convertir la texture en PNG
        byte[] imageBytes = newTexture.EncodeToPNG();
        if(!_gameManager.m_GenerateLogOnAzur)
        {

        
            // Vérifier si le dossier existe, sinon le créer
            if (!Directory.Exists(texturesFolder))
            {
                Directory.CreateDirectory(texturesFolder);
            }

            // Écraser ou créer l'image existante
            try
            {
                File.WriteAllBytes(imagePath, imageBytes);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        else
        {
            // Envoyer l'image sur Azur
            _gameManager._azurManager.UploadPNG(cs._NameImage + ".png", imageBytes);
        }

        // Créer un Sprite à partir de la texture
        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(0, 0, newTexture.width, newTexture.height),
            new Vector2(0.5f, 0.5f)
        );

        // Affecter le Sprite à l'image UI
        cs.m_ImagePlayer.sprite = newSprite;

    }
    public void SetInteractable(bool tf)
    {
        m_LoadingAnimation.SetActive(!tf);
        m_Canvas_SelectectionPersonnageCarres.SetActive(tf);
        m_CloseButton.SetActive(tf);
        m_EnregistrerButton.SetActive(tf);
        m_InputName.interactable = tf;
        m_InputAge.interactable = tf;
        m_InputRace.interactable = tf;
        m_InputClass.interactable = tf;
        m_InputAtouts.interactable = tf;
        m_InputHandicaps.interactable = tf;
        m_InputDescription.interactable = tf;
        m_GenererButton.SetActive(false);
    }
    public void RemoveCharacter(int character)
    {
        m_CurrentCharacter = character;


        switch (character)
        {
            case 1:
                _CurrentSelectedCharacter = m_SelectedCharacters[0];
                m_currentCharacterSelection_P1.m_Cadre.color = Color.white;
                m_currentCharacterSelection_P1 = null;
                break;
            case 2:
                _CurrentSelectedCharacter = m_SelectedCharacters[1];
                m_currentCharacterSelection_P2.m_Cadre.color = Color.white;
                m_currentCharacterSelection_P2 = null;
                break;
            case 3:
                _CurrentSelectedCharacter = m_SelectedCharacters[2];
                m_currentCharacterSelection_P3.m_Cadre.color = Color.white;
                m_currentCharacterSelection_P3 = null;
                break;
            case 4:
                _CurrentSelectedCharacter = m_SelectedCharacters[3];
                m_currentCharacterSelection_P4.m_Cadre.color = Color.white;
                m_currentCharacterSelection_P4 = null;
                break;
            default:
                break;
        }

        changeCurrentCharacterSelection(null);

    }
    public void GenerateNewPersonnage()
    {
        StartCoroutine(GenerateNewPersonnage_Coroutine());
    }
    public IEnumerator GenerateNewPersonnage_Coroutine()
    {
        SetInteractable(false);

        // Créer les chemins des fichiers JSON et PNG
        string playerInfoFolder = _gameManager.FixWebGLPath(Path.Combine(Application.streamingAssetsPath, "SavedPlayerInfo")); // modif persistentDataPath
        string texturesFolder = _gameManager.FixWebGLPath(Path.Combine(Application.streamingAssetsPath, "SavedTextures"));


        string jsonPath = Path.Combine(playerInfoFolder, $"Hero_{m_NombrePersonnages}.json");

        // Générer les informations du personnage
        string[] _response = null;
        yield return StartCoroutine(GenererInformationsPlayers((response) => { _response = response; }));

        // Créer un objet SerializablePlayerData et copier les données de m_PlayerData
        SerializablePlayerData serializedData = new SerializablePlayerData
        {
            playerName = _response[0],
            playerAge = _response[1],
            playerRace = _response[2],
            playerClass = _response[3],
            playerAdvantage = _response[4],
            playerDisadvantage = _response[5],
            playerDescription = _response[6],
            playerResume = _response[7],
            playerArme = _response[8],
            playerArmePuissance = _response[9]
        };

        // update UI
        m_InputName.text = _response[0];
        m_InputAge.text = _response[1];
        m_InputRace.text = _response[2];
        m_InputClass.text = _response[3];
        m_InputAtouts.text = _response[4];
        m_InputHandicaps.text = _response[5];
        m_InputDescription.text = _response[6];
        m_ArmeText.text = _response[8];

        // Convertir serializedData en JSON
        string jsonContent = JsonUtility.ToJson(serializedData, true);
        if (!_gameManager.m_GenerateLogOnAzur) { 
            // Créer les dossiers s'ils n'existent pas
            if (!Directory.Exists(playerInfoFolder)) Directory.CreateDirectory(playerInfoFolder);
            if (!Directory.Exists(texturesFolder)) Directory.CreateDirectory(texturesFolder);
            // Écrire le JSON dans le fichier
            File.WriteAllText(jsonPath, jsonContent);
        }
        else
        {
            //Sauvegarder les données au format JSON sur Azur
            _gameManager._azurManager.UploadJSON($"Hero_{m_NombrePersonnages}" + ".json", jsonContent);
        }
        Texture2D texture = null;
        // Chemin pour l'image PNG
        string texturePath = Path.Combine(texturesFolder, $"Hero_{m_NombrePersonnages}.png");

        // Générer l'image du joueur
        yield return StartCoroutine(GenererImagePlayer((responsetexture) => { texture = responsetexture; }));

        // Encoder la texture en PNG
        byte[] pngData = texture.EncodeToPNG();

        if (!_gameManager.m_GenerateLogOnAzur)
        {
            if (pngData != null)
            {
                // Enregistrer le fichier en local
                File.WriteAllBytes(texturePath, pngData);
            }
        }
        else
        {
            // Envoyer l'image sur Azur
            _gameManager._azurManager.UploadPNG($"Hero_{m_NombrePersonnages}.png", pngData);
        }

        // Ajouter l'image au CharacterGridManager
        CharacterGridManager.AddImageCharacter(texture, $"Hero_{m_NombrePersonnages}", serializedData);
        m_NombrePersonnages++;

        SetInteractable(true);
        Afficher_CharacterSelection();
    }
    public void LoadPredefinedCharacters(int Player1, int Player2 = -1, int Player3 = -1, int Player4 = -1)
    {
        if (Player1 != -1)
        {
            m_CurrentCharacter = 1;
            _CurrentSelectedCharacter = m_SelectedCharacters[0];
            CharacterGridManager.m_AllCharacterSelection[Player1].OnClick();
        }
        if (Player2 != -1)
        {
            _CurrentSelectedCharacter = m_SelectedCharacters[1];
            m_CurrentCharacter = 2;
            CharacterGridManager.m_AllCharacterSelection[Player2].OnClick();
        }
        if (Player3 != -1)
        {
            _CurrentSelectedCharacter = m_SelectedCharacters[2];
            m_CurrentCharacter = 3;
            CharacterGridManager.m_AllCharacterSelection[Player3].OnClick();
        }
        if (Player4 != -1)
        {
            _CurrentSelectedCharacter = m_SelectedCharacters[3];
            m_CurrentCharacter = 4;
            CharacterGridManager.m_AllCharacterSelection[Player4].OnClick();
        }

    }
    public void RemoveAllCharacter()
    {
        if (m_currentCharacterSelection_P1 != null)
        {
            RemoveCharacter(1);
        }
        if (m_currentCharacterSelection_P2 != null)
        {
            RemoveCharacter(2);
        }
        if (m_currentCharacterSelection_P3 != null)
        {
            RemoveCharacter(3);
        }
        if (m_currentCharacterSelection_P4 != null)
        {
            RemoveCharacter(4);
        }

    }
    public void Commencer()
    {
        List<CharacterSelection> validCharacterSelections = new List<CharacterSelection>();


        CharacterSelection[] allSelections = new[]
        {
            m_currentCharacterSelection_P1,
            m_currentCharacterSelection_P2,
            m_currentCharacterSelection_P3,
            m_currentCharacterSelection_P4
        };

        CharacterSelection[] characterSelections = allSelections
            .Where(selection => selection != null)
            .ToArray();


            for (int j = 0; j < characterSelections.Length; j++)
            {
                if (characterSelections[j] != null)
                {
                    PlayerData newPlayerData = _charactersManagerObject.AddComponent<PlayerData>();
                    newPlayerData.playerName = characterSelections[j].m_PlayerData.playerName;
                    newPlayerData.playerAge = characterSelections[j].m_PlayerData.playerAge;
                    newPlayerData.playerRace = characterSelections[j].m_PlayerData.playerRace;
                    newPlayerData.playerClass = characterSelections[j].m_PlayerData.playerClass;
                    newPlayerData.playerAdvantage = characterSelections[j].m_PlayerData.playerAdvantage;
                    newPlayerData.playerDisadvantage = characterSelections[j].m_PlayerData.playerDisadvantage;
                    newPlayerData.playerDescription = characterSelections[j].m_PlayerData.playerDescription;
                    newPlayerData.playerResume = characterSelections[j].m_PlayerData.playerResume;
                    newPlayerData.playerSprite = characterSelections[j].m_ImagePlayer.sprite;
                    newPlayerData.playerArme = characterSelections[j].m_PlayerData.playerArme;
                    newPlayerData.playerArmePuissance = characterSelections[j].m_PlayerData.playerArmePuissance;

                    newPlayerData.playerInventaire = new List<string>(); // Cloner la liste
                    newPlayerData.playerVision = characterSelections[j].m_PlayerData.playerVision;
                    newPlayerData.playerHealth = "";
                    newPlayerData.playerHealthNbr = 100;
                    newPlayerData.playerActions = new List<string>(); // Cloner la liste
                    newPlayerData.playerTags = new List<Tags.Tag>(); // Cloner la liste

                    _charactersManagers.SetCharacters(newPlayerData, j);

                }            
        }
        _charactersManagers.RemoveNullCharacters();

        if(m_CurrentTheme == Theme.MyChoice)
        {
            _charactersManagers.m_CurrentTheme = m_CreatedTheme;
        }
        else
        {
            _charactersManagers.m_CurrentTheme = m_CurrentTheme.ToString();
        }
        SceneManager.LoadScene("2Launch");
    }
    public void ManageInputFields()
    {
        if ((m_CurrentCharacter == 1 && m_currentCharacterSelection_P1 == null) || (m_CurrentCharacter == 2 && m_currentCharacterSelection_P2 == null)
            || (m_CurrentCharacter == 3 && m_currentCharacterSelection_P3 == null) || (m_CurrentCharacter == 4 && m_currentCharacterSelection_P4 == null))
        {
            m_lastCreationInputField = new InputFieldPersonnages(m_InputName.text, m_InputAge.text, m_InputRace.text, m_InputClass.text, m_InputAtouts.text, m_InputHandicaps.text, m_InputDescription.text, m_ArmeText.text);
        }
    }
    public void SetInputField()
    {
        if (m_lastCreationInputField != null)
        {
            SetInfoUI(m_lastCreationInputField.m_Name, m_lastCreationInputField.m_Age, m_lastCreationInputField.m_Race, m_lastCreationInputField.m_Class, m_lastCreationInputField.m_Atouts, m_lastCreationInputField.m_Handicaps, m_lastCreationInputField.m_Description, m_lastCreationInputField.m_arme);
        }
        else
        {
            SetInfoUI("", "", "", "", "", "", "", "");
        }

    }
    public IEnumerator Verify_Coherence_CreateTheme()
    {

        if(m_CreatedTheme_Before == m_InputCreateTheme.text)
        {
            string path = _gameManager.FixWebGLPath(System.IO.Path.Combine(Application.streamingAssetsPath, savedTexturesFolder + "MyChoice.png"));
            Texture2D texture = null;
            yield return StartCoroutine(LoadTexture(path, (response2d) => { texture = response2d; }));
            Image imageComponent = m_BackgroundTheme.GetComponent<Image>();
            imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Afficher_CharacterSelection();
            yield break;
        }

        m_ButtonRetour.SetActive(false);
        m_ButtonContinuer.SetActive(false);
        m_LoadingCreateTheme.SetActive(true);
        m_FrameInputCreateTheme.SetActive(false);
        m_Incoherence.SetActive(false);

        string _theme = "Erreur";
        bool _coherence = false;
        string _responseTheme = "Error";
        while (_theme == "Erreur")
        {
            yield return StartCoroutine(_gameManager._agent.Generate_Text(_gameManager.m_currentModel, Job.CheckCoherenceTheme, m_InputCreateTheme.text, (response)=> { _responseTheme = response; }));
            _coherence = Parser.THEME(_responseTheme, out _theme);
            if(_theme == "Erreur")
            {
                yield return new WaitForSeconds(1);
            }
        }

        if( _coherence)
        {
            m_CreatedTheme = _theme;
            m_CreatedTheme_Before = _theme;
            m_ButtonRetour.SetActive(true);
            m_ButtonContinuer.SetActive(true);
            m_LoadingCreateTheme.SetActive(false);
            m_FrameInputCreateTheme.SetActive(true);
            string filePath = _gameManager.FixWebGLPath(System.IO.Path.Combine(Application.streamingAssetsPath, savedTexturesFolder + "MyChoice.png"));
            Texture2D texture = null;
            yield return StartCoroutine(LoadTexture(filePath, (response2d) => { texture = response2d; }));
            Image imageComponent = m_BackgroundTheme.GetComponent<Image>();
            imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            Afficher_CharacterSelection();
        }
        else
        {
            m_ButtonRetour.SetActive(true);
            m_ButtonContinuer.SetActive(true);
            m_LoadingCreateTheme.SetActive(false);
            m_FrameInputCreateTheme.SetActive(true);
            m_Incoherence.SetActive(true);
            m_IncoherenceText.text = _theme;
        }
    }
    public void OnClickConfirmerChoixLLM()
    {
        _llmIsSelected = true;
        m_CanvasChoixLLM.SetActive(false);
        if (m_LastCanvasOpened == null)
        {
            //Regarder si tout a déjà été chargé
            if (_allIsCharged)
            {
                m_CanvasThemeSelection.SetActive(true);
                m_LoadingCanvas.SetActive(false);
                m_ButtonChangeIA.SetActive(true);
            }
            else
            {
                m_LoadingCanvas.SetActive(true);
                m_CanvasThemeSelection.SetActive(false);
            }
            
        }
        else{
            m_LastCanvasOpened.SetActive(true);
            m_ButtonChangeIA.SetActive(true);
        }
    }
    public void OnClickChangeLLM()
    {
        _llmIsSelected = false;
        m_CanvasChoixLLM.SetActive(true);
        m_LastCanvasOpened.SetActive(false);
        m_ButtonChangeIA.SetActive(false);
    }
    public void OnValueChangeDropDownMenu(int i)
    {
        LocalizationManager localizationManager = FindObjectOfType<LocalizationManager>();
        localizationManager.OnValueChangeDropDownMenu(i);
    }
    public void ChangeUIOptionLanguage(int index_language)
    {
        switch (index_language)
        {
            case 0:
                _tpm.value = 0;
                break;
            case 1:
                _tpm.value = 1;
                break;
            default:
                break;
        }
    }
    #endregion
}

public class InputFieldPersonnages
{
    public string m_Name;
    public string m_Age;
    public string m_Race;
    public string m_Class;
    public string m_Atouts;
    public string m_Handicaps;
    public string m_Description;
    public string m_arme;

    public InputFieldPersonnages(string name, string age, string race, string _class, string atouts, string handicaps, string description, string arme)
    {
        m_Name = name;
        m_Age = age;
        m_Race = race;
        m_Class = _class;
        m_Atouts = atouts;
        m_Handicaps = handicaps;
        m_Description = description;
        m_arme = arme;
    }

}
