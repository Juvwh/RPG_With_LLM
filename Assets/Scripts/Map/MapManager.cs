using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using static Events;
using static Jobs;
using static AgentManager;
using System;
using System.Runtime.ConstrainedExecution;
using static MapManager;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;



public class MapManager : MonoBehaviour
{

    #region Managers
    [Header("Managers")]
    public GameManager _gm;
    public CharactersManagers _charactersManagers;
    #endregion

    #region Max Limits
    [Header("Max Coffre")]
    public int maxCoffre = 3;
    public int maxEnnemy = 2;
    public int bonusCoffre = 0;
    public int bonusEnnemy = 0;
    #endregion

    #region State
    [Header("Etat")]
    public bool mapGenerated = false;
    #endregion

    #region Coordinates
    [Header("Coordonnées")]
    public Dictionary<(int x, int y), Coord> m_CoordSet = new Dictionary<(int, int), Coord>();
    public Dictionary<(int x, int y), Coord> m_WallCoordSet = new Dictionary<(int, int), Coord>();
    #endregion

    #region Tilemap
    [Header("Tuto Tilemap")]
    public TilemapVisualizer m_Visualizer;
    public SimpleRandomWalkDungeonGenerator m_ProceduralALgo;
    public CorridorFirstDungeonGenerator m_CorridorFirst;
    [SerializeField]
    int _scale = 30;
    float _scrollViewSize = 650.0f;
    public Vector2Int _eventTilemapOffset = new Vector2Int(32, 18);
    Vector2Int _scrollOfset = new Vector2Int(0, 0);
    #endregion

    #region Arrows
    [Header("Arrow")]
    private bool m_ArrowTrigger = false;
    public GameObject m_ArrowUp;
    public GameObject m_ArrowDown;
    public GameObject m_ArrowLeft;
    public GameObject m_ArrowRight;
    public int maxScroll = 6;
    #endregion

    #region Tile Click
    [Header("Tile Click")]
    bool _canMove = true;
    Coord _lastCoordClicked;
    HashSet<Vector2Int> _coordsAccessiblesVector = new HashSet<Vector2Int>();
    Vector2Int _vector = new Vector2Int(0, 0);
    #endregion

    #region Door
    [Header("Porte")]
    public Coord _coordPorte;
    #endregion

    #region Special Move
    [Header("SpecialMove")]
    int _playerSpecialMove = -1;
    int _CurrentRoom;
    #endregion

