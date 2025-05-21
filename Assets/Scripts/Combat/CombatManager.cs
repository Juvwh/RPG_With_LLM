using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using static Jobs;
using static AgentManager;
using System.Linq;
using static UI_Manager;
using TMPro;
using System;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Unity.VisualScripting;
using static Events;
using System.Threading;
using System.Buffers;
using static UI_SelectPerso;

#region Class EnemyData

public class EnemyData
{
    public string _name;
    public int _health;
    public int _armor;
    public int _enemyIndex;
    public bool _isDead;
    public string _description;
    public string _physique;
    public string _type;
    public int _maxDegats;
    public int _maxHealth;
    public int _precision;
    public GameObject _ui;
    public Sprite _sprite;

    public EnemyData(string name, int health, int armor, int enemyIndex, string description, string physique, string type, GameObject ui)
    {
        _name = name;
        _health = health;
        _armor = armor;
        _enemyIndex = enemyIndex;
        _isDead = false;
        _description = description;
        _physique = physique;
        _type = type;
        _ui = ui;
        _sprite = null;
    }
}
#endregion

public class CombatManager : MonoBehaviour
{

    #region God Mode Settings
    [Header("GOD MODE")]
    public bool GOD_MODE_HERO = false; // Enable god mode for hero
    public bool GOD_MODE_ENNEMI = false; // Enable god mode for enemies
    #endregion

    #region Manager References
    [Header("Managers")]
    public GameManager _gm; // Reference to the GameManager
    public UI_Manager _ui; // Reference to the UI Manager
    public LocalizationManager _language; // Reference to the Language Manager
    #endregion

    #region Combat Information
    [Header("Combat Information")]
    public bool _isCombat; // Flag to check if combat is ongoing
    public bool _isHeroFirst; // Flag to check if hero goes first
    public Group _group; // Group information
    public bool _isPlayerPlaying; // Flag to check if it's the player's turn
    public bool _isAttack; // Flag to check if an attack is in progress
    public bool _herosWon = false; // Flag to check if heroes have won
    public int _enemyCountInitial = 0; // Initial count of enemies
    public int _heroCountInitial = 0; // Initial count of heroes
    #endregion

    #region Player Information
    [Header("Infos Players")]
    public PlayerData[] _players; // Array of player data
    #endregion

    #region Enemy Information
    [Header("Infos Ennemis")]
    public int _nbEnnemis; // Number of enemies
    public string _typesEnnemis; // Types of enemies
    public EnemyData[] _enemies; // Array of enemy data
    public EnemyData[] _enemiesInitial; // Initial array of enemy data
    #endregion

    #region Combat Turn Information
    [Header("Liste chaînée")]
    public LinkedList<Combat_CharacterInfos> _charactersTour; // Linked list for turn order
    public LinkedListNode<Combat_CharacterInfos> _nodeCurrentCharacter; // Current character node
    public Combat_CharacterInfos _currentCharacter; // Current character info
    public int _currentCharacterIndex; // Index of the current character attacking
    #endregion

    #region Combat UI Characters Info
    [Header("Combat UI Characters Info")]
    public GameObject[] _HerosGameobject; // Array of hero GameObjects
    public GameObject[] _EnnemisGameobject; // Array of enemy GameObjects
    #endregion

    #region Selection Information
    [Header("Selection")]
    public Combat_CharacterInfos _selected; // Selected character info
    #endregion

    #region Corruption Settings
    [Header("Corruption")]
    public bool _isCorruption = false; // Flag to check if corruption is active
    #endregion

    #region Debug Settings
    [Header("Debug Sprite")]
    public Sprite m_SpriteEnnemyDebug; // Debug sprite for enemies if API inference fails
    #endregion

    #region Other Variables
    [Header("Other")]
    private string _preambule; // Preambule text
    private List<PlayerData> _playerDead = new List<PlayerData>(); // List of dead players
    public ToggleSwitch _toggleSwitch; // Toggle switch reference
    public List<String> _allPreviousEnnemy = new List<String>(); // List of all previous enemies
    #endregion

    private void Start()
    {
        // Find and assign the GameManager instance
        _gm = FindFirstObjectByType<GameManager>();

        // Assign the UI Manager from the GameManager
        _ui = _gm._UI_Manager;

        // Initialize combat status to false
        _isCombat = false;

        // Assign the Language Manager from the GameManager
        _language = _gm._language;
    }

    public void Reset()
    {
        // Reset combat status and related flags
        _isCombat = false;
        _isHeroFirst = false;
        _group = null;
        _isPlayerPlaying = false;

        // Reset player and enemy data
        _players = null;
        _nbEnnemis = 0;
        _typesEnnemis = "";
        _enemies = null;

        // Reset turn order and character information
        _charactersTour = null;
        _nodeCurrentCharacter = null;
        _currentCharacter = null;
        _currentCharacterIndex = 0;
        _selected = null;

        // Reset win status and initial counts
        _herosWon = false;
        _enemyCountInitial = 0;
        _heroCountInitial = 0;

        // Reset each hero's combat information
        foreach (GameObject _pl in _HerosGameobject)
        {
            _pl.GetComponent<Combat_PlayerInfo>().Reset();
        }

        // Reset each enemy's combat information
        foreach (GameObject _en in _EnnemisGameobject)
        {
            _en.GetComponent<Combat_EnemyInfo>().Reset();
        }
    }



    #region Initialisation
    public IEnumerator StartCombat(int groupIndex, bool _isHeroFirst)
    {
        // Activate Combat Loading UI
        _gm._UI_Manager.m_Canvas_ChargementCombat.SetActive(true);
        _gm._UI_Manager.m_TexteHerosReperent.SetActive(_isHeroFirst);
        _gm._UI_Manager.m_TexteEnnemisReperent.SetActive(!_isHeroFirst);

        // Special condition for room 2
        if (_gm._historyManager._currentRoom == 2)
        {
            _isHeroFirst = false;
        }

        // Set up necessary variables for combat
        _isCombat = true; // Switch to combat mode
        _group = _gm._groupsManager.m_Groups[groupIndex]; // Indicate which group started the combat
        _players = _group.m_Players; // Retrieve the heroes
        _isPlayerPlaying = _isHeroFirst;

        // Initialize the number and types of enemies randomly
        (int, string)[] _combatPossibles = {
        (3, "P,P,P"),
        (2,"P,P"),
        (1,"P"),
        (1,"M"),
        (2,"M,M"),
        (2,"G,P"),
        (3,"M,P,P"),
        (1,"G"),
        (2,"M,P")
    };

        // Adjust possible combats based on the number of players
        if (_players.Length == 1)
        {
            _combatPossibles = new (int, string)[] { (1, "P"), (1, "M") };
        }
        else if (_players.Length == 2)
        {
            _combatPossibles = new (int, string)[] {
            (2,"P,P"),
            (1,"P"),
            (1,"M"),
            (2,"M,P")
        };
        }
        else if (_players.Length == 3)
        {
            _combatPossibles = new (int, string)[] {
            (2,"P,P"),
            (1,"P"),
            (3, "P,P,P"),
            (1,"M"),
            (2,"M,P"),
            (2,"M,M"),
            (1,"G")
        };
        }

        // Special combat setup for room 2
        if (_gm._historyManager._currentRoom == 2)
        {
            _combatPossibles = new (int, string)[] { (1, "Boss"), };
        }

        // Select a random combat setup
        var random = new System.Random();
        var selectedCombat = _combatPossibles[random.Next(_combatPossibles.Length)];
        _nbEnnemis = selectedCombat.Item1;
        _typesEnnemis = selectedCombat.Item2;

        // Initialize the linked list and add heroes and enemies
        _charactersTour = new LinkedList<Combat_CharacterInfos>();

        // Hide unnecessary heroes and enemies / Display necessary heroes and enemies
        for (int i = 0; i < _HerosGameobject.Length; i++)
        {
            _HerosGameobject[i].SetActive(i < _players.Length);
            if (i < _players.Length)
            {
                _charactersTour.AddLast(_HerosGameobject[i].GetComponent<Combat_CharacterInfos>()); // Add heroes to the linked list
            }
        }

        for (int i = 0; i < _EnnemisGameobject.Length; i++)
        {
            _EnnemisGameobject[i].SetActive(i < _nbEnnemis);
            if (i < _nbEnnemis)
            {
                _charactersTour.AddLast(_EnnemisGameobject[i].GetComponent<Combat_CharacterInfos>()); // Add enemies to the linked list
            }
        }

        // Set the current character index based on who starts first
        _currentCharacterIndex = _isHeroFirst ? _charactersTour.Count - 1 : _players.Length - 1;
        _currentCharacter = GetElementAt(_charactersTour, _currentCharacterIndex);
        _nodeCurrentCharacter = _charactersTour.Find(_currentCharacter);

        // Generate enemies
        yield return StartCoroutine(Generer_Ennemies());

        // Generate images for enemies
        yield return StartCoroutine(GenerateImageForEnemy());
        _enemiesInitial = _enemies.ToArray();

        // Generate the story sequence to display on screen
        yield return StartCoroutine(Generate_Combat_Starting_Sequence());
        Update_Narration_Text(_preambule);

        // Wait until all enemies have a non-null sprite
        while (_enemies.Any(enemy => enemy._sprite == null))
        {
            yield return new WaitForSeconds(1);
        }

        // Fill in the heroes' information on the UI
        for (int i = 0; i < _players.Length; i++)
        {
            _HerosGameobject[i].GetComponent<Combat_PlayerInfo>().m_PlayersLinked = _players[i];
            _HerosGameobject[i].GetComponent<Combat_PlayerInfo>().UpdateInformation();
        }

        // Fill in the enemies' information on the UI
        for (int i = 0; i < _nbEnnemis; i++)
        {
            _EnnemisGameobject[i].GetComponent<Combat_EnemyInfo>().m_EnemyLinked = _enemies[i];
            _EnnemisGameobject[i].GetComponent<Combat_EnemyInfo>().UpdateInformation();
            _enemies[i]._ui = _EnnemisGameobject[i];
            _enemies[i]._enemyIndex = i;
        }

        // Prepare the UI for combat
        _gm._UI_Manager.m_Canvas_ChargementCombat.SetActive(false);
        _gm._UI_Manager.m_CanvasCombat.SetActive(true);
        Show_Text_Narration(true);
        Show_Explications_Characters_Turn(true);
        Update_Informations_Text(_gm._language.GetText("_combat_phase_de_combat"));
        Update_Text(_gm._language.GetText("_combat_force_honneur"));
        Show_Continue_Narration_Button(true);
        Show_Prompt_Button(false);
        Show_Continue_Narration_Button(true);
        Show_Continue_Combat_Finished_Button(false);
        Show_Button_Close_Combat(false);
        Show_Change_Heal_Attack_Button(false);
        _gm._UI_Manager.ChangePositionOptionCanvas(true);
    }


