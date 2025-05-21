using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Events;
using static Tags;
using static UI_Manager;

public class Group : MonoBehaviour
{
    #region Variables
    public GameManager _gameManager;
    public string m_GroupeName;
    public int m_GroupIndex; // L'index du sous-groupe dans le groupe
    public int m_PlayerCount; //Nombre de joueurs dans ce sous-groupe
    public GameObject m_Canvas;
    public UnityEngine.UI.Text playerTextCanvas;
    public PlayerData[] m_Players;
    public List<string> m_GroupInventaire;
    public List<History> _history;
    public List<HistoryResume> _historyResume;
    public string _lastPhase;
    public string _lastPhaseResume;
    public string _penultimatePhaseResume;
    public string _penultimatePenultimatePhaseResume;
    public string _currentObjectif;
    public string _lastAction;
    public string _groupID;


    [Header("Combats")]
    public bool _groupInCombat = false;//indique si le groupe est en combat

    [Header("Input")]
    public bool isCheckingCoherence = false;

    [Header("Choices")]
    public string[] GroupChoices;

    public bool GroupIsReady;

    [Header("Map")]
    public Coord GroupPosition;
    public int GroupVision;
    //public int GroupDeplacement;

    [Header("Dead")]
    public bool isDead = false; //indique si le groupe est mort

    private LocalizationManager _language;
    #endregion

    #region Initialize Methods
    public void Initialize(PlayerData[] players)
    {
        this.m_GroupIndex = -1;
        this.m_Players = players.OrderBy(player => player.playerIndex).ToArray();
        this.m_PlayerCount = players.Length;
        this.m_Canvas = null;
        this.playerTextCanvas = null;
        this.isDead = !m_Players[0].playerIsAlive;
        this.GroupVision = m_Players.Max(player => player.playerVision);
        this.GroupIsReady = players.All(pd => pd.playerIsReady);
        this._history = new List<History>();
        this.m_GroupInventaire = new List<string>();
        this._historyResume = new List<HistoryResume>();
        this.m_GroupeName = string.Join("-", m_Players
            .OrderBy(player => player.playerIndex)
            .Select(player => player.playerName));
        MergeListsHistoryResume();
        MergeListsHistory();
        this._gameManager = FindObjectOfType<GameManager>();
        _language = _gameManager._language;
        for (int i = 0; i < m_PlayerCount; i++)
        {
            m_GroupInventaire.AddRange(m_Players[i].playerInventaire);
            m_Players[i]._playerGroup = this;
        }
        this._groupID = string.Join("-", m_Players
            .OrderBy(player => player.playerIndex)
            .Select(player => player.playerIndex.ToString()));
    }

    public string GetAllInventaireString()
    {
        string toReturn = "";
        if (m_GroupInventaire.Count == 0)
        {
            return _language.GetText("__inventory_empty_");
        }
        foreach (string item in m_GroupInventaire)
        {
            toReturn += item + "; ";
        }
        return toReturn;
    }

    public void MergeListsHistoryResume()
    {
        Dictionary<int, HistoryResume> mergedDict = new Dictionary<int, HistoryResume>();

        foreach (var player in m_Players)
        {
            if (player.GetAllHistoryResume() != null)
            {
                // Ajouter les �l�ments de la liste de player 
                foreach (var item in player.GetAllHistoryResume())
                {
                    if (!mergedDict.ContainsKey(item.GetID())) { mergedDict[item.GetID()] = item; }
                }
            }
            else { return; }
        }

        this._historyResume = mergedDict.Values.OrderBy(hr => hr.GetID()).ToList();

    }
    public void MergeListsHistory()
    {
        Dictionary<int, History> mergedDict = new Dictionary<int, History>();

        foreach (var player in m_Players)
        {
            if (player.GetAllHistory() != null)
            {
                foreach (var item in player.GetAllHistory())
                {
                    if (!mergedDict.ContainsKey(item.GetID())) { mergedDict[item.GetID()] = item; }
                }
            }
            else { return; }
        }

        this._history = mergedDict.Values.OrderBy(hr => hr.GetID()).ToList();
    }
    #endregion