    #region Methods Basics
    public void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        _charactersManagers = _gm._charactersManagers;
    }
    #endregion

    #region Move Map
    //J'ai simplifié la façon dont on déplace la carte, il y avait beaucoup de code répétitif
    public void _enterTriggerArrowButton(string direction)
    {
        m_ArrowTrigger = true;
        int amountMove = 1;
        Vector2Int _currentMove = new Vector2Int(0, 0);
        switch (direction)
        {
            case "up":
                _currentMove.y = -amountMove;
                break;
            case "down":
                _currentMove.y = amountMove;
                break;
            case "left":
                _currentMove.x = amountMove;
                break;
            case "right":
                _currentMove.x -= amountMove;
                break;
            default:
                break;
        }
        StartCoroutine(moveMapView(_currentMove));
    }
    private IEnumerator moveMapView(Vector2Int _currentMove)
    {
        while (m_ArrowTrigger)
        {
            if (_currentMove.x == -1)
            {
                if (_scrollOfset.x <= -maxScroll)
                {
                    m_ArrowRight.SetActive(false);
                    _exitTriggerArrowButton();
                }
                else if (_scrollOfset.x < maxScroll)
                {
                    m_ArrowLeft.SetActive(true);
                }
            }
            else if (_currentMove.x == 1)
            {
                if (_scrollOfset.x >= maxScroll)
                {
                    m_ArrowLeft.SetActive(false);
                    _exitTriggerArrowButton();
                }
                else if (_scrollOfset.x > -maxScroll)
                {
                    m_ArrowRight.SetActive(true);
                }
            }
            else if (_currentMove.y == -1)
            {
                if (_scrollOfset.y <= -maxScroll)
                {
                    m_ArrowUp.SetActive(false);
                    _exitTriggerArrowButton();
                }
                else if (_scrollOfset.y < maxScroll)
                {
                    m_ArrowDown.SetActive(true);
                }
            }
            else if (_currentMove.y == 1)
            {
                if (_scrollOfset.y >= maxScroll)
                {
                    m_ArrowDown.SetActive(false);
                    _exitTriggerArrowButton();
                }
                else if (_scrollOfset.y > -maxScroll)
                {
                    m_ArrowUp.SetActive(true);
                }
            }

            if (m_ArrowTrigger)
            {
                _vector.x += _currentMove.x;
                _vector.y += _currentMove.y;
                _scrollOfset += _currentMove;
                rebuildVector(_currentMove);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    public void _exitTriggerArrowButton()
    {
        m_ArrowTrigger = false;
    }
    #endregion

    #region others Methods

    public IEnumerator GenerateMap(string MemoryRoom, int RoomIndex)
    {
        // RESET
        _vector = new Vector2Int(0, 0);// reset offset
        m_CoordSet = new Dictionary<(int, int), Coord>();
        m_WallCoordSet = new Dictionary<(int, int), Coord>();
        m_Visualizer.Clear();//afface previous map if any
        m_Visualizer.eventTilemap.ClearAllTiles();// efface event

        for (int i = 0; i < _gm._charactersManagers._nbrOfCharacters; i++)
        {
            _charactersManagers.SetHasMoved(false, i);
            _charactersManagers.SetHasMoved(false, i);
            _charactersManagers.SetHasMoved(false, i);
            _charactersManagers.SetHasMoved(false, i);
        }

        _CurrentRoom = RoomIndex;
        bool _hall = false;
        bool _grand = false;
        bool _couloir = false;
        if (RoomIndex <= 1)
        {
            string mapTags = "";
            bool isDone = false;

            while(!isDone)
            {
                yield return StartCoroutine(_gm._agent.Generate_Text(_gm.m_currentModel, Job.GenererFormeMap, MemoryRoom, (response) => { mapTags = response; }));
                isDone = Parser.MAP_TAG(mapTags, out _hall, out _couloir, out _grand);

                if (isDone)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            
            if (_couloir && _hall)
            {
                bonusCoffre = 0;
                bonusEnnemy = 0;
                ConfigureCorridor(false, 15, 1, 25, 15, 1.0f, false);

            }
            else if (_couloir)
            {
                bonusCoffre = 0;
                bonusEnnemy = 0;
                ConfigureCorridor(true, 6, 3, 15, 7, 1.0f, false);

            }
            else if (_hall && _grand)
            {
                bonusCoffre = 1;
                bonusEnnemy = 1;
                ConfigureProceduralAglo(22, 25);
            }
            else
            {
                bonusCoffre = 0;
                bonusEnnemy = 0;
                ConfigureProceduralAglo(19, 20);
            }
        }
        else
        {
            bonusCoffre = 0;
            bonusEnnemy = 0;
            ConfigureProceduralAglo(10, 20);
        }
        _gm._groupsManager.GroupPlayers();
        mapGenerated = true;

    }
    public void _onGeneration()
    {
        ClearAllTileSet();
        foreach (Vector2Int vect in m_ProceduralALgo.RunProceduralGeneration())
        {
            Coord coord = new Coord(vect.x, vect.y, EventCoord.Nothing, this);
            m_CoordSet[(coord.x, coord.y)] = coord;
        }
        SetHerosPositions();


        //Pour chaque héros présent :
        for (int i = 0; i < _charactersManagers.GetNumberOfHeroes(); i++)
        {
            Coord _coord = _charactersManagers.GetPosition(i);
            int _vision = _charactersManagers.GetVision(i);
            visitFloorTile(_coord, _vision);
        }


        // salle 1 et 3 aurons 2x plus de combat que salle 2
        if (_CurrentRoom == 0)
        {
            maxCoffre = 4 + bonusCoffre;
            maxEnnemy = 2 + bonusEnnemy;
            Generate_Events_On_Map(0.3f, 0.5f, 1.0f, 1.2f);
        }
        else if (_CurrentRoom == 1)// mais salle 2 a tjs autant de coffre (salle enigme)
        {
            maxCoffre = 2 + bonusCoffre;
            maxEnnemy = 3;
            Generate_Events_On_Map(0.3f, 0.5f, 1.0f, 2.6f);
        }
        else if (_CurrentRoom == 2)// BOSS Room (petite-moyenne pièce avec 1 combat)
        {
            maxCoffre = 0;
            maxEnnemy = 0;
            Generate_Events_On_Map(0.3f, 0.5f, 1.0f, 1.2f);
        }

        //Create WallCoordSet

        foreach (var coord in m_CoordSet.Values)
        {
            (int, int)[] neighbours = {(coord.x, coord.y + 1),  (coord.x, coord.y - 1), (coord.x + 1, coord.y), (coord.x - 1, coord.y),
                                       (coord.x+1, coord.y + 1),  (coord.x-1, coord.y - 1), (coord.x + 1, coord.y-1), (coord.x - 1, coord.y+1)};

            foreach ((int, int) neighbour in neighbours)
            {
                if (!m_CoordSet.ContainsKey(neighbour) && !m_WallCoordSet.ContainsKey(neighbour))
                {
                    Coord wallCoord = new Coord(neighbour.Item1, neighbour.Item2, EventCoord.Wall, this);
                    m_WallCoordSet[(wallCoord.x, wallCoord.y)] = wallCoord;
                }
            }
        }

        System.Random random = new System.Random();

        // Tableau de décalages pour vérifier les voisins (haut, bas, droite, gauche)
        (int dx, int dy)[] offsets = { (0, 1), (0, -1), (1, 0), (-1, 0) };

        if(_CurrentRoom != 2) {  // Si c'est la salle 2, on ne met pas de porte.

        while (true) // PAS OUF QUAND MEME
        {
            Coord coord = m_WallCoordSet.Values.ElementAt(random.Next(m_WallCoordSet.Count));

            bool foundValidNeighbor = false;

            // Vérification des voisins
            foreach ((int dx, int dy) in offsets)
            {
                (int nx, int ny) = (coord.x + dx, coord.y + dy);
                if (m_CoordSet.TryGetValue((nx, ny), out var neighbor))
                {
                    neighbor._description = "Porte de sortie de la salle du donjon, fermée à clé. Besoin d'une clé se trouvant dans un coffre ou sur un ennemi.";
                    neighbor._event = EventCoord.DoorLocked;
                    foundValidNeighbor = true;
                }
            }

            if (foundValidNeighbor)
            {
                coord._description = "Porte de sortie de la salle du donjon, fermée à clé. Besoin d'une clé se trouvant dans un coffre ou sur un ennemi.";
                coord._event = EventCoord.DoorLocked;
                _coordPorte = coord;
                break;
            }
        }
        }


        //Pour chaque héros présent :
        for (int i = 0; i < _charactersManagers.GetNumberOfHeroes(); i++)
        {
            Coord _coord = _charactersManagers.GetPosition(i);
            int _vision = _charactersManagers.GetVision(i);
            visitFloorTile(_coord, _vision);
        }

        //Generate_Events_On_Map(0.3f, 0.4f, 1.0f, 1.2f);
        rebuildVector(new Vector2Int(32, 18));
        mapGenerated = true;
    }
    private void SetHerosPositions()
    {
        for (int i = 0; i < _charactersManagers.GetNumberOfHeroes(); i++)
        {
            if (!_charactersManagers.GetAllPlayers()[i].playerIsAlive)
            {
                continue; // Skip if the player is not alive
            }

            Coord _coord;
            if (i == 0) { _coord = m_CoordSet.Get(0, 0); } // Player 1
            else if (i == 1) { _coord = m_CoordSet.Get(0, 1); } // Player 2
            else if (i == 2) { _coord = m_CoordSet.Get(1, 0); } // Player 3
            else { _coord = m_CoordSet.Get(1, 1); } // Player 4

            if (_coord != null)
            {
                _charactersManagers.SetPosition(_coord, i);
                _charactersManagers.SetPreviousCoord(_coord, i);
                switch (i)
                {
                    case 0:
                        _coord.SetHero(Hero.Hero1);
                        break;
                    case 1:
                        _coord.SetHero(Hero.Hero2);
                        break;
                    case 2:
                        _coord.SetHero(Hero.Hero3);
                        break;
                    case 3:
                        _coord.SetHero(Hero.Hero4);
                        break;
                    default:
                        break;
                }
            }
        }

    }
    public void Generate_Events_On_Map(float _pEnemy, float _pLoot, float _dF, float _dF_E)
    {
        //Shuffle--------------------------------------------------
        List<Coord> listCoords = m_CoordSet.Select(kvp => kvp.Value).ToList();
        System.Random random = new System.Random();

        for (int i = listCoords.Count - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            var temp = listCoords[i];
            listCoords[i] = listCoords[randomIndex];
            listCoords[randomIndex] = temp;
        }
        //---------------------------------------------------------

        float probability_Enemy = _pEnemy; // _pEnemy;  // Initial probability for monster
        float probability_Loot = _pLoot + 10;    // Initial probability for loot
        float decayFactor = _dF;            // Exponential decay rate (k)
        float decayFactor_Enemy = _dF_E;    // Exponential decay rate (k)

        int lootCount = 0;
        int enemyCount = 0;

        float proba_loot = probability_Loot * Mathf.Exp(-decayFactor * lootCount);
        float proba_enemy = probability_Enemy * Mathf.Exp(-decayFactor_Enemy * enemyCount);
        float previousLootCount = lootCount;
        float previousEnemyCount = enemyCount;
        bool rajouterEnnemi = false;
        //bool rajoutercoffre = false;
        for (int i = 0; i < listCoords.Count; i++)
        {
            Coord _coord = (Coord)listCoords[i];
            if (_coord._heroOnCoord == Hero.Nothing && !_coord._isVisible)
            {

                if (previousLootCount != lootCount)
                {
                    proba_loot = probability_Loot * Mathf.Exp(-decayFactor * lootCount);
                    previousLootCount = lootCount;
                }

                if (previousEnemyCount != enemyCount)
                {
                    proba_enemy = probability_Enemy * Mathf.Exp(-decayFactor_Enemy * enemyCount);
                    previousEnemyCount = enemyCount;
                }

                // Decide whether to add event true or false
                if ((float)random.NextDouble() < proba_loot && lootCount < maxCoffre)
                {
                    //Rajouter des loots dans le coffre.
                    _coord._description = "Dans ce coffre il y a : ";
                    _coord._event = EventCoord.Loot;


                    int randomNbrLoot = random.Next(3, 5);
                    if (lootCount == 0)
                    {
                        _coord._description = "[KEY]";
                        randomNbrLoot--;
                    }
                    if (lootCount == 1)
                    {
                        _coord._description = "[COMPASS]";
                        randomNbrLoot--;
                    }
                    string[] loots = { "[COINS]", "[HEALTH]", "[ATTACK]", "[SHOES]", "[ARMOR]", "[GLASSES]" };

                    for (int j = 0; j < randomNbrLoot; j++)
                    {
                        int randomLoot = random.Next(0, loots.Length);
                        _coord._description += loots[randomLoot] + " ";
                    }

                    lootCount++;
                }
                else if ((float)random.NextDouble() < proba_enemy && enemyCount < maxEnnemy)
                {
                    _coord._event = EventCoord.Enemy;
                    _coord._description = "Enemy" + enemyCount;
                    enemyCount++;
                }
            }
        }
        if (_CurrentRoom == 2)
        {
            m_CoordSet[(0, -1)]._event = EventCoord.Enemy;//
            m_CoordSet[(0, -1)]._description = "Boss final";//
        }
        //m_CoordSet[(0, -1)]._event = EventCoord.Loot;
        //m_CoordSet[(0, -1)]._description = "Dans ce coffre il y a : [COMPASS] [KEY]";
    }
    public void rebuildVector(Vector2Int _currentMove)
    {
        HashSet<Vector2Int> coords = new HashSet<Vector2Int>();
        HashSet<Vector2Int> wallCoords = new HashSet<Vector2Int>();
        Dictionary<(int x, int y), Coord> m_WallCoordSetVector = new Dictionary<(int x, int y), Coord>();
        foreach (Coord coord in m_CoordSet.Values)
        {
            coord.x_vector += _currentMove.x;
            coord.y_vector += _currentMove.y;
            coords.Add(new Vector2Int(coord.x_vector, coord.y_vector));
        }
        foreach (Coord coord in m_WallCoordSet.Values)
        {
            coord.x_vector += _currentMove.x;
            coord.y_vector += _currentMove.y;
            m_WallCoordSetVector[(coord.x_vector, coord.y_vector)] = coord;
        }
        m_Visualizer.Clear();
        m_Visualizer.eventTilemap.ClearAllTiles();
        m_Visualizer.ClearVision();
        _canMove = true;


        //re positionne
        m_Visualizer.PaintFloorTiles(m_CoordSet, _scale);

        WallGenerator.CreateWalls(coords, m_Visualizer, _scale, m_WallCoordSetVector);


        foreach (var coord in m_CoordSet.Values)
        {
            if (coord._isVisible)
            {
                if (coord._event != EventCoord.Nothing || coord._heroOnCoord != Hero.Nothing)
                {
                    m_Visualizer.PaintEventTile(new Vector2Int(coord.x_vector, coord.y_vector), coord._event, coord._heroOnCoord, _scale);
                }
            }
        }

        ChangeColorHeroTile();


    }
    #region Visit
    public void visitFloorTile(Coord pos, int radius = 3)
    {
        HashSet<Coord> _coordsAccessibles = new HashSet<Coord>();
        _coordsAccessibles = pos.GetNeighbours(radius);
        pos._isVisited = true;
        foreach (Coord coord in _coordsAccessibles)
        {
            if (!coord._isVisible)
            {
                coord._isVisible = true;
            }

        }

        //Si la coord de wallCoordSet est a une distance egale ou plus petite que radius, on la rend visible.
        foreach (Coord coord in m_WallCoordSet.Values)
        {
            if (pos.GetDistance(coord) <= radius)
            {
                coord._isVisible = true;
            }
        }

    }
    #endregion
    #endregion

    #region Little Methods

    public void RemoveHeroIcon(PlayerData playerToRemove)
    {
        Coord coordToRemove = m_CoordSet[(playerToRemove.playerPosition.x, playerToRemove.playerPosition.y)];
        int heroIntRemoved = coordToRemove.HeroToInt(coordToRemove._heroOnCoord);
        coordToRemove._event = EventCoord.Nothing;// remove hero
        coordToRemove._heroOnCoord = Hero.Nothing;

        var keysOfHero = m_CoordSet.Where(kv => kv.Value._heroOnCoord != Hero.Nothing).Select(kv => kv.Key).ToList();

        // change sprite des héro qui on un index > que le héro qui à été suprimé
        foreach (var key in keysOfHero)
        {
            if (heroIntRemoved < m_CoordSet[key].HeroToInt(m_CoordSet[key]._heroOnCoord))
            {
                int newInt = m_CoordSet[key].HeroToInt(m_CoordSet[key]._heroOnCoord) - 1;
                m_CoordSet[key]._heroOnCoord = utils_IntToHero(newInt);
            }

        }
        //rebuildVector(_eventTilemapOffset);
    }
    public Hero utils_IntToHero(int heroInt)
    {
        switch (heroInt)
        {
            case 0:
                return Hero.Hero1;
            case 1:
                return Hero.Hero2;
            case 2:
                return Hero.Hero3;
            case 3:
                return Hero.Hero4;
            default:
                return Hero.Nothing;
        }
    }

    public void DestroyEnemy(string name)
    {
        // Find the enemy to remove
        var keysToRemove = m_CoordSet.Where(kv => kv.Value._description == name).Select(kv => kv.Key).ToList();

        // Remove the matching keys
        foreach (var key in keysToRemove)
        {
            m_CoordSet.Remove(key);
        }
    }

    private void ConfigureCorridor(bool d, int cl, int cc, int i, int w, float r, bool s)
    {
        m_CorridorFirst.dense = d;
        m_CorridorFirst.corridorLength = cl;
        m_CorridorFirst.corridorCount = cc;
        m_CorridorFirst.iterations = i;
        m_CorridorFirst.walkLength = w;
        m_CorridorFirst.roomPercent = r;
        m_CorridorFirst.startRandomlyEachIteration = s;
        _onGeneration();
    }
    private void ConfigureProceduralAglo(int w, int i)
    {
        m_ProceduralALgo.walkLength = w;
        m_ProceduralALgo.iterations = i;
        _onGeneration();

    }
    private void ClearAllTileSet()
    {
        m_Visualizer.ClearVision();
        m_Visualizer.Clear();
        m_Visualizer.eventTilemap.ClearAllTiles();
    }

    #endregion

    #region TileClick
    public void Update()
    { // To remove in prod
        if (Input.GetMouseButtonDown(0))
        {
            DetectTileClick();
        }
    }
    public void ChangeColorHeroTile()
    {
        UnityEngine.Color color = new UnityEngine.Color(1, 1, 1);
        foreach (PlayerData player in _charactersManagers.GetAllPlayers())
        {
            if (_gm._groupsManager.m_CurrentGroupIndexOnDisplay != _gm._groupsManager.GetGroupIndexByPlayer(player) || !player.playerIsReady)
            {
                color = new UnityEngine.Color(0.5f, 0.5f, 0.5f);
            }
            else
            {
                color = new UnityEngine.Color(1, 1, 1);
            }
            Coord pos = player.playerPosition;
            if (pos != null)
            {
                Vector2Int currentTileVector = new Vector2Int(pos.x_vector, pos.y_vector);
                m_Visualizer.ChangeColorTile(currentTileVector, color, _scale);
            }
        }
    }
    public void DetectTileClick()
    {
        // Convert the mouse position to a world position
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Tilemap Event = m_Visualizer.eventTilemap;

        Vector3Int tilePosition = Event.WorldToCell(new Vector3Int((int)mouseWorldPos.x + _scale * _eventTilemapOffset.x, (int)mouseWorldPos.y + _scale * _eventTilemapOffset.y, 0));

        Vector2Int currentTile = new Vector2Int(tilePosition.x - _vector.x - _eventTilemapOffset.x, tilePosition.y - _vector.y - _eventTilemapOffset.y);
        Vector2Int currentTileVector = new Vector2Int(tilePosition.x, tilePosition.y);
        Coord currentCoord = m_CoordSet.Get(currentTile.x, currentTile.y);
        //Test si on clique sur un icone de héro
        if (currentCoord == null)
        {
            _canMove = true;
            m_Visualizer.ClearVision();

            //Si avant on avait cliqué sur un héros : 
            if (_playerSpecialMove != -1)
            {
                _charactersManagers.GetPlayer(_playerSpecialMove)._specialMove = 0;
                _playerSpecialMove = -1;
            }

        }
        else if (currentCoord._heroOnCoord != Hero.Nothing && _canMove)
        {

            int heroIndex = currentCoord.HeroToInt(currentCoord._heroOnCoord);

            //On vérifie qu'on a le droit de déplacer ce joueur
            if (!_charactersManagers.GetPlayer(heroIndex).playerIsReady)
            {
                return;
            }
            else if (_gm._groupsManager.GetGroupIndexOfPlayer(_charactersManagers.GetPlayer(heroIndex).playerIndex) != _gm._groupsManager.m_CurrentGroupIndexOnDisplay)
            {

                _gm._UI_Manager.SwitchGroupCanvas(_gm._groupsManager.GetGroupIndexOfPlayer(heroIndex));
            }

            ///////////////VISUALISATION DE LA VISION DU JOUEUR////////
            /////
            int vision = _charactersManagers.GetVision(currentCoord.HeroToInt(currentCoord._heroOnCoord));

            // Si le joueur a utilisé un item de chaussure, alors il peut se déplacer partout.
            if (_charactersManagers.GetPlayer(heroIndex)._specialMove == 2)
            {
                GenerateAllVisiblesTiles();
            }
            else if (_charactersManagers.GetPlayer(heroIndex)._hasMoved)
            {
                Coord previousCoord = _charactersManagers.GetPreviousCoord(heroIndex);
                GenerateVisionTile(previousCoord, vision);
            }
            else
            {
                GenerateVisionTile(currentCoord, vision);
            }
            _canMove = false;
            _lastCoordClicked = currentCoord;
        }
        else if (_coordsAccessiblesVector.Contains(currentTileVector) && !_canMove)
        {
            currentCoord.SetHero(_lastCoordClicked._heroOnCoord);
            _lastCoordClicked.RemoveHero();
            //VisitFloorTile devra être joué uniquement lorsque le joueur aura fini de bouger et que la phase de jeu aura été enclenchée.
            //visitFloorTile(currentCoord, _charactersManagers.GetVision(currentCoord.HeroToInt(currentCoord._heroOnCoord)));


            int heroIndex = currentCoord.HeroToInt(currentCoord._heroOnCoord);
            if (!_gm._charactersManagers.GetPlayer(heroIndex)._hasMoved) // Si le joueur n'avait pas encore changé de place.
            {
                Coord previousCoord = _charactersManagers.GetPosition(heroIndex);
                _charactersManagers.SetPreviousCoord(previousCoord, heroIndex);
                _charactersManagers.SetPosition(currentCoord, heroIndex);
            }
            else //Si le joueur avait déjà changé de place.
            {
                _charactersManagers.SetPosition(currentCoord, heroIndex);
            }
            if (_playerSpecialMove != -1)
            {
                _charactersManagers.GetPlayer(heroIndex)._specialMove = 2;
                _playerSpecialMove = -1;
            }
            _charactersManagers.SetHasMoved(true, heroIndex);
            _gm._groupsManager.PlayerHasMoved(heroIndex);
            //m_Visualizer devra être joué uniquement lorsque le joueur aura fini de bouger et que la phase de jeu aura été enclenché.
            m_Visualizer.floorTilemap.ClearAllTiles();
            m_Visualizer.PaintFloorTiles(m_CoordSet, _scale);

            foreach (var coord in m_CoordSet.Values)
            {
                if (currentCoord._isVisible)
                {
                    if (currentCoord._event != EventCoord.Nothing || currentCoord._heroOnCoord != Hero.Nothing || currentCoord._event != EventCoord.DoorLocked || currentCoord._event != EventCoord.DoorUnlocked)
                    {
                        m_Visualizer.PaintEventTile(new Vector2Int(currentCoord.x_vector, currentCoord.y_vector), currentCoord._event, currentCoord._heroOnCoord, _scale);
                    }
                }
            }
            rebuildVector(new Vector2Int(0, 0));
            _lastCoordClicked = currentCoord;
            m_Visualizer.ClearVision();
            _canMove = true;

        }
        else
        {
            _canMove = true;
            m_Visualizer.ClearVision();
            _lastCoordClicked = currentCoord;
        }
    }

    public void SimulatetileClickOnHero(int heroIndex)
    {
        Coord currentCoord = _charactersManagers.GetPosition(heroIndex);
        int vision = _charactersManagers.GetVision(heroIndex);
        _canMove = false;
        _lastCoordClicked = currentCoord;
        _playerSpecialMove = heroIndex;
        GenerateAllVisiblesTiles();
    }
    public void GenerateVisionTile(Coord coord, int radius)
    {
        HashSet<Coord> _coordsAccessibles = new HashSet<Coord>();
        _coordsAccessiblesVector = new HashSet<Vector2Int>();
        m_Visualizer.ClearVision();
        _coordsAccessibles = coord.GetNeighbours(radius);
        foreach (Coord _coord in _coordsAccessibles)
        {
            if ((_coord._event == EventCoord.Nothing || _coord._event == EventCoord.DoorLocked || _coord._event == EventCoord.DoorUnlocked) && _coord._heroOnCoord == Hero.Nothing)
            {
                _coordsAccessiblesVector.Add(new Vector2Int(_coord.x_vector, _coord.y_vector));
            }
        }

        m_Visualizer.PaintReachableTiles(_coordsAccessiblesVector, _scale);
    }
    public void GenerateAllVisiblesTiles()
    {
        _coordsAccessiblesVector = new HashSet<Vector2Int>();
        m_Visualizer.ClearVision();
        foreach (Coord coord in m_CoordSet.Values)
        {
            if (coord._isVisible && coord._event == EventCoord.Nothing && coord._heroOnCoord == Hero.Nothing)
            {
                _coordsAccessiblesVector.Add(new Vector2Int(coord.x_vector, coord.y_vector));
            }
        }
        m_Visualizer.PaintReachableTiles(_coordsAccessiblesVector, _scale);
    }
    #endregion



}