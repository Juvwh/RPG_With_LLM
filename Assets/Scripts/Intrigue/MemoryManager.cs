using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static AgentManager;
using static Models;
using static Jobs;

/// <summary>
/// This script is the MemoryManager, it stores important plot information and retrieves it.
/// </summary>
public class MemoryManager : MonoBehaviour
{
    #region Variables
    [Header("Rooms and Plots")]
    [SerializeField]
    private string txt_synopsis;
    [SerializeField]
    private string txt_room_1;
    [SerializeField]
    private string txt_room_2;
    [SerializeField]
    private string txt_room_3;
    [SerializeField]
    private string txt_room_4;
    [SerializeField]
    private string txt_room_5;
    [SerializeField]
    private string txt_LastPhase; // Contains what happened during the last game phase. Contains the full text.
    private string txt_LastEvent; // Contains the player's last interaction (choice, dice roll, prompt)
    private string txt_LastGreenInteraction; // Contains the last small text to display in green in the UI.
    [SerializeField]
    private string _theme = "Fantasy";
    public string _synopsis { get; set; }
    public string _synopsisResumed { get; set; }
    public string _herosDescription { get; set; }
    public string _herosSante { get; set; }
    public string _inventaire { get; set; }
    public string _compagnons { get; set; }
    public string _compagnonsSante { get; set; }

    private List<Phase> _phases = new List<Phase>();
    private string txt_AllResume = "";
    public bool isUpdated = true;
    private int countResume = 0;

    private GameManager _gameManager;
    private ElevenLabsManager _elevenLabsAPI;
    private NextManager _nextManager;

    /// <summary>
    /// Enum for different types of memory.
    /// </summary>
    public enum memoryType
    {
        IntrigueComplete,
        HerosDescription,
        Synopsis,
        Room1,
        Room2,
        Room3,
        Room4,
        Room5,
    }
    #endregion
    #region Methods
    /// <summary>
    /// Initializes the component by setting up the GameManager, ElevenLabsAPI, and NextManager references.
    /// </summary>
    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _elevenLabsAPI = _gameManager._elevenLabsManager;
        _nextManager = _gameManager._nextManager;

