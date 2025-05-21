//Code by Justin Vanwichelen and Gaetan Berlaimont
//2025

using UnityEngine;
using System.IO;
using System.Collections;
using System;
using UnityEngine.Networking;
using static Jobs;
using static Models;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Manages the overall game state and interactions between different managers.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Managers
    [Header("MANAGEMENT")]
    // Managers for different aspects of the game
    public AgentManager _agent;
    public AzureUploader _azurManager;
    public Automatic_Launch _automaticLaunch;
    public ButtonsManager _buttonsManager;
    public CharactersManagers _charactersManagers;
    public Coffre _coffre;
    public CombatManager _combatManager;
    public DiceManager _diceManager;
    public ElevenLabsManager _elevenLabsManager;
    public LocalizationManager _language;
    public Free _free;
    public GroupsManager _groupsManager;
    public HistoryManager _historyManager;
    public HuggingFaceImageManager _huggingfaceImageManager;
    public ImagesManager _imagesManager;
    public MapManager _mapManager;
    public MemoryManager _memoryManager;
    public NextManager _nextManager;
    public UI_Inventaire_Buttons _UI_Inventaire_Buttons;
    public UI_Manager _UI_Manager;
    #endregion

    #region Debug Options
    [Header("Debug")]
    // Debugging options (generator log)
    public bool m_GenerateLogOnAzur = false;
    #endregion

    #region Model Configuration
    [Header("Model Configuration")]
    // Current model being used
    public Model m_currentModel = Model.mistral_3_3_70b;
    #endregion

    #region Image Generation Options
    [Header("Image Generation Options")]
    // Options for image generation
    public bool generateImage;
    public enum LLM_image { BlackForest, StableDiffusion, BlackForestFast, autre }
    public LLM_image llm_Image = LLM_image.BlackForest;
    #endregion

    #region Voice Generation Options
    [Header("Voice Generation Options")]
    // Options for voice generation
    public bool generateVoice;
    public enum LLM_voice { ElevenLabs, autre }
    public LLM_voice llm_Voice = LLM_voice.ElevenLabs;
    private AudioClip CurrentVoice;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes the game managers and sets up the game environment.
    /// </summary>
    public void Start()
    {
        _huggingfaceImageManager = new HuggingFaceImageManager();
        _charactersManagers = FindFirstObjectByType<CharactersManagers>();
        _language = FindFirstObjectByType<LocalizationManager>();
        if (_charactersManagers == null)
        {
            StartCoroutine(_automaticLaunch.InitializePlayerData());
            _charactersManagers = FindFirstObjectByType<CharactersManagers>();
        }
        else
        {
            m_currentModel = _charactersManagers.m_currentModel;
            _agent.ChangeUIOptionLLM();
            if (_automaticLaunch != null)
            {
                _automaticLaunch.Done = true;
            }
        }

        if (!m_GenerateLogOnAzur)
        {
            InitializeCSVFiles();

            string filePath = Path.Combine(Application.streamingAssetsPath, "Errors" + ".txt");
            // Write the response to the file
            File.WriteAllText(filePath, "List of all errors encountered during the game \n \n ----------------------------------------");
        }
    }

    /// <summary>
    /// Finds and sets the character manager in the game.
    /// </summary>
    public void FindCharacterManager()
    {
        _charactersManagers = FindFirstObjectByType<CharactersManagers>();
        _charactersManagers.SetGameManager(this);
    }

    /// <summary>
    /// Allows generating an image by choosing an LLM.
    /// </summary>
    /// <param name="LLM">Choice of LLM for image generation</param>
    /// <param name="texte">The text to send to the LLM</param>
    /// <param name="_callback">Callback function to handle the generated image</param>
    /// <returns>Coroutine for generating the image</returns>
    public IEnumerator Generate_Image(LLM_image LLM, string texte, Action<Texture2D> _callback)
    {
        int count = 0;
        Texture2D _toReturn = null;
        while (count < 5 && _toReturn == null)
        {
            count++;
            yield return StartCoroutine(_huggingfaceImageManager.SendPrompt(texte, LLM, (response) => { _toReturn = response; }));
        }
        _callback?.Invoke(_toReturn);
    }

    /// <summary>
    /// Allows generating a voice by choosing an LLM.
    /// </summary>
    /// <param name="LLM">Choice of LLM for voice generation</param>
    /// <param name="texte">The text to send to the LLM</param>
    /// <returns>Task for generating the voice</returns>
    public async Task<AudioClip> Generate_voice(LLM_voice LLM, string texte)
    {
        switch (LLM)
        {
            case LLM_voice.ElevenLabs:
                return await _elevenLabsManager.Generate_Voice(_memoryManager.GetMemory(MemoryManager.memoryType.Synopsis));
            case LLM_voice.autre:
                return await _elevenLabsManager.Generate_Voice(_memoryManager.GetMemory(MemoryManager.memoryType.Synopsis));
            default:
                return await _elevenLabsManager.Generate_Voice(_memoryManager.GetMemory(MemoryManager.memoryType.Synopsis));
        }
    }

    /// <summary>
    /// Saves the prompt for debugging purposes.
    /// </summary>
    /// <param name="model">The model used</param>
    /// <param name="job">The job details</param>
    /// <param name="prompt">The prompt text</param>
    private static void SavePromptForDebug(Model model, Job job, string prompt)
    {
        prompt = $"\n \n ---------------------------------------------------------------- Model : {model.ToString()}" + job.ToString() + "\n\n" + prompt;
        string filePath = Path.Combine(Application.streamingAssetsPath, "Prompts" + ".txt");
        File.AppendAllText(filePath, prompt);
    }

    /// <summary>
    /// Initializes CSV files for logging prompts and responses.
    /// </summary>
    private static void InitializeCSVFiles()
    {
        string debugFilePath = Path.Combine(Application.streamingAssetsPath, "Prompts_Responses_LastSession.csv");
        string allFilePath = Path.Combine(Application.streamingAssetsPath, "Prompts_Responses_AllTimes.csv");
        string influ = Path.Combine(Application.streamingAssetsPath, "Prompts_Mesures_Influencabilite.csv");

        // Delete the debug file at launch and recreate the header
        if (File.Exists(debugFilePath))
        {
            File.Delete(debugFilePath);
        }
        File.WriteAllText(debugFilePath, "Prompt;Response;Model;Job;ResponseTime;Timestamp;PromptTokens;ResponseTokens;langue\n", new UTF8Encoding(true));

        // Check if the global file exists, otherwise create the header
        if (!File.Exists(allFilePath))
        {
            File.WriteAllText(allFilePath, "Prompt;Response;Model;Job;ResponseTime;Timestamp;PromptTokens;ResponseTokens;langue\n", new UTF8Encoding(true));
        }
        if (!File.Exists(influ))
        {
            File.WriteAllText(influ, "Prompt;Response;Model;SuccesDeLinfluence;TypeDinfluence\n", new UTF8Encoding(true));
        }
    }

    /// <summary>
    /// Fixes the WebGL path by removing the leading slash if present.
    /// </summary>
    /// <param name="path">The path to fix</param>
    /// <returns>The fixed path</returns>
    public string FixWebGLPath(string path)
    {
        // If the path starts with "/https:/", remove the first "/"
        if (path.StartsWith("/https:/"))
        {
            path = path.Substring(1); // Remove the first character "/"
        }

        return path;
    }

    /// <summary>
    /// Downloads a file from the given URL.
    /// </summary>
    /// <param name="url">The URL to download from</param>
    /// <param name="callback">Callback function to handle the downloaded data</param>
    /// <returns>Coroutine for downloading the file</returns>
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
                Debug.LogError("Error downloading the file: " + request.error);
                callback?.Invoke(null);
            }
        }
    }

    #region Voice
    /// <summary>
    /// Change the current voice in Unity
    /// </summary>
    /// <param name="clip">The audio clip to set as the current voice</param>
    public void SetCurrentVoice(AudioClip clip)
    {
        CurrentVoice = clip;
    }

    /// <summary>
    /// Play the audio in Unity
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    public void PlayAudio(AudioClip clip)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 1.0f;  // Ensure the volume is correct
        audioSource.spatialBlend = 0.0f;  // Play the audio in 2D
        audioSource.mute = false;  // Ensure the sound is not muted

        if (clip != null && clip.length > 0)
        {
            audioSource.Play();
        }
    }
    #endregion
    #endregion
}
