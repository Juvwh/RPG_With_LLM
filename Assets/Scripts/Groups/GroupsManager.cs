using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Composition;

using static UI_Manager;


public class GroupsManager : MonoBehaviour
{
    #region Attributs
    public GameManager _GameManager;
    public CharactersManagers _charactersManagers;

    public int m_HerosCount; //Nombre de héros dans le jeu
    public PlayerData[] m_HerosList; //Liste des héros dans l'ordre
    public Group[] m_Groups; //Liste des groupes de héros
    public int m_GroupsCount; //Nombre de sous-groupes
    public composition m_CurrentComposition; //Composition actuelle
    public Group m_currentGroupOnDisplay; //Groupe actuel à jouer
    public int m_CurrentGroupIndexOnDisplay; //Index du groupe actuel


    [Header("Union-Find")]
    private Dictionary<int, int> _parent = new Dictionary<int, int>(); // Union-Find structure (index dans m_HerosList)
    private Dictionary<int, List<PlayerData>> _previousGroups = new Dictionary<int, List<PlayerData>>();
    public int DistanceMaxForUnion = 3;

    [Header("Canvas")]
    public GameObject[] m_CanvasCompositions;
    public GameObject m_CurrentCanvasCompositions;
    #endregion

    #region Methods
    private void Start()
    {
        _charactersManagers = _GameManager._charactersManagers;
        m_HerosList = _charactersManagers.GetAllPlayers();
        m_HerosCount = m_HerosList.Length;
        m_GroupsCount = 1;
        StartCoroutine(AtStart());
    }

    public IEnumerator AtStart()
    {
        Automatic_Launch _automaticLaunch = FindFirstObjectByType<Automatic_Launch>();
        while (!_automaticLaunch.Done)
        {
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < m_HerosCount; i++)
        {
            m_HerosList[i].playerIndex = i;
        }
        CompositionAtStart();
    }
    public void CompositionAtStart()
    {
        if (m_HerosCount == 1)
        {
            m_CurrentComposition = composition.Hero1_1;
            GroupOfOne groupOfOne = this.gameObject.AddComponent<GroupOfOne>();
            groupOfOne.Initialize(m_HerosList[0]);
            groupOfOne.ChangeCanvas(m_CanvasCompositions[0].transform.GetChild(0).gameObject);
            groupOfOne.m_GroupIndex = 0;
            groupOfOne.UpdateImageCanvas();
            m_Groups = new Group[1] { groupOfOne };
            m_CurrentCanvasCompositions = m_CanvasCompositions[0];
            _GameManager._UI_Manager.HideInventaire("BCD");
        }
        else if (m_HerosCount == 2)
        {
            m_CurrentComposition = composition.Hero2_2;
            GroupOfTwo groupOfTwo = this.gameObject.AddComponent<GroupOfTwo>();
            groupOfTwo.Initialize(new PlayerData[2] { m_HerosList[0], m_HerosList[1] });
            groupOfTwo.m_GroupIndex = 0;
            groupOfTwo.ChangeCanvas(m_CanvasCompositions[1].transform.GetChild(0).gameObject);
            groupOfTwo.UpdateImageCanvas();
            m_Groups = new Group[1] { groupOfTwo };
            m_CurrentCanvasCompositions = m_CanvasCompositions[1];
            _GameManager._UI_Manager.HideInventaire("CD");
        }
        else if (m_HerosCount == 3)
        {
            m_CurrentComposition = composition.Hero3_3;
            GroupOfThree groupOfThree = this.gameObject.AddComponent<GroupOfThree>();
            groupOfThree.Initialize(new PlayerData[3] { m_HerosList[0], m_HerosList[1], m_HerosList[2] });
            groupOfThree.m_GroupIndex = 0;
            groupOfThree.ChangeCanvas(m_CanvasCompositions[3].transform.GetChild(0).gameObject);
            groupOfThree.UpdateImageCanvas();
            m_Groups = new Group[1] { groupOfThree };
            m_CurrentCanvasCompositions = m_CanvasCompositions[3];
            _GameManager._UI_Manager.HideInventaire("D");
        }
        else if (m_HerosCount == 4)
        {
            m_CurrentComposition = composition.Hero4_4;
            GroupOfFour groupOfFour = this.gameObject.AddComponent<GroupOfFour>();
            groupOfFour.Initialize(new PlayerData[4] { m_HerosList[0], m_HerosList[1], m_HerosList[2], m_HerosList[3] });
            groupOfFour.m_GroupIndex = 0;
            groupOfFour.ChangeCanvas(m_CanvasCompositions[6].transform.GetChild(0).gameObject);
            groupOfFour.UpdateImageCanvas();
            m_Groups = new Group[1] { groupOfFour };

            m_CurrentCanvasCompositions = m_CanvasCompositions[6];
        }
        m_currentGroupOnDisplay = m_Groups[0];
        m_CurrentGroupIndexOnDisplay = 0;
        _GameManager._UI_Manager.m_CurrentCanvasCharacter = m_CurrentCanvasCompositions.transform.parent.gameObject;
        _GameManager._UI_Manager.m_CurrentCanvasComposition = m_CurrentCanvasCompositions;
        m_GroupsCount = m_Groups.Length;
        _GameManager._UI_Manager.JustifierLeCodeVersLeBas();
    }