        _herosSante = "Le héros est en pleine forme.";
        _inventaire = "Vide.";
        _compagnons = "Aucun.";
    }

    /// <summary>
    /// Returns the text stored in memory.
    /// </summary>
    /// <param name="_memoryType">Choose which part is desired.</param>
    /// <returns>The text stored in memory.</returns>
    public string GetMemory(memoryType _memoryType)
    {
        switch (_memoryType)
        {
            case memoryType.IntrigueComplete:
                return txt_synopsis;
            case memoryType.HerosDescription:
                return _herosDescription;
            case memoryType.Synopsis:
                return _synopsis;
            case memoryType.Room1:
                return txt_room_1;
            case memoryType.Room2:
                return txt_room_2;
            case memoryType.Room3:
                return txt_room_3;
            case memoryType.Room4:
                return txt_room_4;
            case memoryType.Room5:
                return txt_room_5;

            default:
                return txt_synopsis;
        }
    }

    /// <summary>
    /// Gets the current theme.
    /// </summary>
    /// <returns>The current theme.</returns>
    public string GetTheme()
    {
        return _theme;
    }

    /// <summary>
    /// Sets the current theme.
    /// </summary>
    /// <param name="theme">The theme to set.</param>
    public void SetTheme(string theme)
    {
        _theme = theme;
    }

    /// <summary>
    /// Gets the synopsis.
    /// </summary>
    /// <returns>The synopsis.</returns>
    public string GetSynopsis()
    {
        return _synopsis;
    }

    /// <summary>
    /// Sets the last interaction text.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetLastInteract(string text)
    {
        txt_LastEvent = text;
    }

    /// <summary>
    /// Gets the last interaction text.
    /// </summary>
    /// <returns>The last interaction text.</returns>
    public string GetLastInteract()
    {
        return txt_LastEvent;
    }

    /// <summary>
    /// Gets the last phase text.
    /// </summary>
    /// <returns>The last phase text.</returns>
    public string GetLastPhase()
    {
        return txt_LastPhase;
    }

    /// <summary>
    /// Gets the last event text.
    /// </summary>
    /// <returns>The last event text.</returns>
    public string GetLastEvent()
    {
        return txt_LastGreenInteraction;
    }

    /// <summary>
    /// Allows retrieving the text stored in memory for a specific room.
    /// </summary>
    /// <param name="room">The room number.</param>
    /// <returns>The text stored in memory for the specified room.</returns>
    public string GetMemoryRoom(int room)
    {
        switch (room)
        {
            case 0:
                return txt_room_1;
            case 1:
                return txt_room_2;
            case 2:
                return txt_room_3;
            case 3:
                return txt_room_4;
            case 4:
                return txt_room_5;
            default:
                return txt_room_1;
        }
    }

    /// <summary>
    /// Sets the last event text.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetLastEvent(string text)
    {
        txt_LastGreenInteraction = text;
    }

    /// <summary>
    /// Sets the last phase text.
    /// </summary>
    /// <param name="text">The text to set.</param>
    public void SetLastPhase(string text)
    {
        txt_LastPhase = text;
    }

    /// <summary>
    /// Stores the global summary text.
    /// </summary>
    /// <param name="text">The text to store.</param>
    public void SetResume(string text)
    {
        txt_AllResume = text;
    }

    /// <summary>
    /// Stores the text in a specific memory.
    /// </summary>
    /// <param name="_memoryType">The type of memory to store the text in.</param>
    /// <param name="text">The text to store.</param>
    public void SetMemory(memoryType _memoryType, string text)
    {
        switch (_memoryType)
        {
            case memoryType.IntrigueComplete:
                txt_synopsis = text;
                break;
            case memoryType.HerosDescription:
                _herosDescription = text;
                break;
            case memoryType.Synopsis:
                _synopsis = text;
                break;
            case memoryType.Room1:
                txt_room_1 = text;
                break;
            case memoryType.Room2:
                txt_room_2 = text;
                break;
            case memoryType.Room3:
                txt_room_3 = text;
                break;
            case memoryType.Room4:
                txt_room_4 = text;
                break;
            case memoryType.Room5:
                txt_room_5 = text;
                break;

            default:
                txt_synopsis = text;
                break;
        }
    }

    /// <summary>
    /// Creates the complete plot and divides it into several parts, then stores it in the corresponding memory variables.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    public IEnumerator Generate_Intrigue()
    {
        bool success = false;

        while (!success)
        {
            // Step 1: Generate the text
            var values = new Dictionary<string, string>
            {
                { "theme", _theme },
                { "characters", _gameManager._charactersManagers.GetAllCharactersNameAndDescriptions() }
            };
            string _prompt = _gameManager._language.GetPrompt("__createPrompt__Generate_Intrigue__", values);
            string _reponse = null;
            yield return StartCoroutine(_gameManager._agent.Generate_Text(
            _gameManager.m_currentModel, Job.Intrigue, _prompt, (response) =>
            {
                _reponse = response;
            }));

            // Step 2: Save each information text
            string[] all_txt = Parser.SYNOPSYS(_reponse);

            success = all_txt.Length == 5;

            if (!success)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            _synopsis = all_txt[0];
            _synopsisResumed = all_txt[1];
            txt_room_1 = all_txt[2];
            txt_room_2 = all_txt[3];
            txt_room_3 = all_txt[4];

            // If everything went well, exit the loop
            success = true;
        }
    }

    /// <summary>
    /// Generates the plot in stages.
    /// </summary>
    /// <returns>An IEnumerator for coroutine execution.</returns>
    public IEnumerator Generate_Intrigue_mesure()
    {
        bool success = false;
        string _reponse = null;
        while (!success)
        {
            // Step 1: Generate the text
            var values = new Dictionary<string, string>
            {
                { "theme", _theme },
                { "characters", _gameManager._charactersManagers.GetAllCharactersNameAndDescriptions() }
            };
            string _prompt = _gameManager._language.GetPrompt("__createPrompt__Generate_Intrigue__", values);
            yield return StartCoroutine(_gameManager._agent.Generate_Text(
            _gameManager.m_currentModel, Job.Mesure_intrigue, _prompt, (response) =>
            {
                _reponse = response;
            }));

            success = true;
        }

        Model _currentModel = _gameManager.m_currentModel;
        _gameManager.m_currentModel = Model.llama_3_3_70b_versatile;
        string _reponse2 = null;
        yield return StartCoroutine(_gameManager._agent.Generate_Text(
        _gameManager.m_currentModel, Job.Antagonsite, _reponse, (response) =>
        {
            _reponse2 = response;
        }));
        _gameManager.m_currentModel = _currentModel;
    }

    /// <summary>
    /// Gets all phases of a specific room.
    /// </summary>
    /// <param name="room">The room number.</param>
    /// <returns>The concatenated text of all phases for the specified room.</returns>
    public string GetAllPhasesOfRoom(int room)
    {
        string _phaseTxt = "";
        foreach (Phase _phase in _phases)
        {
            if (_phase.NbrSalle == room)
            {
                _phaseTxt += _phase.Description + " ";
            }
        }
        return _phaseTxt;
    }
    #endregion
}

/// <summary>
/// Represents a phase in the game.
/// </summary>
public class Phase
{
    #region Variables
    private string _description;
    private int _room;
    #endregion
    #region Methods
    /// <summary>
    /// Constructor for the Phase class.
    /// </summary>
    /// <param name="description">The description of the phase.</param>
    /// <param name="nbrSalle">The room number associated with the phase.</param>
    public Phase(string description, int nbrSalle)
    {
        this._description = description;
        this._room = nbrSalle;
    }

    /// <summary>
    /// Gets or sets the description of the phase.
    /// </summary>
    public string Description
    {
        get { return _description; }
        set { _description = value; }
    }

    /// <summary>
    /// Gets or sets the room number associated with the phase.
    /// </summary>
    public int NbrSalle
    {
        get { return _room; }
        set { _room = value; }
    }
    #endregion
}