    #region Methods
    public void ChangeCanvas(GameObject canvas)
    {
        m_Canvas = canvas;
        playerTextCanvas = m_Canvas.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        UpdateImageCanvas();
        playerTextCanvas.text = GetAllHistoryInString();

    }
    public void UpdateImageCanvas()
    {
        for (int i = 0; i < m_PlayerCount; i++)
        {
            if (m_Players[i].playerIndex == 0) { UpdateTextsAndImageCanvas(new Color(1, 0, 0, 1), i); } // red
            else if (m_Players[i].playerIndex == 1) { UpdateTextsAndImageCanvas(new Color(0.08497676f, 0.8188679f, 0.2022778f, 1), i); } // green
            else if (m_Players[i].playerIndex == 2) { UpdateTextsAndImageCanvas(new Color(0.2018868f, 0.5714071f, 1, 1), i); } // blue
            else { UpdateTextsAndImageCanvas(new Color(1, 0.8090067f, 0, 1), i); } // yellow

        }

    }
    public void UpdateTextsAndImageCanvas(Color color, int playerIndexInsideGroup)
    {
        //CADRE
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = m_Players[playerIndexInsideGroup].playerSprite;
        //NAME
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerName;  //Name
        if(!isDead)
        { 
            //SANTE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerHealthNbr.ToString();  //Sante(int)
            //ARMOR
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerArmorClass.ToString();  //armor
            //ARME
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerArmePuissance.ToString(); //arme
            //VISEE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerAimBonus.ToString(); //visee
            //VISION
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerVision.ToString(); // vision
            //DEPLACEMENT
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = m_Players[playerIndexInsideGroup].playerVision.ToString(); // deplacement
            //INVENTAIRE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
            //TO DO
        }

        if (isDead)
        {
            //DEAD
            //SANTE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false); //Sante
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false); //Sante
                                                                                                                                                                                                                                  //ARMOR
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false); //Armor
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false); //Armor
                                                                                                                                                                                                                                 //ARME
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false); //Arme
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false); //Arme
                                                                                                                                                                                                                                        //VISEE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false); //Visee
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).transform.GetChild(1).gameObject.SetActive(false); //Visee
                                                                                                                                                                                                                                        //VISION
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(0).gameObject.SetActive(false); //Vision
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(1).gameObject.SetActive(false); //Vision
                                                                                                                                                                                                                                        //DEPLACEMENT
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(0).gameObject.SetActive(false); //Deplacement
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(1).gameObject.SetActive(false); //Deplacement
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
            color = new Color(0.3f, 0.3f, 0.3f, 0.3f); // gris
                                                    //CADRE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
            //IMAGE HERO
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Name

            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndexInsideGroup).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>().color = color;
        }
        else
        {
            updateColorText(color, playerIndexInsideGroup);
        }
    }

    public void updateColorText(Color color, int playerIndex)
    {
        if (isDead)
        {
            color = new Color(0.3f, 0.3f, 0.3f, 0.3f); // gris
                                                       //CADRE
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = color;
            //IMAGE HERO
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = color;
            m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Name


            return;
        }
        //CADRE
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = color;
        //IMAGE HERO
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = color;
        //NAME
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Name
        //SANTE
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Sante
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color; //Sante
        //ARMOR
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Armor
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color; //Armor
        //ARME
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Arme
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color; //Arme
        //VISEE
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Visee
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(4).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color; //Visee
        //VISION
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Vision
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(5).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color; //Vision
        //DEPLACEMENT
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Deplacement
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(6).transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = color; //Deplacement
        //INVENTAIRE
        m_Canvas.transform.GetChild(0).transform.GetChild(playerIndex).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(7).transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = color; //Inventaire
    }

    public void showSelectionned(bool tf)
    {

        for (int i = 0; i < m_PlayerCount; i++)
        {
            GameObject target = m_Canvas.transform.GetChild(0).GetChild(i).GetChild(0).GetChild(1).gameObject;
            target.SetActive(tf);

            Color color = new Color(1, 1, 1, 1);
            if (m_Players[i].playerIndex == 0) { color = new Color(1, 0, 0, 1); } // red
            else if (m_Players[i].playerIndex == 1) { color = new Color(0.08497676f, 0.8188679f, 0.2022778f, 1); } // green
            else if (m_Players[i].playerIndex == 2) { color = new Color(0.2018868f, 0.5714071f, 1, 1); } // blue
            else { color = new Color(1, 0.8090067f, 0, 1); } // yellow

            if (!tf)
            {
                //UNSELECTED
                color = new Color(color.r, color.g, color.b, 0.3f);
            }

            updateColorText(color, i);
            updateInventaireColor(tf, i);
        }
    }

    public void updateInventaireColor(bool tf, int indexPlayer)
    {
        int numeroInventaire = 0;
        for (int i = 0; i < m_GroupIndex; i++)
        {
            numeroInventaire += _gameManager._groupsManager.m_Groups[i].m_PlayerCount;
        }
        numeroInventaire += indexPlayer;

        GameObject inventaire = _gameManager._UI_Inventaire_Buttons.m_Canvas_Inventaires[numeroInventaire];
        for (int i = 0; i < 8; i++)
        {
            int[] inventairePlayer = m_Players[indexPlayer].GetAllItemsCount();

            if (inventairePlayer[i] > 0)
            {

                inventaire.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().color = tf ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1);
                inventaire.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = tf ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1);
            }
            else
            {

                inventaire.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().color = Color.black;
                inventaire.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            }

        }
    }

    public void AfficherText(bool tf)
    {
        m_Canvas.transform.GetChild(1).gameObject.SetActive(tf);
    }
    #endregion

    #region Player Data Method
    public virtual string GetAllNames()
    {
        return "";
    }


    public void SetPosition(Coord position)
    {
        GroupPosition = position;
    }

    public void SetCoordDescription(string descriptionCase)
    {
        for (int i = 0; i < m_PlayerCount; i++)
        {
            m_Players[i].playerPosition._description = descriptionCase;
        }
    }

    #endregion

    #region Groups Méthods


    public string UpdateMessageGroup(string[] prevID, string[] newID)
    {
        int[] _prev = prevID.Select(int.Parse).ToArray();
        int[] _new = newID.Select(int.Parse).ToArray();
        PlayerData[] _listePlayer = _gameManager._groupsManager.m_HerosList;

        var values = new Dictionary<string, string>
        {
            { "_prev0", _listePlayer[_prev[0]].playerName},
            { "_prev1", _listePlayer[_prev[1]].playerName},
            { "_new0", _listePlayer[_new[0]].playerName}
        };

        if (_new.Length == 1)
        {
            if (_prev.Length == 2)
            {
                return _gameManager._language.GetPrompt("__split_group_1_message__", values);
            }
            else if (_prev.Length == 3)
            {
                values.Add("_prev2", _listePlayer[_prev[2]].playerName);
                return _gameManager._language.GetPrompt("__split_group_2_message__", values);
            }
            else if (_prev.Length == 4)
            {
                values.Add("_prev2", _listePlayer[_prev[2]].playerName);
                values.Add("_prev3", _listePlayer[_prev[3]].playerName);
                return _gameManager._language.GetPrompt("__split_group_3_message__", values);
            }
        }
        else if (_new.Length == 2)
        {
            if (_prev.Length == 3) { 
                values.Add("_new1", _listePlayer[_new[1]].playerName);
                values.Add("_prev2", _listePlayer[_prev[2]].playerName);
            return _gameManager._language.GetPrompt("__split_group_4_message__", values);
            }
            else if (_prev.Length == 4)
            {
                values.Add("_prev2", _listePlayer[_prev[2]].playerName);
                values.Add("_prev3", _listePlayer[_prev[3]].playerName);
                values.Add("_new1", _listePlayer[_new[1]].playerName);
                return _gameManager._language.GetPrompt("__split_group_5_message__", values);
            }
        }
        else if (_new.Length == 3)
        {
            if (_prev.Length == 4)
            {
                values.Add("_prev2", _listePlayer[_prev[2]].playerName);
                values.Add("_prev3", _listePlayer[_prev[3]].playerName);
                values.Add("_new1", _listePlayer[_new[1]].playerName);
                values.Add("_new2", _listePlayer[_new[2]].playerName);
                return _gameManager._language.GetPrompt("__split_group_6_message__", values);
            }
        }
        return "Erreur --> Script \"Group\" ligne 220 à peu près";
    }

    public bool CheckHistoryChangingGroup(History new_History)
    {
        History prev_history = _history.Last();

        if (!prev_history.isChangingGroup || !new_History.isChangingGroup)
        {
            return false;
        }

        // Index du dernier élément
        int index = _history.Count - 1;
        // Parcourir la liste en remontant
        while (_history[index].isChangingGroup)
        {
            index--;
        }
        index++;
        while (index < _history.Count)
        {
            // Récupération de l'instance de l'élément précédent
            prev_history = _history[index];

            string[] allPreviousID = prev_history.previousGroupId.Split('+');

            foreach (string previousID in allPreviousID)
            {
                //previousID
                string[] individualPreviousID = previousID.Split('-');
                string[] individualNewID = new_History.newGroupId.Split('-');
                bool allElementsPresent = individualNewID.All(id => individualPreviousID.Contains(id));
                string newMessage = "";


                // Si tous les �l�ments de individualNewID sont pr�sents dans individualPreviousID
                if (allElementsPresent)
                {
                    //Supprimer les messages de changement de groupe.
                    try
                    {
                        _history.RemoveRange(index, _history.Count - index);
                        _historyResume.RemoveRange(index, _history.Count - index);
                    }
                    catch 
                    {
                        //
                    }

                    if (individualPreviousID.Length > individualNewID.Length)
                    {
                        newMessage = UpdateMessageGroup(individualPreviousID, individualNewID);
                    }

                    foreach (PlayerData player in m_Players)
                    {
                        // Index du dernier �l�ment
                        int indexByPlayer = player.playerAllHistory.Count - 1;

                        // Parcourir la liste en remontant
                        while (player.playerAllHistory[indexByPlayer].isChangingGroup)
                        {
                            indexByPlayer--;
                        }
                        indexByPlayer++;
                        player.playerAllHistory.RemoveRange(indexByPlayer, player.playerAllHistory.Count - indexByPlayer);

                        // Index du dernier �l�ment
                        indexByPlayer = player.playerAllHistoryResume.Count - 1;
                        // Parcourir la liste en remontant
                        while (player.playerAllHistoryResume[indexByPlayer].isChangingGroup)
                        {
                            indexByPlayer--;
                        }
                        indexByPlayer++;
                        player.playerAllHistoryResume.RemoveRange(indexByPlayer, player.playerAllHistoryResume.Count - indexByPlayer);

                    }

                    if (newMessage != "")
                    {
                        AddHistory(TextType.Event, newMessage, true, previousID, new_History.newGroupId, prev_history.ID);
                        AddHistoryResume(newMessage, true, previousID, new_History.newGroupId, prev_history.ID);
                    }
                    return true;
                }

            }
            index++;
        }
        return false;

    }

    public void AddHistory(TextType type, string txt, bool isChangingGroup = false, string previousGroupID = null, string newGroupID = null, int id = -1)
    {
        History new_history;
        if (!isChangingGroup)
        {
            new_history = new History(txt);
        }
        else
        {
            new_history = new History(txt, isChangingGroup, previousGroupID, newGroupID, id);
            if (CheckHistoryChangingGroup(new_history))
            {
                playerTextCanvas.text = GetAllHistoryInString();
                return;
            }
            AddHistoryResume(txt, isChangingGroup, previousGroupID, newGroupID, id);
        }

        if (txt.StartsWith(":"))
        {
            txt = txt.Substring(1).Trim();
        }

        if (type == TextType.Histoire)
        {
            _history.Add(new_history);
            if (!isChangingGroup)
            {
                _lastPhase = txt;
            }
        }
        else
        {
            string newTxt = "<color=#00FF00>" + txt + "</color>";
            new_history.SetText(newTxt);
            _history.Add(new_history);
            //if(!isChangingGroup)
            //{
            _lastAction = txt;
            //}
            _lastAction = txt;
        }

        for (int i = 0; i < m_PlayerCount; i++)
        {
            m_Players[i].AddHistory(type, new_history);
        }

        playerTextCanvas.text = GetAllHistoryInString();
    }
    public void AddHistoryResume(string txt, bool isChangingGroup = false, string previousGroupID = null, string newGroupID = null, int id = -1)
    {

        HistoryResume new_historyResume;
        if (!isChangingGroup)
        {
            new_historyResume = new HistoryResume(txt);
        }
        else
        {
            new_historyResume = new HistoryResume(txt, isChangingGroup, previousGroupID, newGroupID, id);
        }

        _historyResume.Add(new_historyResume);
        //if (!isChangingGroup)
        //{
        _penultimatePenultimatePhaseResume = _penultimatePhaseResume;
        _penultimatePhaseResume = _lastPhaseResume;
        _lastPhaseResume = txt;
        //}
        for (int i = 0; i < m_PlayerCount; i++)
        {
            m_Players[i].AddHistoryResume(new_historyResume);
        }
    }

    public void SetReady(bool ready)
    {
        GroupIsReady = ready;
        for (int i = 0; i < m_PlayerCount; i++)
        {
            m_Players[i].SetReady(ready);
        }
    }
    //rend groupe ready mais que 1 seul joueur
    public void SetReady(bool ready, int index)
    {
        GroupIsReady = ready;
        m_Players[index].SetReady(ready);
    }
    public bool isReady()
    {
        return GroupIsReady;
    }
    public void SetObjectif(string objectif)
    {
        _currentObjectif = objectif;
        for (int i = 0; i < m_PlayerCount; i++)
        {
            m_Players[i].SetObjectif(objectif);
        }
    }
    public void SetMoved(bool moved)
    {
        foreach (var player in m_Players)
        {
            player._hasMoved = moved;
            if (player._specialMove == 2)
            {
                player.AddInventoryShoes(-1);
                player._specialMove = 0;
            }
        }
    }

    public bool SomeoneHasMoved()
    {
        //Vérifie si au moins un joueur a bougé
        return m_Players.Any(player => player._hasMoved);
    }
    public virtual string GetResume(bool forCoherence = false)
    {
        return "";
    }

    public virtual string GetResume_Names()
    {
        return "";
    }
    public string GetResume_Position()
    {
        string toReturn = "";
        for (int i = 0; i < m_PlayerCount; i++)
        {
            if (m_Players[i].playerPosition._isVisited == true)
            {
                toReturn += m_Players[i].playerPosition._description + ";";
            }
        }
        if (toReturn == "")
        {
            toReturn = _gameManager._language.GetText("__case__non_visitee__");
        }
        return toReturn;
    }

    public string GetResume_LastPosition()
    {
        HashSet<string> uniqueDescriptions = new HashSet<string>();
        for (int i = 0; i < m_PlayerCount; i++)
        {
            if (m_Players[i].playerPreviousCoord != null) {
                uniqueDescriptions.Add(m_Players[i].playerPreviousCoord._description);
            }
        }
        return string.Join(";", uniqueDescriptions);
    }


    public string GetResume_InVision(bool onlyVisible = false)
    {
        GroupVision = m_Players.Max(player => player.playerVision);
        string toReturn = "";
        bool doorIsVisible = false;
        Dictionary<int, string> events = GetEventNeighbours(out doorIsVisible, onlyVisible);



        for (int i = 0; i <= GroupVision; i++)
        {
            var values = new Dictionary<string, string>
            {
                { "content", events.GetValueOrDefault(i)}
            };
            if (i == 0)
            {
                toReturn += _gameManager._language.GetPrompt("__vision__current", values);
            }
            else if (i == 1)
            {
                toReturn += _gameManager._language.GetPrompt("__vision__distance_one", values);
            }
            else
            {
                toReturn += _gameManager._language.GetPrompt("__vision__distance_many", values);
            }
        }

        if (!doorIsVisible)
        {
            toReturn += _gameManager._language.GetText("__vision__no_door");
        }
        return toReturn;
    }

    public Dictionary<int, string> GetEventNeighbours(out bool doorIsVisible, bool onlyVisible = false)
    {
        HashSet<Coord> allCoordPreviousDist = new HashSet<Coord>();
        Dictionary<int, string> toReturn = new Dictionary<int, string>();
        HashSet<string> uniqueDescriptions = new HashSet<string>();
        doorIsVisible = false;
        for (int i = 0; i <= GroupVision; i++)
        {

            HashSet<Coord> currentDist = new HashSet<Coord>();
            foreach (PlayerData player in m_Players)
            {
                Coord playerCoord = player.GetCoord();
                if (i == 0)
                {
                    currentDist.Add(playerCoord);
                    allCoordPreviousDist.UnionWith(currentDist);
                }
                else
                {
                    HashSet<Coord> playerNeighbors = playerCoord.GetNeighboursPreciseDistance(i, allCoordPreviousDist, onlyVisible);
                    currentDist.UnionWith(playerNeighbors);
                    allCoordPreviousDist.UnionWith(playerNeighbors);
                }
            }

            string txt = GetEventNeighbours(currentDist, uniqueDescriptions, out doorIsVisible);

            toReturn[i] = txt;
        }

        return toReturn;
    }
    public string GetAllHistoryResumeInString()
    {
        string toReturn = "";
        if (_historyResume == null || _historyResume.Count == 0) { return toReturn; }
        foreach (HistoryResume hr in _historyResume)
        {
            string txt = hr.GetResume();
            toReturn += txt + "; ";
        }
        return toReturn;
    }
    public List<HistoryResume> GetAllHistoryResume()
    {
        return _historyResume;
    }
    public List<History> GetAllHistory()
    {
        return _history;
    }
    public string GetAllHistoryInString()
    {
        string toReturn = "";
        if(_history == null || _history.Count == 0) { return toReturn; }
        foreach (History hr in _history)
        {
            string txt = hr.GetText();
            toReturn += txt + "\n \n ";
        }
        return toReturn;
    }
    public string GetGroupNeighbors(bool forCoherence = false)
    {
        Dictionary<Coord, int> neighborsDict = new Dictionary<Coord, int>();

        foreach (PlayerData player in m_Players)
        {
            Coord playerCoord = player.GetCoord();
            HashSet<Coord> playerNeighbors = forCoherence == true ? playerCoord.GetNeighboursVisibles(player.playerVision) : playerCoord.GetNeighbours(player.playerVision);
            foreach (Coord c in playerNeighbors)
            {
                int dist = playerCoord.GetDistance(c);
                if (!neighborsDict.ContainsKey(c) || neighborsDict[c] > dist)
                {
                    neighborsDict[c] = dist;
                }
            }
        }

        string toReturn = "";

        int nbr_monstre = 0;
        int nbr_coffre = 0;
        List<string> memoryLoot = new List<string>();


        foreach (var coordDist in neighborsDict)
        {
            Coord c = coordDist.Key;
            int dist = coordDist.Value;
            var values = new Dictionary<string, string>
            {
                { "dist", dist.ToString()}
            };

            switch (c.GetEvent())
            {
                case EventCoord.Nothing:
                    break;
                case EventCoord.Wall:
                    break;
                case EventCoord.Loot:
                    if (c._isVisited) {break;}
                    nbr_coffre++;
                    if (dist == 1) { memoryLoot.Add(_gameManager._language.GetPrompt("__voisin__distance__coffre_singulier_", values));}
                    else { memoryLoot.Add(_gameManager._language.GetPrompt("__voisin__distance__coffre_pluriel_", values)); }
                    break;
                case EventCoord.Enemy:
                    if (c._isVisited) {break;}
                    nbr_monstre++;
                    break;
                case EventCoord.Trap:
                    break;
                case EventCoord.DoorLocked:
                    toReturn += "";
                    break;
                case EventCoord.Boss:
                    break;
                default:
                    break;
            }
        }
        return toReturn;
    }

    public void AddInformationsWhenSplit()
    {
        var values = new Dictionary<string, string>
            {
                { "player0", m_Players[0].playerName},
                { "player1", m_Players[1].playerName},
                { "player2", m_Players[2].playerName},
            };

        if (m_PlayerCount == 1)
        {
            AddHistory(TextType.Event, _language.GetPrompt("__case_split_group_1_", values));
            AddHistoryResume(_language.GetPrompt("__case_split_group_2_", values));
        }
        else if (m_PlayerCount == 2)
        {
            AddHistory(TextType.Event, _language.GetPrompt("__case_split_group_3_", values));
            AddHistoryResume(_language.GetPrompt("__case_split_group_4_", values));
        }
        else if (m_PlayerCount == 3)
        {
            AddHistory(TextType.Event, _language.GetPrompt("__case_split_group_5_", values));
            AddHistoryResume(_language.GetPrompt("__case_split_group_6_", values));
        }

        GroupIsReady = true;
    }
    public string GetNamePlayersNotInGroup()
    {
        PlayerData[] _pdt = _gameManager._charactersManagers.GetAllPlayers();
        if (_pdt.Length == m_PlayerCount)
        {
            return _language.GetText("__case_aucun_autre_joueur");
        }

        return string.Join("-", _gameManager._charactersManagers.GetAllPlayers()
            .Where(player => player._groupID != _groupID)
            .Select(player => player.playerName));
    }
    public void UpdateCanvasInventoryAllPlayer()
    {
        for (int i = 0; i < m_PlayerCount; i++)
        {
            _gameManager._UI_Inventaire_Buttons.Update_UI_Inventaire(m_GroupIndex, i);
        }
    }
    public void UpdateCanvasInventaire(PlayerData pd)
    {
        int indexPlayer = 0;
        for (int i = 0; i < m_PlayerCount; i++)
        {
            if (m_Players[i].playerIndex == pd.playerIndex)
            {
                indexPlayer = i;
                break;
            }
        }
        _gameManager._UI_Inventaire_Buttons.Update_UI_Inventaire(m_GroupIndex, indexPlayer);
    }
    #endregion

    #region Coord
    public void CheckIfDoorIsNearby(out bool _isnearby, out bool _haveKey)
    {
        _isnearby = false; _haveKey = false;
        //On vérifie si une case porte se trouve à une distance de 0 case 
        foreach (PlayerData player in m_Players)
        {
            Coord c = player.GetCoord();

            if (c.GetEvent() == EventCoord.DoorLocked)
            {
                _isnearby = true;
                _haveKey = false;
                return ;
            }
            else if (c.GetEvent() == EventCoord.DoorUnlocked)
            {
                _isnearby = true;
                _haveKey = true;
                return ;
            }
        }
    }

    public void CheckIfChestIsNearby(out bool _isnearby)
    {
        _isnearby = false;
        //On vérifie si une case porte se trouve à une distance de 0 case 
        foreach (PlayerData player in m_Players)
        {
            Coord c = player.GetCoord();
            HashSet<Coord> cc = c.GetNeighboursVisibles(1);
            foreach (Coord ccc in cc)
            {
                if (ccc.GetEvent() == EventCoord.Loot)
                {
                    _isnearby = true;
                    return;
                }
            }
        }
    }

    public string GetEventNeighbours(HashSet<Coord> voisin, HashSet<string> uniqueDescriptions, out bool doorIsVisible)
    {
        string toReturn = "";
        int countCNV = 0;
        doorIsVisible = false;
        foreach (Coord c in voisin)
        {
            if( c == null || voisin == null) { continue; }
            switch (c.GetEvent())
            {
                case EventCoord.Nothing:
                    if (c._isVisited && c._description != _language.GetText("__case__non_visitee__"))
                    {
                        if (!uniqueDescriptions.Contains(c._description))
                        {
                            toReturn += c._description + ";";
                            uniqueDescriptions.Add(c._description);
                        }
                    }
                    else
                    {
                        countCNV++;
                    }
                    break;
                case EventCoord.Wall:
                    break;
                case EventCoord.Loot:
                    toReturn += _language.GetText("__case__loot__");
                    break;
                case EventCoord.Enemy:
                    toReturn += _language.GetText("__case__ennemy__");
                    break;
                case EventCoord.DoorLocked:
                    toReturn += _language.GetText("__case__doorlocked__");
                    doorIsVisible = true;
                    break;
                case EventCoord.DoorUnlocked:
                    toReturn += _language.GetText("__case__doorunlocked__");
                    doorIsVisible = true;
                    break;
                case EventCoord.Boss:
                    break;
                default:
                    break;
            }
        }
        if (countCNV > 0)
        {
            toReturn += $"{countCNV} {_language.GetText("__case__non_visitee__minuscule")}";
        }
        return toReturn;
    }

    public bool IsEnemyNearbyTheGroup()
    {
        HashSet<Coord> EnnemyCoord = new HashSet<Coord>();
        foreach (PlayerData player in m_Players)
        {
            Coord playerCoord = player.GetCoord();
            HashSet<Coord> playerNeighbors = playerCoord.GetNeighbours(player.playerVision);
            foreach (Coord c in playerNeighbors)
            {
                if (c.GetEvent() == EventCoord.Enemy)
                {
                    EnnemyCoord.Add(c);
                }
            }
        }

        if(EnnemyCoord.Count == 0)
        {
            return false;
        }
        else if(EnnemyCoord.Count == 1)
        {
            return true;
        }
        else
        {
            int i = -1;
            foreach (Coord c in EnnemyCoord)
            {
                i++;
                if (i ==0)
                {
                    continue;
                }
                c._event = EventCoord.Nothing;
                c._description = _language.GetText("__case__non_visitee__");
            }
            return true;
        }
    }
    public void RemoveEnemyNearby()
    {
        foreach (PlayerData player in m_Players)
        {
            Coord playerCoord = player.GetCoord();
            HashSet<Coord> playerNeighbors = playerCoord.GetNeighbours(player.playerVision);
            foreach (Coord c in playerNeighbors)
            {
                if (c.GetEvent() == EventCoord.Enemy)
                {
                    c._description = _language.GetText("__case_restes_ennemies");
                    c._event = EventCoord.Nothing;
                }
            }
        }
                    return;
    }
}

    #endregion