    public void RemakeComposition()
    {
        if (m_HerosCount == 1)
        {
            m_CurrentCanvasCompositions = m_CanvasCompositions[0];
            _GameManager._UI_Manager.HideInventaire("BCD");
        }
        else if (m_HerosCount == 2)
        {
            m_CurrentCanvasCompositions = m_CanvasCompositions[1];
            _GameManager._UI_Manager.HideInventaire("CD");
        }
        else if (m_HerosCount == 3)
        {
            m_CurrentCanvasCompositions = m_CanvasCompositions[3];
            _GameManager._UI_Manager.HideInventaire("D");
        }
        else if (m_HerosCount == 4)
        {

        }
    }


    public Group GetGroupByPlayer(PlayerData playerData)
    {
        foreach (var group in m_Groups)
        {
            if (group.m_Players.Contains(playerData))
            {
                return group;
            }
        }
        return null;
    }
    public Group GetGroupByPlayer(int indexPlayer)
    {
        PlayerData playerData = m_HerosList[indexPlayer];
        foreach (var group in m_Groups)
        {
            if (group.m_Players.Contains(playerData))
            {
                return group;
            }
        }
        return null;
    }
    public Group GetGroupByGroupIndex(int indexGroup)
    {
        return m_Groups[indexGroup];
    }
    public int GetGroupIndexByPlayer(PlayerData playerData)
    {
        int i = 0;
        foreach (var group in m_Groups)
        {
            if (group.m_Players.Contains(playerData))
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    public int GetGroupIndexOfPlayer(int indexPlayer)
    {
        PlayerData playerData = m_HerosList[indexPlayer];
        int i = 0;
        foreach (var group in m_Groups)
        {
            if (group.m_Players.Contains(playerData))
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    public PlayerData GetPlayerByGroup(int indexGroup, int indexPlayer)
    {
        return m_Groups[indexGroup].m_Players[indexPlayer];
    }

    public void SetObjectif(int indexGroup, string objectif)
    {
        m_Groups[indexGroup].SetObjectif(objectif);
    }

    public void SetCoordDescription(int indexGroup, string descriptionCase)
    {
        m_Groups[indexGroup].SetCoordDescription(descriptionCase);
    }

    public void reset_History()
    {
        m_Groups[0]._history.Clear();
        m_Groups[0]._historyResume.Clear();

    }

    #endregion

    #region Method by group
    public void AddHistory(int indexGroup, TextType type, string txt)
    {
        m_Groups[indexGroup].AddHistory(type, txt);
    }
    public void AddHistoryResume(int indexGroup, string txt)
    {
        m_Groups[indexGroup].AddHistoryResume(txt);
    }

    public void SetReady(int indexGroup, bool ready)
    {
        m_Groups[indexGroup].SetReady(ready);
    }
    public bool IsReady(int indexGroup)
    {
        return m_Groups[indexGroup].isReady();
    }

    #endregion

    #region UNION-FIND
    public int Find(int index)
    {
        if (_parent[index] != index)
        {
            _parent[index] = Find(_parent[index]);
        }
        return _parent[index];
    }
    private void Union(int index1, int index2)
    {
        int root1 = Find(index1);
        int root2 = Find(index2);
        if (root1 != root2)
        {
            _parent[root2] = root1;
        }
    }
    // Initialise et regroupe les joueurs par proximité
    public void GroupPlayers()
    {
        // Sauvegarde des groupes précédents
        var oldGroups = new Dictionary<int, List<PlayerData>>(_previousGroups);

        // Initialisation Union-Find
        for (int i = 0; i < m_HerosCount; i++)
        {
            _parent[i] = i; // Chaque joueur est son propre parent
        }

        // Connecte les joueurs proches (par distance <= 3) DistanceMaxForUnion=3
        for (int i = 0; i < m_HerosCount; i++)
        {
            for (int j = i + 1; j < m_HerosCount; j++)
            {
                Coord coord1 = m_HerosList[i].playerPosition;
                Coord coord2 = m_HerosList[j].playerPosition;

                if (coord1.GetDistance(coord2) <= DistanceMaxForUnion)
                {
                    Union(m_HerosList[i].playerIndex, m_HerosList[j].playerIndex);
                }
            }
        }

        // Mise à jour des groupes actuels
        var newGroups = GetGroups();

        // Assigne une signature unique aux nouveaux groupes
        foreach (var group in newGroups)
        {
            // Trie les joueurs par leur identifiant unique
            group.Value.Sort((a, b) => a.playerIndex.CompareTo(b.playerIndex));

            // Crée une signature basée sur les IDs des joueurs
            List<string> playerIdentifiers = group.Value
                .Select(player => player.playerIndex.ToString())
                .ToList();
            string groupID = string.Join("-", playerIdentifiers);

            // Applique la signature à chaque joueur
            foreach (var player in group.Value)
            {
                player._previousGroupID = player._groupID;
                player._groupID = groupID;
            }
        }

        // Sauvegarde des groupes actuels pour la prochaine exécution
        _previousGroups = newGroups;
    }
    public Dictionary<int, List<PlayerData>> GetGroups()
    {
        Dictionary<int, List<PlayerData>> groups = new Dictionary<int, List<PlayerData>>();

        for (int i = 0; i < m_HerosCount; i++)
        {
            int root = Find(i);
            if (!groups.ContainsKey(root))
            {
                groups[root] = new List<PlayerData>();
            }
            groups[root].Add(m_HerosList[i]);
        }

        return groups;
    }
    public int GetGroupSize(PlayerData targetPlayer)
    {
        int groupSize = 0;
        foreach (var player in m_HerosList)
        {
            if (player.groupSignature == targetPlayer.groupSignature)
            {
                groupSize++;
            }
        }

        return groupSize;
    }
    public List<PlayerData> GetGroupMembers(PlayerData targetPlayer)
    {
        List<PlayerData> groupMembers = new List<PlayerData>();

        foreach (var player in m_HerosList)
        {
            if (player.groupSignature == targetPlayer.groupSignature)
            {
                groupMembers.Add(player);
            }
        }

        return groupMembers;
    }
    public composition GetCompoOfUnionFind()
    {
        Dictionary<int, List<PlayerData>> groups = GetGroups();

        // Récupère la taille de chaque groupe
        List<int> groupSizes = groups.Values.Select(group => group.Count).ToList();

        // Trie les tailles des groupes
        groupSizes.Sort();

        // Crée une chaîne avec les tailles des groupes
        string composition = string.Join("-", groupSizes);
        return StringToComposition(composition);
    }
    #endregion

    #region Player Move
    public void PlayerHasMoved(int indexPlayerWhoMoved, bool isForDeath = false , int IndexToWatch = 0)
    {
        GroupPlayers();
        //Un joueur s'est déplacé, on regarde si cela a modifié les groupes.
        if (m_HerosList[indexPlayerWhoMoved]._previousGroupID != m_HerosList[indexPlayerWhoMoved]._groupID)
        {
            //Les groupes ont été modifié
            Move();
        }
        
        if (isForDeath)
        {
            Move();
            _GameManager._UI_Manager.m_CurrentCanvasComposition = m_CurrentCanvasCompositions;
            _GameManager._UI_Manager.SwitchGroupCanvas(GetGroupIndexOfPlayer(IndexToWatch));
        }
        else
        {
            _GameManager._UI_Manager.m_CurrentCanvasComposition = m_CurrentCanvasCompositions;
            AddEventMessageChangeGroup();
            _GameManager._UI_Manager.SwitchGroupCanvas(GetGroupIndexOfPlayer(indexPlayerWhoMoved));
        }

    }
    private void CreateMGroup()
    {
        composition compo = GetCompoOfUnionFind();

        GroupOfOne[] allGroupOfOnes = gameObject.GetComponents<GroupOfOne>();
        GroupOfTwo[] allGroupOfTwo = gameObject.GetComponents<GroupOfTwo>();
        GroupOfThree groupOfThree = gameObject.GetComponent<GroupOfThree>();
        GroupOfFour groupOfFour = gameObject.GetComponent<GroupOfFour>();

        int a = 0;
        int b = 1;
        int c = 2;
        int d = 3;
        switch (compo)
        {
            case composition.Hero1_1:
                m_Groups = new Group[1] { allGroupOfOnes[0] };
                UpdateCanvasAndIndex(0);
                break;
            case composition.Hero2_2:
                m_Groups = new Group[1] { allGroupOfTwo[0] };
                UpdateCanvasAndIndex(1);
                break;
            case composition.Hero2_1_1:
                a = GetGroupIndex(allGroupOfOnes, m_HerosList[0]);
                b = GetGroupIndex(allGroupOfOnes, m_HerosList[1]);
                m_Groups = new Group[2] { allGroupOfOnes[a], allGroupOfOnes[b] };
                UpdateCanvasAndIndex(2);
                break;
            case composition.Hero3_3:
                m_Groups = new Group[1] { groupOfThree };
                UpdateCanvasAndIndex(3);
                break;
            case composition.Hero3_2_1:
                m_Groups = new Group[2] { allGroupOfTwo[0], allGroupOfOnes[0] };
                UpdateCanvasAndIndex(4);
                break;
            case composition.Hero3_1_1_1:
                a = GetGroupIndex(allGroupOfOnes, m_HerosList[0]);
                b = GetGroupIndex(allGroupOfOnes, m_HerosList[1]);
                c = GetGroupIndex(allGroupOfOnes, m_HerosList[2]);
                m_Groups = new Group[3] { allGroupOfOnes[a], allGroupOfOnes[b], allGroupOfOnes[c] };
                UpdateCanvasAndIndex(5);
                break;
            case composition.Hero4_4:
                m_Groups = new Group[1] { groupOfFour };
                UpdateCanvasAndIndex(6);
                break;
            case composition.Hero4_3_1:
                m_Groups = new Group[2] { groupOfThree, allGroupOfOnes[0] };
                UpdateCanvasAndIndex(7);
                break;
            case composition.Hero4_2_2:
                a = GetGroupIndex(allGroupOfTwo, m_HerosList[0]);
                m_Groups = new Group[2] { allGroupOfTwo[a], allGroupOfTwo[Mathf.Abs(a - 1)] };
                UpdateCanvasAndIndex(9);
                break;
            case composition.Hero4_2_1_1:


                a = GetGroupIndex(allGroupOfOnes, m_HerosList[0]);
                b = GetGroupIndex(allGroupOfOnes, m_HerosList[1]);
                c = GetGroupIndex(allGroupOfOnes, m_HerosList[2]);
                d = GetGroupIndex(allGroupOfOnes, m_HerosList[3]);
                if (a == -1 && b == -1) { a = c; b = d; }
                else if (a == -1 && c == -1) { a = b; b = d; }
                else if (a == -1 && d == -1) { a = b; b = c; }
                else if (b == -1 && c == -1) { b = d; }
                else if (b == -1 && d == -1) { b = c; }
                m_Groups = new Group[3] { allGroupOfTwo[0], allGroupOfOnes[a], allGroupOfOnes[b] };

                UpdateCanvasAndIndex(8);
                break;
            case composition.Hero4_1_1_1_1:
                a = GetGroupIndex(allGroupOfOnes, m_HerosList[0]);
                b = GetGroupIndex(allGroupOfOnes, m_HerosList[1]);
                c = GetGroupIndex(allGroupOfOnes, m_HerosList[2]);
                d = GetGroupIndex(allGroupOfOnes, m_HerosList[3]);
                m_Groups = new Group[4] { allGroupOfOnes[a], allGroupOfOnes[b], allGroupOfOnes[c], allGroupOfOnes[d] };
                UpdateCanvasAndIndex(10);
                break;
            default:
                break;
        }

        m_CurrentComposition = compo;
        
    }
    public composition StringToComposition(string compo)
    {
        switch (compo)
        {
            case "1":
                return composition.Hero1_1;
            case "2":
                return composition.Hero2_2;
            case "1-1":
                return composition.Hero2_1_1;
            case "3":
                return composition.Hero3_3;
            case "2-1" or "1-2":
                return composition.Hero3_2_1;
            case "1-1-1":
                return composition.Hero3_1_1_1;
            case "4":
                return composition.Hero4_4;
            case "3-1" or "1-3":
                return composition.Hero4_3_1;
            case "2-2":
                return composition.Hero4_2_2;
            case "2-1-1" or "1-2-1" or "1-1-2":
                return composition.Hero4_2_1_1;
            case "1-1-1-1":
                return composition.Hero4_1_1_1_1;
            default:
                return composition.Hero1_1;
        }
    }
    int GetGroupIndex(Group[] group, PlayerData player)
    {


        if (group[0].m_Players.Contains(player))
        {
            return 0;
        }
        else if (group.Length > 1 && group[1].m_Players.Contains(player))
        {
            return 1;
        }
        else if (group.Length > 2 && group[2].m_Players.Contains(player))
        {
            return 2;
        }
        else if (group.Length > 3 && group[3].m_Players.Contains(player))
        {
            return 3;
        }
        else
        {
            return -1;
        }
    }
    public void Move()
    {
        foreach (var group in m_Groups)
        {
            DestroyImmediate(group);
        }

        Dictionary<int, List<PlayerData>> new_groups = GetGroups();
        foreach (var group in new_groups.Values)
        {

            switch (group.Count)
            {
                case 1:
                    GroupOfOne groupOfOne = this.gameObject.AddComponent<GroupOfOne>();
                    groupOfOne.Initialize(group[0]);
                    break;
                case 2:
                    GroupOfTwo groupOfTwo = this.gameObject.AddComponent<GroupOfTwo>();
                    groupOfTwo.Initialize(new PlayerData[2] { group[0], group[1] });
                    break;
                case 3:
                    GroupOfThree groupOfThree = this.gameObject.AddComponent<GroupOfThree>();
                    groupOfThree.Initialize(new PlayerData[3] { group[0], group[1], group[2] });
                    break;
                case 4:
                    GroupOfFour groupOfFour = this.gameObject.AddComponent<GroupOfFour>();
                    groupOfFour.Initialize(new PlayerData[4] { group[0], group[1], group[2], group[3] });
                    break;
                default:
                    break;
            }
        }

        CreateMGroup();
    }
    public void UpdateCanvasAndIndex(int nbrCanvas)
    {
        m_GroupsCount = m_Groups.Length;
        int index = 0;
        foreach (var group in m_Groups)
        {
            group.m_GroupIndex = index;
            group.ChangeCanvas(m_CanvasCompositions[nbrCanvas].transform.GetChild(index).gameObject);
            //group.UpdateImageCanvas();
            index++;
            group.UpdateCanvasInventoryAllPlayer();
        }
        m_CurrentCanvasCompositions.SetActive(false);
        m_CurrentCanvasCompositions = m_CanvasCompositions[nbrCanvas];
        m_CurrentCanvasCompositions.SetActive(true);
    }
    public void UpdateAllCanvasInventory()
    {
        foreach (var group in m_Groups)
        {
            group.UpdateCanvasInventoryAllPlayer();
        }
    }
    public void AddEventMessageChangeGroup()
    {
        //On parcourt tout les groupes existants
        //Un groupe est composé de 1 à 4 joueurs. 
        foreach (var group in m_Groups)
        {
            if (group.m_PlayerCount == 1) // Si le groupe est composé d'un seul joueur
            {
                PlayerData player = group.m_Players[0];

                if (player._groupID != player._previousGroupID) // Si le groupID a changé
                {
                    // Récupérer les noms des joueurs de l'ancien groupe
                    List<string> otherPlayerNames = GetPlayerNames(player._previousGroupID);

                    if (otherPlayerNames.Count > 0)
                    {
                        var value = new Dictionary<string, string>
                        {
                            {"allName", string.Join(", ", otherPlayerNames) },
                            {"playerName", player.playerName }
                        };
                        string message = _GameManager._language.GetPrompt("_group_manager_split_group_A", value);
                        group.AddHistory(TextType.Event, message, true, player._previousGroupID, player._groupID);
                    }
                }
            }

            else if (group.m_PlayerCount == 2) // Si le groupe est composé de 2 joueurs
            {
                PlayerData player1 = group.m_Players[0];

                if (player1._groupID != player1._previousGroupID) // Si le groupID a changé
                {
                    PlayerData player2 = group.m_Players[1];
                    //On regarde si les deux joueurs étaient déjà dans le même groupe avant ou pas.
                    if (WereInSamePreviousGroup(player1, player2))
                    {
                        //Si ils étaient en effet déjà dans le même groupe, c'est qu'ils se sont séparés à deux de leur ancien groupe de 3 ou de 4.
                        List<string> otherPlayerNames = GetPlayerNames(player1._previousGroupID);
                        var value = new Dictionary<string, string>
                        {
                            {"allName", string.Join(", ", otherPlayerNames) },
                            {"playerName1", player1.playerName },
                            {"playerName2", player2.playerName }
                        };
                        string message = _GameManager._language.GetPrompt("_group_manager_split_group_B", value);
                        group.AddHistory(TextType.Event, message, true, player1._previousGroupID, player1._groupID);
                    }

                    else
                    {
                        //Si ils n'étaient pas dans le même groupe, soit ils étaient tous les deux tout seul, soit l'un a quitté un groupe pour rejoindre l'autre.
                        int countheroInPreviousGroupOfPlayer1 = CountHeroInPreviousGroup(player1);
                        int countheroInPreviousGroupOfPlayer2 = CountHeroInPreviousGroup(player2);
                        //On vérifie si ils étaient tous les deux seuls
                        if (countheroInPreviousGroupOfPlayer1 == 1 && countheroInPreviousGroupOfPlayer2 == 1)
                        {
                            var value = new Dictionary<string, string>
                        {
                            {"playerName1", player1.playerName },
                            {"playerName2", player2.playerName }
                        };
                            string message = _GameManager._language.GetPrompt("_group_manager_split_group_C", value);
                            group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}", player1._groupID);
                        }
                        else
                        {
                            if (countheroInPreviousGroupOfPlayer1 > 1)
                            {
                                //Si le player 1 a quitté un groupe pour rejoindre player 2
                                List<string> otherPlayerNames = GetOtherPlayerNames(player1._previousGroupID, player1.playerIndex);
                                var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"allName", string.Join(", ", otherPlayerNames) }
                                };
                                string message = _GameManager._language.GetPrompt("_group_manager_split_group_D", value);
                                group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}", player1._groupID);
                            }
                            else
                            {
                                //Si le player 2 a quitté un groupe pour rejoindre player 1
                                List<string> otherPlayerNames = GetOtherPlayerNames(player2._previousGroupID, player2.playerIndex);
                                var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"allName", string.Join(", ", otherPlayerNames) }
                                };
                                string message = _GameManager._language.GetPrompt("_group_manager_split_group_E", value);
                                group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}", player1._groupID);
                            }
                        }
                    }
                }
            }

            else if (group.m_PlayerCount == 3) // Si le groupe est composé de 3 joueurs
            {
                PlayerData player1 = group.m_Players[0];

                if (player1._groupID != player1._previousGroupID) // Si le groupID a changé
                {
                    PlayerData player2 = group.m_Players[1];
                    PlayerData player3 = group.m_Players[2];
                    //On regarde si les trois joueurs étaient déjà dans le même groupe avant ou pas.
                    if (WereInSamePreviousGroup(player1, player2) && WereInSamePreviousGroup(player1, player3))
                    {
                        //Si ils étaient en effet déjà dans le même groupe, c'est qu'ils se sont séparés à trois de leur ancien groupe de 4.
                        List<string> otherPlayerNames = GetPlayerNames(player1._previousGroupID);
                        var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName },
                                    {"allName", string.Join(", ", otherPlayerNames) }
                                };
                        string message = _GameManager._language.GetPrompt("_group_manager_split_group_F", value);
                        group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}", player1._groupID);
                    }

                    else
                    {
                        //Si ils n'étaient pas dans le même groupe, soit ils étaient tous les trois tout seul, soit l'un a quitté un groupe pour rejoindre les deux autres.
                        int countheroInPreviousGroupOfPlayer1 = CountHeroInPreviousGroup(player1);
                        int countheroInPreviousGroupOfPlayer2 = CountHeroInPreviousGroup(player2);
                        int countheroInPreviousGroupOfPlayer3 = CountHeroInPreviousGroup(player3);
                        //On vérifie si ils étaient tous les trois seuls
                        if (countheroInPreviousGroupOfPlayer1 == 1 && countheroInPreviousGroupOfPlayer2 == 1 && countheroInPreviousGroupOfPlayer3 == 1)
                        {
                            var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName }
                                };
                            string message = _GameManager._language.GetPrompt("_group_manager_split_group_G", value);
                            group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}+{player3._previousGroupID}", player1._groupID);
                        }
                        // Sinon of vérifie lesquels étaient déjà ensemble à deux.
                        else
                        {
                            //Si le player 1 et player2 étaient dans le même groupe.
                            if (WereInSamePreviousGroup(player1, player2))
                            {
                                if (countheroInPreviousGroupOfPlayer3 == 1)
                                {
                                    //Si le player 3 était seul et a rejoins / a été rejoins pas le groupe de 2
                                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName }
                                };
                                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_H", value);
                                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player3._previousGroupID}", player1._groupID);
                                }
                                else
                                {
                                    //Si le player 3 a quitté son groupe pour rejoindre le groupe de 2
                                    List<string> otherPlayerNames = GetOtherPlayerNames(player3._previousGroupID, player3.playerIndex);
                                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName },
                                    {"allName", string.Join(", ", otherPlayerNames) }
                                };
                                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_I", value);
                                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player3._previousGroupID}", player1._groupID);
                                }
                            }
                            //Si le player 1 et player3 étaient dans le même groupe.
                            else if (WereInSamePreviousGroup(player1, player3))
                            {
                                if (countheroInPreviousGroupOfPlayer2 == 1)
                                {
                                    //Si le player 2 était seul et a rejoins / a été rejoins pas le groupe de 2
                                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName }
                                };
                                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_J", value);
                                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}", player1._groupID);
                                }
                                else
                                {
                                    //Si le player 2 a quitté son groupe pour rejoindre le groupe de 2
                                    List<string> otherPlayerNames = GetOtherPlayerNames(player2._previousGroupID, player2.playerIndex);
                                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName },
                                    {"allName", string.Join(", ", otherPlayerNames) }
                                };
                                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_K", value);
                                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}", player1._groupID);
                                }
                            }
                            //Si le player 1 et player3 étaient dans le même groupe.
                            else if (WereInSamePreviousGroup(player2, player3))
                            {
                                if (countheroInPreviousGroupOfPlayer1 == 1)
                                {
                                    //Si le player 1 était seul et a rejoins / a été rejoins pas le groupe de 2
                                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName }
                                };
                                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_L", value);
                                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player3._previousGroupID}", player1._groupID);
                                }
                                else
                                {
                                    //Si le player 1 a quitté son groupe pour rejoindre le groupe de 2
                                    List<string> otherPlayerNames = GetOtherPlayerNames(player1._previousGroupID, player1.playerIndex);
                                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName },
                                    {"allName", string.Join(", ", otherPlayerNames) }
                                };
                                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_M", value);
                                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player3._previousGroupID}", player1._groupID);
                                }
                            }
                        }
                    }
                }
            }

            else
            {//Si le groupe est composé de 4 joueurs , c'est qu'ils ont fusionné forcément.
                PlayerData player1 = group.m_Players[0];
                PlayerData player2 = group.m_Players[1];
                PlayerData player3 = group.m_Players[2];
                PlayerData player4 = group.m_Players[3];

                if (player1._groupID != player1._previousGroupID) // Si le groupID a changé
                {
                    var value = new Dictionary<string, string>
                                {
                                    {"playerName1", player1.playerName },
                                    {"playerName2", player2.playerName },
                                    {"playerName3", player3.playerName },
                                    {"playerName4", player4.playerName }
                                };
                    string message = _GameManager._language.GetPrompt("_group_manager_split_group_N", value);
                    group.AddHistory(TextType.Event, message, true, $"{player1._previousGroupID}+{player2._previousGroupID}+{player3._previousGroupID}+{player4._previousGroupID}", player1._groupID);
                }
            }
        }
    }
    private bool WereInSamePreviousGroup(PlayerData player1, PlayerData player2)
    {
        // Extraire les IDs des anciens groupes des deux joueurs
        List<int> player1PreviousGroup = player1._previousGroupID.Split('-').Select(int.Parse).ToList();
        List<int> player2PreviousGroup = player2._previousGroupID.Split('-').Select(int.Parse).ToList();

        // Vérifier s'il y a un ID en commun entre les deux listes
        return player1PreviousGroup.Intersect(player2PreviousGroup).Any();
    }
    private int CountHeroInPreviousGroup(PlayerData player1)
    {
        List<int> player1PreviousGroup = player1._previousGroupID.Split('-').Select(int.Parse).ToList();
        return player1PreviousGroup.Count;
    }
    private List<string> GetOtherPlayerNames(string previousGroupID, int currentPlayerID)
    {
        // Diviser la chaîne "previousGroupID" en un tableau d'IDs
        string[] ids = previousGroupID.Split('-');

        // Convertir les IDs en entiers, tout en excluant le joueur actuel
        List<int> otherPlayerIDs = ids
            .Select(id => int.Parse(id))
            .Where(id => id != currentPlayerID)
            .ToList();

        // Récupérer les noms des joueurs correspondants
        List<string> otherPlayerNames = otherPlayerIDs
            .Select(id => m_HerosList[id].playerName)
            .ToList();

        return otherPlayerNames;
    }
    private List<string> GetPlayerNames(string groupId)
    {
        // Diviser la chaîne "previousGroupID" en un tableau d'IDs
        string[] ids = groupId.Split('-');

        // Convertir les IDs en entiers
        List<int> _playerIDs = ids
            .Select(id => int.Parse(id))
            .ToList();

        // Récupérer les noms des joueurs correspondants
        List<string> _playerNames = _playerIDs
            .Select(id => m_HerosList[id].playerName)
            .ToList();

        return _playerNames;
    }

    public bool GetIsCheckingCoherence(int indexGroup)
    {
        return m_Groups[indexGroup].isCheckingCoherence;
    }
    public void SetIsCheckingCoherence(int indexGroup, bool value)
    {
        m_Groups[indexGroup].isCheckingCoherence = value;
    }
    #endregion

}