    public IEnumerator Generer_Ennemies()
    {
        // Initialize the enemies array
        _enemies = new EnemyData[0];
        var random = new System.Random();

        // Generate enemies based on game state
        var value = new Dictionary<string, string>
    {
        {"theme", _gm._memoryManager.GetTheme()},
        {"synopsys", _gm._memoryManager._synopsisResumed},
        {"environnement", _gm._memoryManager.GetMemoryRoom(_gm._historyManager.GetCurrentRoom())},
        {"count", _nbEnnemis.ToString()},
        {"difficulte", _typesEnnemis},
        {"previousEnemy", string.Join(", ", _allPreviousEnnemy)}
    };

        // Create prompt for enemy generation
        string prompt = _gm._language.GetPrompt("__combat_creation_prompt_generation_enemy", value);
        prompt += _allPreviousEnnemy.Count > 0 ? _gm._language.GetPrompt("__combat_creation_prompt_generation_enemy_info_supp", value) : "\n}}";

        // API request to generate enemies
        while (_enemies.Length != _nbEnnemis)
        {
            _enemies = new EnemyData[0];
            string txt_generated = "";

            if (_gm._historyManager._currentRoom == 2)
            {
                // Generate boss enemy
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.CombatStartBoss, prompt, (response) => { txt_generated = response; }));
                _enemies = Parser.COMBAT_BOSS(txt_generated, _nbEnnemis);
            }
            else
            {
                // Generate regular enemies
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.combat_start, prompt, (response) => { txt_generated = response; }));
                _enemies = Parser.COMBAT(txt_generated, _nbEnnemis);
            }

            if (_enemies.Length != _nbEnnemis)
            {
                yield return new WaitForSeconds(1);
            }
        }

        // Add health points and other attributes for each enemy
        foreach (EnemyData _enemy in _enemies)
        {
            _allPreviousEnnemy.Add(_enemy._name);
            switch (_enemy._type)
            {
                case "P":
                    _enemy._health = 8 + random.Next(-1, 2);
                    _enemy._armor = 7 + random.Next(-1, 1);
                    _enemy._maxDegats = 5 + random.Next(-1, 1);
                    _enemy._precision = 0 + random.Next(-1, 1);
                    break;
                case "M":
                    _enemy._health = 10 + random.Next(-1, 2);
                    _enemy._armor = 8 + random.Next(-1, 1);
                    _enemy._maxDegats = 6 + random.Next(-1, 1);
                    _enemy._precision = 2 + random.Next(-1, 1);
                    break;
                case "G":
                    _enemy._health = 12 + random.Next(-1, 2);
                    _enemy._armor = 9 + random.Next(-1, 1);
                    _enemy._maxDegats = 7 + random.Next(-1, 1);
                    _enemy._precision = 4 + random.Next(-1, 1);
                    break;
                case "Boss":
                    _enemy._health = 20 + random.Next(-1, 2);
                    _enemy._armor = 10 + random.Next(-1, 1);
                    _enemy._maxDegats = 8;
                    _enemy._precision = 6 + random.Next(-1, 1);
                    break;
                default:
                    break;
            }
            _enemy._maxHealth = _enemy._health;
        }
    }

    public IEnumerator GenerateImageForEnemy()
    {
        // Generate images for each enemy
        foreach (EnemyData _enemy in _enemies)
        {
            StartCoroutine(Generate_Enemy_Image_Multithreading(_enemy));
        }
        yield break;
    }

    public IEnumerator Generate_Enemy_Image_Multithreading(EnemyData _enemy)
    {
        // Generate image for the enemy
        Texture2D image = null;
        if (_gm.generateImage)
        {
            yield return StartCoroutine(_gm.Generate_Image(GameManager.LLM_image.BlackForestFast, $"{_enemy._physique}, look very evil, RPG arts, masterpiece", (responseImage) => { image = responseImage; }));
        }

        // Set the sprite for the enemy
        Sprite currentSprite = null;
        if (image == null)
        {
            currentSprite = m_SpriteEnnemyDebug;
        }
        else
        {
            currentSprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
        }
        _enemy._sprite = currentSprite;
        yield break;
    }

    public IEnumerator Generate_Combat_Starting_Sequence()
    {
        // Prepare values for the prompt
        var value = new Dictionary<string, string>
    {
        {"theme", _gm._memoryManager.GetTheme()},
        {"synopsys", _gm._memoryManager._synopsisResumed},
        {"environnement", _gm._memoryManager.GetMemoryRoom(_gm._historyManager.GetCurrentRoom())},
        {"personnages", _group.GetResume_Names()},
        {"oldposition", _group.GetResume_LastPosition()},
        {"action", _group._lastPhaseResume}
    };

        // Generate the initial prompt for combat sequence
        string prompt_Free = _gm._language.GetPrompt("_combat_createprompt_starting", value);
        prompt_Free += _isHeroFirst ? _gm._language.GetText("_combat_createprompt_starting_Suite_hero") : _gm._language.GetText("_combat_createprompt_starting_Suite_ennemi");

        // Add enemy count to the prompt
        var value2 = new Dictionary<string, string>
    {
        {"count", _nbEnnemis.ToString()}
    };
        prompt_Free += _gm._language.GetPrompt("_combat_createprompt_enemy_count", value2);

        // Add enemy details to the prompt
        for (int i = 0; i < _nbEnnemis; i++)
        {
            prompt_Free += $"\n<{_gm._language.GetText("_combat_ennemi_word")} {i + 1} : {_enemies[i]._name} : {_enemies[i]._description}\",\n";
        }
        prompt_Free += "}";

        bool done = false;
        string history = "";
        string resume = "";

        // Generate text until successful
        while (!done)
        {
            string text_Genere = "";
            if (_gm._historyManager._currentRoom == 2)
            {
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.FreeBoss, prompt_Free, (response) => { text_Genere = response; }));
            }
            else
            {
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.FreeCombat, prompt_Free, (response) => { text_Genere = response; }));
            }
            done = Parser.FREE_COMBAT(text_Genere, out history, out resume);
            if (!done)
            {
                yield return new WaitForSeconds(1);
            }
        }

        // Add generated history and resume to the group
        _group.AddHistory(TextType.Histoire, history);
        _group.AddHistoryResume(resume);
        _preambule = history;
    }

    public static T GetElementAt<T>(LinkedList<T> list, int index)
    {
        // Iterate through the linked list to find the element at the specified index
        int currentIndex = 0;
        foreach (var item in list)
        {
            if (currentIndex == index)
            {
                return item;
            }
            currentIndex++;
        }
        throw new InvalidOperationException("Index not found");
    }
    #endregion

    #region Combat --> Tour héros

    public void Combat_Iniatilise_Player_Sequence()
    {
        // Gray out other characters and focus on the current player
        foreach (Combat_CharacterInfos _character in _charactersTour)
        {
            _character.ShowShadow(!_character.m_IsPlaying);
            _character.m_IsSelectable = false;
            _character.m_IsSelected = false;
        }
        _selected = null;
        _toggleSwitch.Reset();

        // Display attack and heal buttons
        var value = new Dictionary<string, string>
    {
        {"name", _currentCharacter.m_Text_Value_Name.text}
    };
        Update_Informations_Text(_language.GetPrompt("_combat_autourde", value));
        Update_Text(_language.GetText("_combat_que_veux_tu_faire"));
        Combat_Player_Initialise_Attack_Sequence(); // Default to attack
    }


    //ATTACK
    /// <summary>
    /// Initializes the player's turn for attacking. Sets up the UI for selecting a target and entering an attack description.
    /// </summary>
    public void Combat_Player_Initialise_Attack_Sequence()
    {
        _selected = null;

        Show_Change_Heal_Attack_Button(true);
        // Display the input prompt
        _currentCharacter.GetComponent<Combat_PlayerInfo>().CanUseInventory(true);
        Show_Prompt_Button(true);
        Update_Txt_PlaceHolderPrompt(_language.GetText("_combat_place_holder_attack"));
        // Indicate in the explanation that the player must choose a target
        Update_Text(_language.GetText("_combat_place_attack_describe"));
        // Make enemies selectable
        foreach (EnemyData _enemy in _enemies)
        {
            _enemy._ui.GetComponent<Combat_EnemyInfo>().ShowShadow(false);
            _enemy._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = true;
        }
        // Turn off the Send Prompt button until a target is selected
        TurnSendPromptButton(false);
        _gm._UI_Manager.UpdateTextExplicationsSupplementaireBouttonSend(_language.GetText("_combat_send_button_explication_attaque"));
        _isAttack = true; // Indicate that this is an attack
                          // The player must now choose a target and enter the description of their attack
    }

    /// <summary>
    /// Initializes the player's turn for healing. Sets up the UI for selecting a target and entering a heal description.
    /// </summary>
    public void Combat_Player_Initialise_Health_Sequence()
    {
        _selected = null;
        Show_Change_Heal_Attack_Button(true);
        // Display the input prompt
        _currentCharacter.GetComponent<Combat_PlayerInfo>().CanUseInventory(true);
        Show_Prompt_Button(true);
        Update_Txt_PlaceHolderPrompt(_language.GetText("_combat_place_holder_soin"));
        // Indicate in the explanation that the player must choose a target
        Update_Text(_language.GetText("_combat_place_soin_describe"));
        // Make enemies non-selectable and heroes selectable
        foreach (EnemyData _enemy in _enemies)
        {
            _enemy._ui.GetComponent<Combat_EnemyInfo>().ShowShadow(true);
            _enemy._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = false;
        }
        foreach (GameObject _pl in _HerosGameobject)
        {
            if (_pl.GetComponent<Combat_PlayerInfo>()._isDead || !_pl.activeSelf) { continue; }
            _pl.GetComponent<Combat_PlayerInfo>().ShowShadow(false);
            _pl.GetComponent<Combat_PlayerInfo>().m_IsSelectable = true;
        }
        // Turn off the Send Prompt button until a target is selected
        TurnSendPromptButton(false);
        _gm._UI_Manager.UpdateTextExplicationsSupplementaireBouttonSend(_language.GetText("_combat_send_button_explication_soin"));
        _isAttack = false; // Indicate that this is a heal
                           // The player must now choose a target and enter the description of their heal
    }
    #endregion

    #region Combat --> Tour ennemis

    /// <summary>
    /// Initializes the enemy's turn. Sets up the UI to show that the enemy is thinking and prepares for the enemy's action.
    /// </summary>
    public void Combat_Enemy_Sequence_Initialise()
    {
        // Gray out other characters and enemies and focus on the enemy who is playing
        var value = new Dictionary<string, string>
    {
        {"name", _currentCharacter.m_Text_Value_Name.text}
    };
        Update_Informations_Text(_language.GetPrompt("_combat_autourde", value));
        foreach (Combat_CharacterInfos _character in _charactersTour)
        {
            _character.ShowShadow(!_character.m_IsPlaying);
            _character.m_IsSelectable = false;
            _character.m_IsSelected = false;
        }

        // Display loading
        Show_Combat_Loading_Screen(true);
        // Indicate in the explanation that the enemy is thinking
        Update_Text(_language.GetText("_combat_ennemi_pense"));
        StartCoroutine(Combat_Enemy_Sequence());
    }

    /// <summary>
    /// Handles the sequence of actions for an enemy's turn in combat. Determines if the enemy attacks or heals and processes the action.
    /// </summary>
    public IEnumerator Combat_Enemy_Sequence()
    {
        string prompt = Create_Enemy_Sequence_Prompt();

        // API request
        bool _done = false;
        bool _isAttack = false;
        string _cible = "erreur";
        string _strategie = "erreur";

        while (!_done)
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Combat_Tour_Ennemi, prompt, (response) => { text_Genere = response; }));
            _done = Parser.COMBAT_ENEMY_SEQUENCE(text_Genere, out _isAttack, out _cible, out _strategie);
            if (!_done)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            _done = false; // Reset done to false to verify if the target is correct

            // If attacking, verify that the target is among the heroes
            if (_isAttack)
            {
                foreach (GameObject _pl in _HerosGameobject)
                {
                    if (_pl.GetComponent<Combat_PlayerInfo>()._isDead || !_pl.activeSelf) { continue; }
                    if (_pl.GetComponent<Combat_PlayerInfo>().m_PlayersLinked.playerName == _cible)
                    {
                        _done = true;
                        _selected = _pl.GetComponent<Combat_PlayerInfo>();
                    }
                }
            }
            else // If healing, verify that the target is among the enemies
            {
                foreach (EnemyData _enemy in _enemies)
                {
                    if (_enemy._name == _cible)
                    {
                        _selected = _enemy._ui.GetComponent<Combat_EnemyInfo>();
                        _done = true;
                    }
                }
            }
            if (!_done)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
        }

        // Display the enemy's strategy
        _currentCharacter.GetComponent<Combat_EnemyInfo>().UpdateStrategie(true, _strategie);

        // If it's an attack
        if (_isAttack)
        {
            // Update the explanation text
            Update_Text($"{_language.GetText("_combat_ennemi_choisit_attaque")} {_selected.GetComponent<Combat_PlayerInfo>().m_PlayersLinked.playerName}.");
            StartCoroutine(Combat_Enemy_Attack_Sequence());
            yield break;
        }
        else
        {
            // Update the explanation text
            Update_Text($"{_language.GetText("_combat_ennemi_choisit_soin")} {_selected.GetComponent<Combat_EnemyInfo>().m_EnemyLinked._name}.");
            StartCoroutine(Combat_Enemy_Health_Sequence());
            yield break;
        }
    }

    /// <summary>
    /// Handles the enemy's attack turn. Determines if the attack hits and processes the damage.
    /// </summary>
    public IEnumerator Combat_Enemy_Attack_Sequence()
    {
        PlayerData _heroSelected = _selected.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;
        EnemyData _enemy = _currentCharacter.GetComponent<Combat_EnemyInfo>().m_EnemyLinked;

        // Start by choosing a random number between 1 and 20
        var random = new System.Random();
        int _dicePrecision = random.Next(1, 21);
        _dicePrecision += _enemy._precision;

        string prompt = "";

        // Check if the enemy hits the hero
        if (_dicePrecision < _heroSelected.playerArmorClass + _selected.GetComponent<Combat_PlayerInfo>()._bonusArmure)
        {
            var value = new Dictionary<string, string>
        {
            {"name", _heroSelected.playerName },
            {"race", _heroSelected.playerRace},
            {"classe", _heroSelected.playerClass},
            {"description", _heroSelected.playerDescription},
            {"sante", _heroSelected.playerHealth},
            {"ennemi", _enemy._name},
            {"ennemi_description", _enemy._description}
        };

            // The enemy missed their attack because the dice roll is less than the hero's armor
            prompt = _language.GetPrompt("_combat_ennemi_tour_attaque_ennemi_rate", value);
        }
        else
        {
            var value = new Dictionary<string, string>
        {
            {"name", _heroSelected.playerName },
            {"race", _heroSelected.playerRace},
            {"classe", _heroSelected.playerClass},
            {"description", _heroSelected.playerDescription},
            {"sante", _heroSelected.playerHealth},
            {"santeInt", _heroSelected.playerHealthNbr.ToString() },
            {"ennemi", _enemy._name},
            {"ennemi_description", _enemy._description}
        };

            // The enemy succeeded in their attack
            prompt = _language.GetPrompt("_combat_ennemi_tour_attaque_ennemi_reussi", value);

            // The enemy succeeded in their attack
            int _diceDegats = random.Next(1, _currentCharacter.GetComponent<Combat_EnemyInfo>().m_EnemyLinked._maxDegats); // Randomly choose a number between 1 and the enemy's max damage
            _diceDegats = GOD_MODE_ENNEMI ? 100 : _diceDegats; // DEBUG
            _heroSelected.playerHealthNbr -= _diceDegats; // Subtract the damage from the hero's health
                                                          // MODIF POUR TEST
                                                          //_heroSelected.playerHealthNbr = 0;

            if (_heroSelected.playerHealthNbr <= 0) // Check if the player is dead
            {
                _players = _players.Where(val => val != _heroSelected).ToArray(); // Remove this hero from the list of players

                // Remove this hero from the linked list
                _charactersTour.Remove(_selected);

                // Handle death case
                _playerDead.Add(_heroSelected);
                var val = new Dictionary<string, string>
            {
                { "name", _heroSelected.playerName}
            };
                // End of death case handling
                prompt += $"\n\"{_language.GetText("_combat_santeEnChiffre")}\" : {_heroSelected.playerHealthNbr} /20 \"\n" +
                    _language.GetPrompt("_combat_hero_vaincu", val);
                _selected.GetComponent<Combat_PlayerInfo>().m_DeathVector.SetActive(true);
                _selected.GetComponent<Combat_PlayerInfo>().ShowShadow(true);
                _selected.GetComponent<Combat_PlayerInfo>()._isDead = true;
                _currentCharacterIndex--;

                if (_players.Length == 0)
                {
                    _selected.GetComponent<Combat_CharacterInfos>().UpdateInformation();
                    StartCoroutine(SadEnding());
                    yield break;
                }
            }
            else
            {
                prompt += $"\n\"{_language.GetText("_combat_santeEnChiffre")}\" : {_heroSelected.playerHealthNbr} /20 \"\n";
            }
        }

        // Generate the story continuation
        bool done = false;
        string _history = "";
        string _summary = "";
        while (!done)
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Combat_narration, prompt, (response) => { text_Genere = response; }));
            done = Parser.COMBAT_SEQUENCE(text_Genere, out _history, out _summary);
            if (!done)
            {
                yield return new WaitForSeconds(1);
            }
        }

        // Update the narration
        Update_Narration_Text(_history);
        Show_Text_Narration(true);

        // Update the hero's information
        _heroSelected.playerHealth = _summary;
        _selected.GetComponent<Combat_CharacterInfos>().UpdateInformation();

        // Display the continue button
        Show_Combat_Loading_Screen(false);
        Show_Continue_Narration_Button(true);

        // End of turn
    }

    /// <summary>
    /// Handles the enemy's healing turn. Determines if the healing is successful and processes the healing.
    /// </summary>
    public IEnumerator Combat_Enemy_Health_Sequence()
    {
        EnemyData _enemySelected = _selected.GetComponent<Combat_EnemyInfo>().m_EnemyLinked;
        EnemyData _enemy = _currentCharacter.GetComponent<Combat_EnemyInfo>().m_EnemyLinked;

        // Start by choosing a random number between 1 and 20
        var random = new System.Random();
        int _dicePrecision = random.Next(1, 21);
        _dicePrecision += _enemy._precision;

        var value = new Dictionary<string, string>
    {
        {"enemyName", _enemy._name },
        {"enemyDescription", _enemy._description},
        {"enemySelectedName", _enemySelected._name },
        {"enemySelectedDescription", _enemySelected._description}
    };

        string prompt = _language.GetPrompt("_combat_ennemi_tour_soin", value);

        // Check if the enemy succeeds in healing
        if (_dicePrecision >= 10)
        {
            // The enemy succeeded in healing
            int _diceSoin = random.Next(1, 5); // Randomly choose a number between 1 and 5
                                               // Apply healing to the selected enemy
            _enemySelected._health += _diceSoin;
            if (_enemySelected._health > _enemySelected._maxHealth)
            {
                _enemySelected._health = _enemySelected._maxHealth;
            }
            value.Add("diceSoin", _diceSoin.ToString());
            prompt += _language.GetPrompt("_combat_ennemi_tour_soin_success", value);
        }
        else
        {
            prompt += _language.GetPrompt("_combat_ennemi_tour_soin_rate", value);
        }

        // Generate the story continuation
        bool done = false;
        string _history = "";
        string _summary = "";
        while (!done)
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Combat_Narration_Soin, prompt, (response) => { text_Genere = response; }));
            done = Parser.COMBAT_SEQUENCE(text_Genere, out _history, out _summary);
            if (!done)
            {
                yield return new WaitForSeconds(1);
            }
        }

        // Update the narration
        Update_Narration_Text(_history);
        Show_Text_Narration(true);

        // Update the enemy's information
        _selected.GetComponent<Combat_EnemyInfo>().UpdateInformation();

        // Display the continue button
        Show_Combat_Loading_Screen(false);
        Show_Continue_Narration_Button(true);

        // End of turn
    }

    /// <summary>
    /// Creates a prompt for the enemy's turn in combat, including details about the enemy and their allies.
    /// </summary>
    /// <returns>A string representing the constructed prompt.</returns>
    public string Create_Enemy_Sequence_Prompt()
    {
        // Construct the prompt to be sent to the LLM
        EnemyData currentEnemy = _currentCharacter.GetComponent<Combat_EnemyInfo>().m_EnemyLinked;
        var value = new Dictionary<string, string>
    {
        {"name", currentEnemy._name },
        {"description", currentEnemy._description},
        {"sante", currentEnemy._health.ToString() },
        {"santeMax", currentEnemy._maxHealth.ToString() }
    };

        string prompt = _language.GetPrompt("_create_prompt_tour_ennnemi_part_1", value);
        prompt += currentEnemy._health == currentEnemy._maxHealth ? $"{_language.GetText("_create_prompt_tour_ennnemi_part_2")}\"\n}}],\n" : "\"\n}],\n";

        prompt += $"\"{_language.GetText("_create_prompt_tour_ennemi_allies")}\": [";
        int i = 0;

        foreach (EnemyData _enemy in _enemies)
        {
            if (_enemy == currentEnemy) { continue; }

            var value2 = new Dictionary<string, string>
        {
            {"name", _enemy._name },
            {"sante", _enemy._health.ToString() },
            {"santeMax", _enemy._maxHealth.ToString() }
        };

            prompt += _language.GetPrompt("_create_prompt_tour_ennnemi_part_3", value2);
            prompt += _enemy._health == _enemy._maxHealth ? _language.GetText("_create_prompt_tour_ennnemi_part_2") : "";
            prompt += "\",\n";
            prompt += $"\"{_language.GetText("_armure")}\" : \"{_enemy._armor}\"\n" +
                        $"}}";
            i++;
            if (i < _enemies.Length - 2)
            {
                prompt += ",\n";
            }
        }

        prompt += $"\n],\n\"{_language.GetText("_heros")}\": [";
        i = 0;
        List<GameObject> shuffledList = _HerosGameobject.OrderBy(x => UnityEngine.Random.value).ToList();

        foreach (GameObject _pl in shuffledList)
        {
            if (_pl.GetComponent<Combat_PlayerInfo>()._isDead || !_pl.activeSelf) { continue; }
            PlayerData _player = _pl.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;

            var value3 = new Dictionary<string, string>
        {
            {"name", _player.playerName },
            {"sante", _player.playerHealth},
            {"class", _player.playerClass },
            {"race", _player.playerRace }
        };

            prompt += _language.GetPrompt("_create_prompt_tour_ennnemi_part_4", value3);
            i++;
            if (i < _players.Length - 1)
            {
                prompt += ",\n";
            }
        }
        prompt += "\n]\n}";
        prompt += $"\n {_language.GetText("_tu_es")}{_currentCharacter.GetComponent<Combat_EnemyInfo>().m_EnemyLinked._name}.{_language.GetText("_create_prompt_tour_ennnemi_part_5")}";

        return prompt;
    }
    #endregion

    #region Check Coherence
    /// <summary>
    /// Checks the coherence of the player's input for combat actions. Validates the input and processes the result.
    /// </summary>
    public IEnumerator LLM_Validation_Player_Attack_Input()
    {
        // Prepare the UI for loading
        Show_Unvalidation_Text(false);
        Show_Loading_Icon(true);
        Show_Prompt_Button(false);

        PlayerData _hero = _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;
        EnemyData _enemy = _selected.GetComponent<Combat_EnemyInfo>().m_EnemyLinked;

        // Retrieve the text from the input
        string input = _gm._UI_Manager.m_InputPromptCombat.GetComponent<TextMeshProUGUI>().text;

        // Build the prompt
        var value = new Dictionary<string, string>
    {
        {"heroName", _hero.playerName },
        {"heroRace", _hero.playerRace},
        {"heroClass", _hero.playerClass},
        {"heroArme", _hero.playerArme},
        {"heroDescription", _hero.playerDescription},
        {"heroSante", _hero.playerHealth},
        {"enemyName", _enemy._name },
        {"enemyDescription", _enemy._description },
        {"enemySante", _enemy._health.ToString() },
        {"input", input}
    };

        string prompt = _language.GetPrompt("_combat_check_coherence_attaque", value);

        string _statut = "[Error]";
        string _txt_Incoherence = "";
        string _txt_Summary = "";

        while (_statut == "[Error]")
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.ChechCoherenceInputCombat, prompt, (response) => { text_Genere = response; }));
            _statut = Parser.LLM_VALIDATION_COMBAT(text_Genere, out _txt_Incoherence, out _txt_Summary);
            if (_statut == "[Error]")
            {
                yield return new WaitForSeconds(1); // Wait 2 seconds before retrying the request
            }
        }

        if (_statut == "[True]")
        {
            // The user's input is coherent
            Show_Loading_Icon(false);
            // Reset the input text area
            _gm._UI_Manager.m_InputPromptTextGlob_.text = "";
            // Roll the dice
            Update_Txt_Action(_txt_Summary);
            Afficher_Txt_Action(true);
            StartCoroutine(Roll_Dice_Attack(_txt_Summary));
        }
        else // if (_statut == "[FALSE]")
        {
            // The user's input is not coherent
            // Hide the loading icon
            _currentCharacter.GetComponent<Combat_PlayerInfo>().CanUseInventory(true);
            Show_Loading_Icon(false);
            Show_Prompt_Button(true);
            Show_Unvalidation_Text(true);
            Update_Combat_Incoherence_Text(_txt_Incoherence);
            Show_Change_Heal_Attack_Button(true);
            foreach (EnemyData _e in _enemies)
            {
                _e._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = true;
            }
        }
    }

    //HEALTH
    /// <summary>
    /// Checks the coherence of the player's input for healing actions. Validates the input and processes the result.
    /// </summary>
    public IEnumerator LLM_Validation_Player_Health_Input()
    {
        // Prepare the UI for loading
        Show_Unvalidation_Text(false);
        Show_Loading_Icon(true);
        Show_Prompt_Button(false);

        PlayerData _hero = _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;
        PlayerData _selectedHero = _selected.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;

        // Retrieve the text from the input
        string input = _gm._UI_Manager.m_InputPromptCombat.GetComponent<TextMeshProUGUI>().text;

        // Build the prompt
        var value = new Dictionary<string, string>
    {
        {"heroName", _hero.playerName },
        {"heroRace", _hero.playerRace},
        {"heroClass", _hero.playerClass},
        {"heroArme", _hero.playerArme},
        {"heroDescription", _hero.playerDescription},
        {"heroSante", _hero.playerHealth},
        {"selectedHeroName", _selectedHero.playerName },
        {"selectedHeroRace", _selectedHero.playerRace},
        {"selectedHeroClass", _selectedHero.playerClass},
        {"selectedHeroDescription", _selectedHero.playerDescription},
        {"selectedHeroSante", _selectedHero.playerHealth.ToString() },
        {"input", input}
    };

        string prompt = _language.GetPrompt("_combat_check_coherence_soin", value);

        string _statut = "[Error]";
        string _txt_Incoherence = "";
        string _txt_Summary = "";

        while (_statut == "[Error]")
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.ChechCoherenceInputCombat, prompt, (response) => { text_Genere = response; }));
            _statut = Parser.LLM_VALIDATION_COMBAT(text_Genere, out _txt_Incoherence, out _txt_Summary);
            if (_statut == "[Error]")
            {
                yield return new WaitForSeconds(1); // Wait 2 seconds before retrying the request
            }
        }

        if (_statut == "[True]")
        {
            // The user's input is coherent
            Show_Loading_Icon(false);
            // Reset the input text area
            _gm._UI_Manager.m_InputPromptTextGlob_.text = "";
            // Roll the dice
            Update_Txt_Action(_txt_Summary);
            Afficher_Txt_Action(true);
            StartCoroutine(Roll_Dice_Health(_txt_Summary));
        }
        else // if (_statut == "[FALSE]")
        {
            // The user's input is not coherent
            // Hide the loading icon
            Show_Loading_Icon(false);
            _currentCharacter.GetComponent<Combat_PlayerInfo>().CanUseInventory(true);
            Show_Prompt_Button(true);
            Show_Unvalidation_Text(true);
            Update_Combat_Incoherence_Text(_txt_Incoherence);
            foreach (GameObject _pl in _HerosGameobject)
            {
                if (_pl.GetComponent<Combat_PlayerInfo>()._isDead || !_pl.activeSelf) { continue; }
                _pl.GetComponent<Combat_PlayerInfo>().m_IsSelectable = true;
            }
        }
    }
    #endregion

    #region Lancer de dé
    /// <summary>
    /// Handles the dice rolling for combat actions. Determines if the attack is successful and processes the damage.
    /// </summary>
    /// <param name="_descriptionAttaque">Description of the attack.</param>
    public IEnumerator Roll_Dice_Attack(string _descriptionAttaque)
    {
        // Indicate that this is a precision dice roll
        Update_Text(_language.GetText("_combat_precision_attaque"));
        Show_More_Explication_Dice(true);
        Update_MoreExplicationDice(_language.GetText("_combat_lancer_attaque"));

        // Roll the precision dice
        int _visee = -10;
        if (!GOD_MODE_HERO)
        {
            yield return StartCoroutine(_gm._diceManager.Roll_Dice(20, -1, -1, (response) => { _visee = response; }));
            _visee += _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked.playerAimBonus;

            // Wait until the 2D dice canvas is inactive to continue
            while (_gm._diceManager.m_CanvasDice2D.activeSelf)
            {
                yield return new WaitForSeconds(1);
            }
        }
        else
        {
            _visee = 50;
        }

        // Check if the attack is successful, if not, skip the damage dice roll
        if (_visee < _selected.GetComponent<Combat_EnemyInfo>().m_EnemyLinked._armor)
        {
            yield return StartCoroutine(Generate_Text_Attack_Sequence(_visee, 0, false, _descriptionAttaque));
            yield break;
        }
        else
        {
            // Roll the damage dice
            Update_Text(_language.GetText("_combat_degat_attaque"));
            Update_MoreExplicationDice(_language.GetText("_combat_lancer_degat"));

            int _dice1 = _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked.GetDiceWeapon();
            int _dice2 = -1;
            int _bonusAttaque = _currentCharacter.GetComponent<Combat_PlayerInfo>()._bonnusAttaque;

            if (_bonusAttaque == 2)
            {
                _dice1 += 2;
            }
            else
            {
                _dice2 = _bonusAttaque;
            }

            int _degats = -100;
            if (!GOD_MODE_HERO)
            {
                yield return StartCoroutine(_gm._diceManager.Roll_Dice(_dice1, _dice2, -1, (response) => { _degats = response; }));

                // Wait until the 2D dice canvas is inactive to continue
                while (_gm._diceManager.m_CanvasDice2D.activeSelf)
                {
                    yield return new WaitForSeconds(1);
                }
            }
            else
            {
                _degats = 100;
            }

            // Now that we have precision and damage, write the story continuation
            StartCoroutine(Generate_Text_Attack_Sequence(_visee, _degats, true, _descriptionAttaque));
        }
    }

    /// <summary>
    /// Handles the dice rolling for healing actions. Determines if the healing is successful and processes the healing.
    /// </summary>
    /// <param name="_descriptionSoin">Description of the healing.</param>
    public IEnumerator Roll_Dice_Health(string _descriptionSoin)
    {
        // Indicate that this is a precision dice roll
        Update_Text(_language.GetText("_combat_precision_soin"));
        Show_More_Explication_Dice(true);
        Update_MoreExplicationDice(_language.GetText("_combat_lancer_p_soin"));

        // Roll the precision dice
        int _visee = -10;
        yield return StartCoroutine(_gm._diceManager.Roll_Dice(20, -1, -1, (response) => { _visee = response; }));
        _visee += _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked.playerAimBonus;

        // Wait until the 2D dice canvas is inactive to continue
        while (_gm._diceManager.m_CanvasDice2D.activeSelf)
        {
            yield return new WaitForSeconds(1);
        }

        // Check if the healing is successful, if not, skip the healing dice roll
        if (_visee < 10)
        {
            yield return StartCoroutine(Generate_Text_Health_Sequence(_visee, 0, false, _descriptionSoin));
            yield break;
        }
        else
        {
            // Roll the healing dice
            Update_Text(_language.GetText("_combat_degat_soin"));
            Update_MoreExplicationDice(_language.GetText("_combat_lancer_soin"));

            // Here we choose random dice, I might add logic based on players later
            (int, int, int)[] _dice = {(4,-1,-1),(6,-1,-1), (8, -1, -1), (10, -1, -1), (4, 4, -1), (4, 4, 4), (6, 4, -1), (4, 4, 6)
                               ,(4,8,-1),(4,10,-1),(10,6,-1),(10,4,4),(4,8,4),(10,4,6),(8,8,0),(6,6,-1),(6,8,-1)
                                ,(6,8,-1),(6,10,4),(4,10,8)};

            System.Random random = new System.Random();
            (int, int, int) _diceChoisi = _dice[random.Next(_dice.Length)];
            int _degats = -100;
            yield return StartCoroutine(_gm._diceManager.Roll_Dice(_diceChoisi.Item1, _diceChoisi.Item2, _diceChoisi.Item3, (response) => { _degats = response; }));
            _degats = Remap_Range_Value_Health(_degats, 1, 22, 1, 5); // Remap values to make them more consistent

            // Wait until the 2D dice canvas is inactive to continue
            while (_gm._diceManager.m_CanvasDice2D.activeSelf)
            {
                yield return new WaitForSeconds(1);
            }

            // Now that we have precision and healing, write the story continuation
            yield return StartCoroutine(Generate_Text_Health_Sequence(_visee, _degats, true, _descriptionSoin));
        }
    }
    #endregion

    #region Ecriture
    /// <summary>
    /// Writes the continuation of the story for a player's attack. Updates the UI and processes the attack result.
    /// </summary>
    /// <param name="_visee">Precision value of the attack.</param>
    /// <param name="_degat">Damage value of the attack.</param>
    /// <param name="_attaqueReussie">Whether the attack was successful.</param>
    /// <param name="descriptionAttack">Description of the attack.</param>
    public IEnumerator Generate_Text_Attack_Sequence(int _visee, int _degat, bool _attaqueReussie, string descriptionAttack)
    {
        // Prepare the UI for loading
        bool _combatFini = false;
        Show_Combat_Loading_Screen(true);
        Show_More_Explication_Dice(false);
        Update_Text(_attaqueReussie ? _language.GetText("_combat_attaque_success") : _language.GetText("_combat_attaque_rate"));

        // Retrieve necessary information
        PlayerData _hero = _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;
        EnemyData _enemy = _selected.GetComponent<Combat_EnemyInfo>().m_EnemyLinked;

        // Build the prompt
        var value = new Dictionary<string, string>
    {
        {"heroName", _hero.playerName },
        {"heroRace", _hero.playerRace},
        {"heroClass", _hero.playerClass},
        {"heroArme", _hero.playerArme},
        {"heroDescription", _hero.playerDescription},
        {"heroSante", _hero.playerHealth },
        {"enemyName", _enemy._name },
        {"enemyDescription", _enemy._description },
        {"enemySante", _enemy._health.ToString() },
        {"descriptionAttaque", descriptionAttack},
        { "degat", _degat.ToString()}
    };

        string prompt = _language.GetPrompt("_combat_ecriture_attaque", value);

        // If the attack was successful:
        if (_attaqueReussie)
        {
            // Subtract damage from the enemy's health
            _enemy._health -= _degat;

            // MODIF TEST
            //_enemy._health = 0;

            // Build the rest of the prompt based on the enemy's health
            if (_enemy._health <= 0)
            {
                _enemy._isDead = true; // The enemy is dead
                _enemy._ui.GetComponent<Combat_EnemyInfo>().m_CorruptionIcon.SetActive(false);

                // Remove the enemy from the linked list
                _charactersTour.Remove(_selected);

                // Modify the enemies array by removing the dead enemy
                _enemies = _enemies.Where(enemy => enemy != _enemy).ToArray();
            }

            if (_enemy._isDead)
            {
                prompt += _language.GetPrompt("_combat_ecriture_attaque_2", value);
                if (_enemies.Length > 0)
                {
                    value.Add("count", _enemies.Length.ToString());
                    prompt += _language.GetPrompt("_combat_ecriture_attaque_3", value);
                }
                else
                {
                    prompt += _language.GetText("_combat_ecriture_attaque_4");
                    _combatFini = true;
                }
            }
            else
            {
                prompt += _language.GetPrompt("_combat_ecriture_attaque_5", value);
            }
        }
        else
        {
            // If the attack failed
            prompt += _language.GetPrompt("_combat_ecriture_attaque_6", value);
        }

        // Generate the story continuation
        bool done = false;
        string _history = "";
        string _summary = "";
        while (!done)
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Combat_narration, prompt, (response) => { text_Genere = response; }));
            done = Parser.COMBAT_SEQUENCE(text_Genere, out _history, out _summary);
            if (!done)
            {
                yield return new WaitForSeconds(1);
            }
        }

        // Update the narration
        Update_Narration_Text(_history);
        Show_Text_Narration(true);

        // Update the enemy's information
        _selected.GetComponent<Combat_EnemyInfo>().UpdateInformation();

        // Display the continue button
        Afficher_Txt_Action(false);
        Show_Combat_Loading_Screen(false);

        if (!_combatFini)
        {
            Show_Continue_Narration_Button(true);
            // End of turn
        }
        else
        {
            Show_Continue_Combat_Finished_Button(true);
            _herosWon = true;
            // End of combat
        }
    }

    /// <summary>
    /// Writes the continuation of the story for a player's healing action. Updates the UI and processes the healing result.
    /// </summary>
    /// <param name="_visee">Precision value of the healing.</param>
    /// <param name="_degat">Healing value.</param>
    /// <param name="_soinReussie">Whether the healing was successful.</param>
    /// <param name="descriptionSoin">Description of the healing.</param>
    public IEnumerator Generate_Text_Health_Sequence(int _visee, int _degat, bool _soinReussie, string descriptionSoin)
    {
        // Prepare the UI for loading
        Show_Combat_Loading_Screen(true);
        Show_More_Explication_Dice(false);
        Update_Text(_soinReussie ? _language.GetText("_combat_soin_success") : _language.GetText("_combat_soin_rate"));

        // Retrieve necessary information
        PlayerData _hero = _currentCharacter.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;
        PlayerData _selectedHero = _selected.GetComponent<Combat_PlayerInfo>().m_PlayersLinked;

        // Build the prompt
        var value = new Dictionary<string, string>
    {
        {"heroName", _hero.playerName },
        {"heroRace", _hero.playerRace},
        {"heroClass", _hero.playerClass},
        {"heroArme", _hero.playerArme},
        {"heroDescription", _hero.playerDescription},
        {"heroSante", _hero.playerHealth },
        {"selectedHeroName", _selectedHero.playerName },
        {"selectedHeroRace", _selectedHero.playerRace },
        {"selectedHeroClasse", _selectedHero.playerClass },
        {"selectedHeroDescription", _selectedHero.playerDescription },
        {"selectedHeroSante", _selectedHero.playerHealth},
        {"selectedHeroSanteNbr", _selectedHero.playerHealthNbr.ToString()},
        {"selectedHeroSanteNbrnew", (_selectedHero.playerHealthNbr + _degat).ToString()},
        {"descriptionSoin", descriptionSoin},
        { "degat", _degat.ToString()}
    };

        string prompt = _language.GetPrompt("_combat_ecriture_soin", value);

        // If the healing was not successful:
        if (!_soinReussie)
        {
            prompt += _language.GetPrompt("_combat_ecriture_soin_2", value);
        }

        // Generate the story continuation
        bool done = false;
        string _history = "";
        string _summary = "";
        while (!done)
        {
            string text_Genere = "";
            yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.Combat_Narration_Soin, prompt, (response) => { text_Genere = response; }));
            done = Parser.COMBAT_SEQUENCE(text_Genere, out _history, out _summary);
            if (!done)
            {
                yield return new WaitForSeconds(1);
            }
        }

        // Update the health of the healed player
        if (_soinReussie)
        {
            if (_selectedHero.playerHealthNbr < 20)
            {
                _selectedHero.playerHealth = _summary;
            }
            _selectedHero.SetHealthNbr(_selectedHero.playerHealthNbr + _degat);

            // Do not set health above 20
            if (_selectedHero.playerHealthNbr > 20)
            {
                _selectedHero.SetHealthNbr(20);
            }
        }

        // Update the narration
        Update_Narration_Text(_history);
        Show_Text_Narration(true);

        // Update the healed player's information
        _selected.GetComponent<Combat_PlayerInfo>().UpdateInformation();

        // Display the continue button
        Afficher_Txt_Action(false);
        Show_Combat_Loading_Screen(false);
        Show_Continue_Narration_Button(true);

        // End of turn
    }
    #endregion

    #region UI
    /// <summary>
    /// Updates the narration text in the UI.
    /// </summary>
    /// <param name="text">The text to display in the narration.</param>
    public void Update_Narration_Text(string text)
    {
        _gm._UI_Manager.UpdateNarrationText(text);
    }

    /// <summary>
    /// Displays or hides the narration text in the UI.
    /// </summary>
    /// <param name="tf">True to display the narration text, false to hide it.</param>
    public void Show_Text_Narration(bool tf)
    {
        _gm._UI_Manager.AfficherNarrationText(tf);
    }

    /// <summary>
    /// Updates the "Au Tour De" text in the UI.
    /// </summary>
    /// <param name="txt">The text to display.</param>
    public void Update_Informations_Text(string txt)
    {
        _gm._UI_Manager.UpdateAuTourde(txt);
    }

    /// <summary>
    /// Updates the main explanation text in the UI.
    /// </summary>
    /// <param name="txt">The text to display.</param>
    public void Update_Text(string txt)
    {
        _gm._UI_Manager.UpdateExplicationText(txt);
    }

    /// <summary>
    /// Displays or hides the continue narration button in the UI.
    /// </summary>
    /// <param name="tf">True to display the button, false to hide it.</param>
    public void Show_Continue_Narration_Button(bool tf)
    {
        _gm._UI_Manager.AfficherButtonContinuerNarration(tf);
    }

    /// <summary>
    /// Displays or hides the change attack/heal button in the UI.
    /// </summary>
    /// <param name="tf">True to display the button, false to hide it.</param>
    public void Show_Change_Heal_Attack_Button(bool tf)
    {
        _gm._UI_Manager.AfficherButtonChangerSoinAttack(tf);
    }

    /// <summary>
    /// Displays or hides the send prompt button in the UI.
    /// </summary>
    /// <param name="tf">True to display the button, false to hide it.</param>
    public void Show_Prompt_Button(bool tf)
    {
        _gm._UI_Manager.AfficherSendPrompt(tf);
    }

    /// <summary>
    /// Displays or hides the "Au Tour De" explanation in the UI.
    /// </summary>
    /// <param name="tf">True to display the explanation, false to hide it.</param>
    public void Show_Explications_Characters_Turn(bool tf)
    {
        _gm._UI_Manager.AfficherAuTourDeExplication(tf);
    }

    /// <summary>
    /// Displays or hides the continue combat finished button in the UI.
    /// </summary>
    /// <param name="tf">True to display the button, false to hide it.</param>
    public void Show_Continue_Combat_Finished_Button(bool tf)
    {
        _gm._UI_Manager.AfficherButtonContinuerCombatFini(tf);
    }

    /// <summary>
    /// Displays or hides the close combat button in the UI.
    /// </summary>
    /// <param name="tf">True to display the button, false to hide it.</param>
    public void Show_Button_Close_Combat(bool tf)
    {
        _gm._UI_Manager.AfficherButtonContinuerCloseCombat(tf);
    }

    /// <summary>
    /// Displays or hides the combat loading icon in the UI.
    /// </summary>
    /// <param name="tf">True to display the icon, false to hide it.</param>
    public void Show_Loading_Icon(bool tf)
    {
        _gm._UI_Manager.AfficherIconeChargementCombat(tf);
    }

    /// <summary>
    /// Displays or hides the incoherence text in the UI.
    /// </summary>
    /// <param name="tf">True to display the text, false to hide it.</param>
    public void Show_Unvalidation_Text(bool tf)
    {
        _gm._UI_Manager.AfficherIncoherence(tf);
    }

    /// <summary>
    /// Updates the incoherence text in the UI.
    /// </summary>
    /// <param name="txt">The text to display.</param>
    public void Update_Combat_Incoherence_Text(string txt)
    {
        _gm._UI_Manager.UpdateTextIncoherenceCombat(txt);
    }

    /// <summary>
    /// Displays or hides the combat loading screen in the UI.
    /// </summary>
    /// <param name="tf">True to display the screen, false to hide it.</param>
    public void Show_Combat_Loading_Screen(bool tf)
    {
        _gm._UI_Manager.Show_Combat_Loading_Screen(tf);
    }

    /// <summary>
    /// Displays or hides the more explanation dice text in the UI.
    /// </summary>
    /// <param name="tf">True to display the text, false to hide it.</param>
    public void Show_More_Explication_Dice(bool tf)
    {
        _gm._UI_Manager.Show_More_Explication_Dice(tf);
    }

    /// <summary>
    /// Updates the more explanation dice text in the UI.
    /// </summary>
    /// <param name="txt">The text to display.</param>
    public void Update_MoreExplicationDice(string txt)
    {
        _gm._UI_Manager.Update_MoreExplicationDice(txt);
    }

    /// <summary>
    /// Displays or hides the action text in the UI.
    /// </summary>
    /// <param name="tf">True to display the text, false to hide it.</param>
    public void Afficher_Txt_Action(bool tf)
    {
        _gm._UI_Manager.Show_Action_Text(tf);
    }

    /// <summary>
    /// Updates the action text in the UI.
    /// </summary>
    /// <param name="txt">The text to display.</param>
    public void Update_Txt_Action(string txt)
    {
        _gm._UI_Manager.Update_Txt_Action(txt);
    }

    /// <summary>
    /// Updates the placeholder prompt text in the UI.
    /// </summary>
    /// <param name="txt">The text to display.</param>
    public void Update_Txt_PlaceHolderPrompt(string txt)
    {
        _gm._UI_Manager.Update_Txt_PlaceHolderPrompt(txt);
    }

    /// <summary>
    /// Displays or hides the corruption canvas in the UI.
    /// </summary>
    /// <param name="tf">True to display the canvas, false to hide it.</param>
    public void Show_Corruption_Canvas(bool tf)
    {
        _gm._UI_Manager.Show_Corruption_Canvas(tf);
    }
    #endregion

    #region Buttons
    /// <summary>
    /// Handles the click event for the continue narration button. Updates the UI and proceeds to the next character's turn.
    /// </summary>
    public void OnClickButtonContinuerNarration()
    {
        // Hide the UI
        Show_Continue_Narration_Button(false);
        Show_Text_Narration(false);
        Show_Change_Heal_Attack_Button(false);
        _currentCharacter.m_IsPlaying = false;

        if (_currentCharacter.GetComponent<Combat_EnemyInfo>() != null)
        {
            _currentCharacter.GetComponent<Combat_EnemyInfo>().UpdateStrategie(false);
        }

        // Move to the next character's turn in the list
        if (_currentCharacterIndex == _charactersTour.Count - 1)
        {
            _currentCharacterIndex = 0;
            _nodeCurrentCharacter = _charactersTour.First;
            _isPlayerPlaying = true;
        }
        else
        {
            _currentCharacterIndex++;
            _nodeCurrentCharacter = _nodeCurrentCharacter.Next;
            _isPlayerPlaying = _currentCharacterIndex < _players.Length; // Check if it's the player's or enemy's turn
        }

        // Update the current character
        _currentCharacter = _nodeCurrentCharacter.Value;
        _currentCharacter.m_IsPlaying = true;
        _selected = null;

        // If it's the player's turn, display the choice between attack and heal, and update the UI accordingly
        if (_isPlayerPlaying)
        {
            Combat_Iniatilise_Player_Sequence();
        }
        // If it's the enemy's turn, start the enemy attack sequence
        else
        {
            // First, check if the enemy is corrupted
            if (_currentCharacter.GetComponent<Combat_EnemyInfo>()._corruptionToursRestant > 0)
            {
                _currentCharacter.GetComponent<Combat_EnemyInfo>().UpdateCorruption();
                OnClickButtonContinuerNarration();
            }
            else
            {
                // Start the enemy's turn
                _currentCharacter.GetComponent<Combat_EnemyInfo>().m_CorruptionIcon.SetActive(false);
                Combat_Enemy_Sequence_Initialise();
            }
        }
    }

    /// <summary>
    /// Changes the color of the send prompt button based on whether it can be clicked.
    /// </summary>
    /// <param name="tf">True to enable the button, false to disable it.</param>
    public void TurnSendPromptButton(bool tf)
    {
        // Should we change the color of the send button?
        if (!tf)
        {
            _gm._UI_Manager.m_ButtonSendPrompt.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1);
        }
        else
        {
            _gm._UI_Manager.m_ButtonSendPrompt.GetComponent<Image>().color = Color.white;
        }
    }

    /// <summary>
    /// Handles the click event for the send button prompt input. Validates the input and processes the action.
    /// </summary>
    public void OnClickSendButtonPromptInput()
    {
        Show_Change_Heal_Attack_Button(false);

        // Check if the send prompt button is white (enabled)
        if (_gm._UI_Manager.m_ButtonSendPrompt.GetComponent<Image>().color == Color.white)
        {
            _currentCharacter.GetComponent<Combat_PlayerInfo>().CanUseInventory(false);

            // Launch the coherence check prompt
            // Make all enemies non-selectable
            foreach (EnemyData _enemy in _enemies)
            {
                _enemy._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = false;
            }
            foreach (GameObject _pl in _HerosGameobject)
            {
                if (_pl.GetComponent<Combat_PlayerInfo>()._isDead || !_pl.activeSelf) { continue; }
                _pl.GetComponent<Combat_PlayerInfo>().m_IsSelectable = false;
            }

            if (_isAttack)
            {
                // Launch the coherence check prompt for combat
                StartCoroutine(LLM_Validation_Player_Attack_Input());
            }
            else
            {
                // Launch the coherence check prompt for healing
                StartCoroutine(LLM_Validation_Player_Health_Input());
            }
        }
    }

    /// <summary>
    /// Handles the trigger enter event for the send button. Displays instructions if the button is not clickable.
    /// </summary>
    public void OnTriggerEnterSendButton()
    {
        // If the prompt cannot be sent
        if (_gm._UI_Manager.m_ButtonSendPrompt.GetComponent<Image>().color != Color.white)
        {
            // Display instructions indicating that a target must be selected
            _gm._UI_Manager.Show_More_Explication_SenddingButton(true);
        }
    }

    /// <summary>
    /// Handles the trigger exit event for the send button. Hides additional instructions.
    /// </summary>
    public void OnTriggerExitSendButton()
    {
        _gm._UI_Manager.Show_More_Explication_SenddingButton(false);
    }

    /// <summary>
    /// Handles the click event for the combat finished button. Updates the UI and ends the combat.
    /// </summary>
    public void OnClickButtonCombatFini()
    {
        // Hide the UI
        Show_Continue_Combat_Finished_Button(false);
        Show_Text_Narration(false);
        Show_Combat_Loading_Screen(true);

        if (_herosWon)
        {
            StartCoroutine(HappyEnding());
        }
        else
        {
            SadEnding();
        }
    }


    /// <summary>
    /// Handles the click event for the continue close combat button. Updates the UI and resets the combat state.
    /// </summary>
    public void OnClickButtonContinuerCloseCombat()
    {
        // Hide the UI
        Show_Button_Close_Combat(false);
        Show_Text_Narration(true);
        Show_Continue_Narration_Button(true);
        Reset();
        _gm._UI_Manager.ChangePositionOptionCanvas(false);
        _gm._UI_Manager.m_CanvasCombat.SetActive(false);

        if (_gm._historyManager._currentRoom == 2)
        {
            StartCoroutine(_gm._historyManager.GoToNextRoom());
        }
    }
    #endregion

    #region Fin du combat
    /// <summary>
    /// Handles the happy ending scenario when heroes win the combat. Updates the UI and generates the next part of the story.
    /// </summary>
    public IEnumerator HappyEnding() // Heroes have won :)
    {
        // Build the prompt
        _group.RemoveEnemyNearby();

        var value = new Dictionary<string, string>
    {
        {"theme", _gm._memoryManager.GetTheme()},
        {"synopsys", _gm._memoryManager._synopsisResumed},
        {"environnement", _gm._memoryManager.GetMemoryRoom(_gm._historyManager._currentRoom)},
        {"groupe", _group.GetResume_Names()},
        {"position", _group.GetResume_LastPosition()},
        {"vision", _group.GetResume_InVision()},
        {"historique", $"{_group._penultimatePenultimatePhaseResume} ; {_group._penultimatePhaseResume}"}
    };

        string prompt_Free = _language.GetPrompt("_combat_prompt_happy", value);

        string _ennemisName = "";
        foreach (EnemyData _enemy in _enemiesInitial)
        {
            _ennemisName += _enemy._name + ", ";
        }

        value.Add("ennemisName", _ennemisName);
        value.Add("ennemisCount", _enemiesInitial.Length.ToString());
        value.Add("groupName", _group.m_GroupeName);

        prompt_Free += _language.GetPrompt("_combat_prompt_happy_2", value);

        _group.AddHistory(TextType.Event, _language.GetPrompt("_combat_prompt_happy_3", value));

        if (_group.m_PlayerCount != _players.Length)
        {
            value.Add("heroDead", GetNameHeroDead());
            // Some heroes are dead
            prompt_Free += _language.GetPrompt("_combat_prompt_happy_4", value);
        }
        prompt_Free += "\"\n}";

        // Send a prompt to generate the continuation
        yield return StartCoroutine(_gm._free.Generate(prompt_Free, _group.m_GroupIndex));

        // Remove characters from the game:
        if (_playerDead.Count > 0)
        {
            foreach (PlayerData _heroDead in _playerDead)
            {
                _heroDead.SetDead();
                // Transfer Key
                if (_heroDead.playerInventory_Keys > 0)
                {
                    _players[0].playerInventory_Keys += _heroDead.playerInventory_Keys;
                    _heroDead.playerInventory_Keys = 0;
                }
            }

            _gm._groupsManager.PlayerHasMoved(_playerDead[0].playerIndex, true, _players[0].playerIndex);
            _gm._mapManager.rebuildVector(new Vector2Int(0, 0));
        }

        // Display the continue button
        Show_Combat_Loading_Screen(false);
        Show_Text_Narration(true);
        Update_Narration_Text(_group._lastPhase);
        Show_Button_Close_Combat(true);
    }

    /// <summary>
    /// Handles the sad ending scenario when heroes lose the combat. Updates the UI and generates the next part of the story.
    /// </summary>
    public IEnumerator SadEnding() // Heroes have lost and are all dead :(
    {
        if (_gm._groupsManager.m_Groups.Length == 1) // All heroes in the game are dead, so we start the end.
        {
            string _ennemisName = "";
            foreach (EnemyData _enemy in _enemiesInitial)
            {
                _ennemisName += _enemy._name + ", ";
            }
            var value = new Dictionary<string, string>
        {
            {"nameGroup", _group.m_GroupeName},
            {"ennemisName", _ennemisName}
        };

            string _summary = _language.GetPrompt("_combat_prompt_sad_0", value);
            StartCoroutine(_gm._historyManager.GenerateBadEnd(_summary));
            yield break;
        }
        else
        {
            var value = new Dictionary<string, string>
        {
            {"theme", _gm._memoryManager.GetTheme()},
            {"synopsys", _gm._memoryManager._synopsisResumed},
            {"environnement", _gm._memoryManager.GetMemoryRoom(_gm._historyManager._currentRoom)},
            {"groupe", _group.GetResume_Names()},
            {"historique", $"{_group._penultimatePenultimatePhaseResume} ; {_group._penultimatePhaseResume}"}
        };

            // Not all groups are dead, so we simply end this combat.

            // Build the prompt
            _group.RemoveEnemyNearby();

            string prompt_Free = _language.GetPrompt("_combat_prompt_sad_1", value);

            string _ennemisName = "";
            foreach (EnemyData _enemy in _enemiesInitial)
            {
                _ennemisName += _enemy._name + ", ";
            }

            value.Add("groupName", _group.m_GroupeName);
            value.Add("ennemiLenght", _enemiesInitial.Length.ToString());
            value.Add("enemiName", _ennemisName);

            prompt_Free += _language.GetPrompt("_combat_prompt_sad_2", value);

            prompt_Free += "\"\n}";

            // Send a prompt to generate the continuation
            yield return StartCoroutine(_gm._free.Generate(prompt_Free, _group.m_GroupIndex));
            Update_Narration_Text(_group._lastPhase);

            value.Add("allgroupeName", _group.GetAllNames());
            value.Add("playerCount", _group.m_PlayerCount.ToString());

            string names = _group.m_PlayerCount > 1 ? _language.GetPrompt("_combat_prompt_sad_3", value) : _language.GetPrompt("_combat_prompt_sad_4", value);

            // Remove characters from the game:
            int keyToAdd = 0;
            foreach (PlayerData _heroDead in _playerDead)
            {
                _heroDead.SetDead();
                if (_heroDead.playerInventory_Keys > 0)
                {
                    keyToAdd += _heroDead.playerInventory_Keys;
                    _heroDead.playerInventory_Keys = 0;
                }
            }

            int indexOtherPlayer = 0;
            foreach (Group group in _gm._groupsManager.m_Groups)
            {
                if (group != _group)
                {
                    indexOtherPlayer = group.m_Players[0].playerIndex;
                    group.m_Players[0].playerInventory_Keys += keyToAdd;
                    break;
                }
            }

            _gm._groupsManager.PlayerHasMoved(_playerDead[0].playerIndex, true, indexOtherPlayer);
            _gm._mapManager.rebuildVector(new Vector2Int(0, 0));

            foreach (Group group in _gm._groupsManager.m_Groups)
            {
                if (!group.isDead)
                {
                    group.AddHistory(TextType.Event, $"{names}");
                }
            }

            // Display the continue button
            Show_Combat_Loading_Screen(false);
            Show_Text_Narration(true);
            Show_Button_Close_Combat(true);
        }
    }
    #endregion

    #region Others
    /// <summary>
    /// Remaps a value from one range to another.
    /// </summary>
    /// <param name="value">The value to remap.</param>
    /// <param name="fromMin">The minimum value of the original range.</param>
    /// <param name="fromMax">The maximum value of the original range.</param>
    /// <param name="toMin">The minimum value of the target range.</param>
    /// <param name="toMax">The maximum value of the target range.</param>
    /// <returns>The remapped value.</returns>
    public int Remap_Range_Value_Health(int value, int fromMin, int fromMax, int toMin, int toMax)
    {
        return (int)Mathf.Round((float)(value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin);
    }

    /// <summary>
    /// Retrieves the names of dead heroes.
    /// </summary>
    /// <returns>A string containing the names of dead heroes.</returns>
    public string GetNameHeroDead()
    {
        string _herosMorts = "";
        foreach (PlayerData _player in _players)
        {
            bool _isInGroup = false;
            foreach (PlayerData _playerGroup in _group.m_Players)
            {
                if (_player.playerName == _playerGroup.playerName)
                {
                    _isInGroup = true;
                    break;
                }
            }
            if (!_isInGroup)
            {
                _herosMorts += _player.playerName + ", ";
            }
        }
        return _herosMorts;
    }

    /// <summary>
    /// Prepares the UI for choosing an enemy to corrupt.
    /// </summary>
    public void Choice_Enemy_For_Corruption()
    {
        Show_Prompt_Button(false);
        _isCorruption = true;

        // For the duration of the corruption choice, we will reset all enemies to clear
        foreach (EnemyData _enemy in _enemies)
        {
            _enemy._ui.GetComponent<Combat_EnemyInfo>().ShowShadow(false);
            _enemy._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = false;
        }
    }

    /// <summary>
    /// Validates the enemy chosen for corruption and updates the UI.
    /// </summary>
    public void Validate_Enemy_Corruption()
    {
        _isCorruption = false;
        foreach (EnemyData _enemy in _enemies)
        {
            if (_selected != _enemy._ui.GetComponent<Combat_EnemyInfo>())
            {
                _enemy._ui.GetComponent<Combat_EnemyInfo>().ShowShadow(false);
                _enemy._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = true;
            }
            else
            {
                _enemy._ui.GetComponent<Combat_EnemyInfo>().ShowShadow(true);
                _enemy._ui.GetComponent<Combat_EnemyInfo>().m_IsSelectable = true;
            }
        }
        Show_Corruption_Canvas(false);
        _currentCharacter.GetComponent<Combat_PlayerInfo>().CanUseInventory(true);
        _currentCharacter.GetComponent<Combat_PlayerInfo>().UpdateInventory();
        _currentCharacter.GetComponent<Combat_PlayerInfo>().UpdateInformation();

        Show_Prompt_Button(true);
    }
    #endregion

}
