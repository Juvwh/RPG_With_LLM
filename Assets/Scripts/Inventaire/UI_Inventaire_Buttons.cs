using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using UnityEngine.EventSystems;
using System;

public class UI_Inventaire_Buttons : MonoBehaviour
{
    #region Variables
    private GameManager _gameManager;
    public GameObject _explications_GO;
    public TextMeshProUGUI _explications_Text;
    public UI_Inventaire_Loot _LAST_UI_Inventaire_Loot;
    GameObject _canvasPlayerSelected;

    [Header("Positions")]
    public Vector3[] _explications_Positions_Health = new Vector3[4];
    public Vector3[] _explications_Positions_Attack = new Vector3[4];
    public Vector3[] _explications_Positions_Armor = new Vector3[4];
    public Vector3[] _explications_Positions_Coins = new Vector3[4];
    public Vector3[] _explications_Positions_Shoes = new Vector3[4];
    public Vector3[] _explications_Positions_Glasses = new Vector3[4];
    public Vector3[] _explications_Positions_Compass = new Vector3[4];
    public Vector3[] _explications_Positions_Key = new Vector3[4];

    [Header("Texts")]
    public string _explications_Health;
    public string _explications_Attack;
    public string _explications_Armor;
    public string _explications_Coins;
    public string _explications_Shoes;
    public string _explications_Glasses;
    public string _explications_Compass;
    public string _explications_Key;

    [Header("UI")]
    public GameObject _UI_LootFocus;
    public GameObject _UI_PlayerFocus;
    public GameObject Canvas_Loot;
    public GameObject[] Canvas_FocusPlayer;
    public GameObject Canvas_FocusPlayerGO;
    public GameObject Canvas_BlueBackGround;

    [Header("UI Inventaire")]
    public GameObject[] m_Canvas_Inventaires;

    [Header("PREFABS")]
    public GameObject _AreaSpriteLoot;
    public GameObject _Prefabs_Health;
    public GameObject _Prefabs_Attack;
    public GameObject _Prefabs_Armor;
    public GameObject _Prefabs_Coins;
    public GameObject _Prefabs_Shoes;
    public GameObject _Prefabs_Glasses;
    public GameObject _Prefabs_Compass;
    public GameObject _Prefabs_Key;

    [Header("Animation Coffre")]
    public GameObject _Prefabs_Coffre;
    private GameObject _InstanceCoffre;
    public Vector3 _PositionCoffre;
    public Vector3 _RotationCoffre;

    public int countItemToAttribute = 0;
    public bool isAttributingItem = false;

    public int currentItemToConfirm;
    public int currentcharacterToConfirm;
    #endregion
    #region Methods
    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        DesactiverEventTrigger();
        UpdateLanguage(_gameManager._language.currentLanguage);
    }

    public void Create_UI(List<string> tags, List<(string Nom, string Description)> items)
    {
        if(_InstanceCoffre != null)
        {
            Destroy(_InstanceCoffre);
        }
        Canvas_BlueBackGround.SetActive(true);
        //Canvas_Loot.SetActive(true);
        int i = 0;

        foreach (var tag in tags)
        {
            GameObject _prefab;

            switch (tag)
            {
                case "[HEALTH]":
                    _prefab = _Prefabs_Health;
                    break;
                case "[ATTACK]":
                    _prefab = _Prefabs_Attack;
                    break;
                case "[ARMOR]":
                    _prefab = _Prefabs_Armor;
                    break;
                case "[COINS]":
                    _prefab = _Prefabs_Coins;
                    break;
                case "[SHOES]":
                    _prefab = _Prefabs_Shoes;
                    break;
                case "[GLASSES]":
                    _prefab = _Prefabs_Glasses;
                    break;
                case "[COMPASS]":
                    _prefab = _Prefabs_Compass;
                    break;
                case "[KEY]":
                    _prefab = _Prefabs_Key;
                    break;
                default:
                    _prefab = null;
                    break;
            }
            GameObject _loot = Instantiate(_prefab, _AreaSpriteLoot.transform);
            UI_Inventaire_Loot script__loot = _loot.GetComponent<UI_Inventaire_Loot>();
            script__loot.m_TXT_Name.text = items[i].Nom;
            script__loot.m_TXT_Description.text = items[i].Description;
            i++;
        }

    }

    public void LaunchAnimationCoffre()
    {
        Canvas_BlueBackGround.SetActive(false);
        Canvas_Loot.SetActive(true);
        _InstanceCoffre = Instantiate(_Prefabs_Coffre, _PositionCoffre, Quaternion.Euler(_RotationCoffre));


    }

    private void DesactiverEventTrigger()
    {
        GameObject[] allCompositions = _gameManager._groupsManager.m_CanvasCompositions;

        for (int i = 0; i < allCompositions.Length; i++)
        {
            GameObject _compo = allCompositions[i];

            int nombreEnfants = _compo.transform.childCount;

            for (int j = 0; j < nombreEnfants; j++)
            {
                GameObject characters = _compo.transform.GetChild(j).transform.GetChild(0).gameObject;
                int countPlayerInsideCompo = characters.transform.childCount;

                for (int k = 0; k < countPlayerInsideCompo; k++)
                {
                    GameObject player = characters.transform.GetChild(k).gameObject;
                    if (player != null && player.GetComponent<EventTrigger>() != null)
                    {
                        player.GetComponent<EventTrigger>().enabled = false;
                    }
                }
            }
        }
    }


    //OnClick
    

    public void OnClick_Shoes(int character)
    {
        GameObject Icon = Get_Icon(4, character);
        
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            //SIMULER LA SELECTION D'UN PERSONNAGE SUR LA CARTE ET PERMETTRE LE DEPLACEMENT SUR TOUTE LES CASES POSSIBLES.
            PlayerData heros = GetPlayerByPositionOnUI(character); // GetPlayerByInt marche pas si plusieur groupe
            heros._specialMove = 1;
            _gameManager._mapManager.SimulatetileClickOnHero(heros.playerIndex);
        }

    }

    public PlayerData GetPlayerByInt(int character)
    {
       return _gameManager._groupsManager.m_Groups[_gameManager._UI_Manager.m_IndexCurrentGroupOnDisplay].m_Players[character];
    }

    public PlayerData GetPlayerByPositionOnUI(int character)
    {
        for (int g = 0; g < _gameManager._groupsManager.m_Groups.Length; g++)
        {
            if(_gameManager._groupsManager.m_Groups[g].m_PlayerCount > character)// is character in this group ?
            {
                return _gameManager._groupsManager.m_Groups[g].m_Players[character];// YES
            }
            character -= _gameManager._groupsManager.m_Groups[g].m_PlayerCount;// NO, minus lenght
        }
        return null;
    }

    //POINTER ENTER
    public GameObject Get_Icon(int icon, int character)
    {
        // 0 --> Health // 1 --> Attack // 2 --> Armor // 3 --> Coins // 4 --> Shoes // 5 --> Glasses // 6 --> Compass // 7 --> Key
        return _gameManager._UI_Manager.m_CurrentCanvasCharacter.transform.parent.transform.GetChild(0).transform.GetChild(character).transform.GetChild(icon).gameObject;
    }

    public void OnPointerEnter_HealthPotion(int character)
    {
        GameObject Icon = Get_Icon(0, character);
        if(Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Health[character];
            _explications_Text.text = _explications_Health;
        }
    }

    public void OnPointerEnter_AttackPotion(int character)
    {
        GameObject Icon = Get_Icon(1, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Attack[character];
            _explications_Text.text = _explications_Attack;
        }
    }

    public void OnPointerEnter_ArmorPotion(int character)
    {
        GameObject Icon = Get_Icon(2, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Armor[character];
            _explications_Text.text = _explications_Armor;
        }
    }

    public void OnPointerEntrer_Coins(int character)
    {
        GameObject Icon = Get_Icon(3, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Coins[character];
            _explications_Text.text = _explications_Coins;
        }
    }
    public void OnPointerEnter_Shoes(int character)
    {
        GameObject Icon = Get_Icon(4, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Shoes[character];
            _explications_Text.text = _explications_Shoes;
        }
    }
    public void OnPointerEnter_Glasses(int character)
    {
        GameObject Icon = Get_Icon(5, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Glasses[character];
            _explications_Text.text = _explications_Glasses;
        }
    }
    public void OnPointerEnter_Compass(int character)
    {
        GameObject Icon = Get_Icon(6, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Compass[character];
            _explications_Text.text = _explications_Compass;
        }
    }
    public void OnPointerEnter_Key(int character)
    {
        GameObject Icon = Get_Icon(7, character);
        if (Icon.GetComponent<Image>().color == Color.white)
        {
            _explications_GO.SetActive(true);
            _explications_GO.transform.localPosition = _explications_Positions_Key[character];
            _explications_Text.text = _explications_Key;
        }
    }

    //POINTER EXIT

    public void OnClickAttribuer(UI_Inventaire_Loot typeInventaire)
    {
        _LAST_UI_Inventaire_Loot = typeInventaire;
        _UI_LootFocus.SetActive(false);
        _UI_PlayerFocus.SetActive(true);

        //Cacher Le Selected Sur Tous Les Player Du Groupe
        Canvas_FocusPlayerGO.SetActive(true);
        ToggleSelectedColorForAllPlayers(false);
    }

    private void ToggleSelectedColorForAllPlayers(bool afficher)
    {
        int index_Group = _gameManager._groupsManager.m_CurrentGroupIndexOnDisplay;
        //On compte le nombre de player qui se trouve dans les groupes avec un index inférieur afin d'afficher le focus correcte sur les player sélectionnable
        int countPlayer = 0;
        for (int i = 0; i < index_Group; i++)
        {
            countPlayer += _gameManager._groupsManager.GetGroupByGroupIndex(i).m_Players.Length;
        }
        for(int j = 0 ; j < countPlayer; j++)
        {
            Canvas_FocusPlayer[j].SetActive(!afficher);
        }

        for (int i = 0; i < _gameManager._groupsManager.GetGroupByGroupIndex(index_Group).m_Players.Length; i++)
        {
            _canvasPlayerSelected = GetCanvasPlayerSelected(i);
            _canvasPlayerSelected.GetComponent<Canvas>().overrideSorting = !afficher;
            _canvasPlayerSelected.GetComponent<EventTrigger>().enabled = !afficher;
            _canvasPlayerSelected.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(afficher); //Selected_ColorX

            if(afficher) //On souhaite supprimer les boutons
            {
                Destroy(_canvasPlayerSelected.GetComponent<Button>());
            }
            else //On souhaite créer un bouton
            {
                Button button = _canvasPlayerSelected.AddComponent<Button>();
                button.transition = Selectable.Transition.None;
                int capturedIndex = i; // Capture la valeur de i par copie
                button.onClick.AddListener(() => this.OnClick_SelectPersonnage(capturedIndex));
            }

        }


    }

    public void OnPointerExitIcon(int character)
    {
        _explications_GO.SetActive(false);
    }

    private GameObject GetCanvasPlayerSelected(int character_index)
    {
        return _gameManager._UI_Manager.m_CurrentCanvasComposition.transform.GetChild(_gameManager._groupsManager.m_CurrentGroupIndexOnDisplay).transform.GetChild(0).transform.GetChild(character_index).gameObject;
    }
    #region Attribution Item
    public void OnTriggerEnter_SelectionCharacterToAddIntem(int index_Character)
    {
        _canvasPlayerSelected = GetCanvasPlayerSelected(index_Character);
        _canvasPlayerSelected.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true); //Selected_ColorX --> actif
    }
    public void OnTriggerExit_SelectionCharacterToAddIntem(int index_Character)
    {
        _canvasPlayerSelected.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
    }
    public void OnClick_SelectPersonnage(int index_Character)
    {
        int index_Group = _gameManager._groupsManager.m_CurrentGroupIndexOnDisplay;
        PlayerData _selectedpPlayerData = _gameManager._groupsManager.GetPlayerByGroup(index_Group, index_Character);
        switch (_LAST_UI_Inventaire_Loot.m_Loot_Type)
        {
            case "[HEALTH]":
                _selectedpPlayerData.AddInventoryHealthPotion(1);
                break;
            case "[ATTACK]":
                _selectedpPlayerData.AddInventoryAttackPotion(1);
                break;
            case "[ARMOR]":
                _selectedpPlayerData.AddInventoryArmorPotion(1);
                break;
            case "[COINS]":
                _selectedpPlayerData.AddInventoryCoins(1);
                break;
            case "[SHOES]":
                _selectedpPlayerData.AddInventoryShoes(1);
                break;
            case "[GLASSES]":
                _selectedpPlayerData.AddInventoryGlasses(1);
                _selectedpPlayerData.playerVision += 1;
                _selectedpPlayerData._playerGroup.UpdateImageCanvas();// update vision dans UI
                _gameManager._mapManager.rebuildVector(new Vector2Int(0,0));
                break;
            case "[COMPASS]":
                _selectedpPlayerData.AddInventoryCompass(1);
                _gameManager._mapManager._coordPorte._isVisible = true;
                _gameManager._mapManager.rebuildVector(new Vector2Int(0, 0));
                break;
            case "[KEY]":
                _selectedpPlayerData.AddInventoryKeys(1);
                transformDoorLockedToUnlocked();

                break;
            default:
                break;
        }
        Destroy(_LAST_UI_Inventaire_Loot.gameObject);
        _UI_LootFocus.SetActive(true);
        _UI_PlayerFocus.SetActive(false);
        Canvas_FocusPlayerGO.SetActive(false);
        countItemToAttribute--;
        ToggleSelectedColorForAllPlayers(true);
        if (countItemToAttribute <= 0)
        {
            isAttributingItem = false;
            Canvas_Loot.SetActive(false);
            
            _gameManager._UI_Manager.SwitchGroupCanvas(index_Group);
        }
    }
    #endregion

    public void Update_UI_Inventaire(int groupIndex, int playerIndexInsideGroup)
    {
        if(_gameManager == null)
        {
            _gameManager = FindFirstObjectByType<GameManager>();
        }
        int countPlayer = 0;
        for (int i = 0; i < groupIndex; i++)
        {
            countPlayer += _gameManager._groupsManager.GetGroupByGroupIndex(i).m_Players.Length;
        }
        PlayerData _playerData = _gameManager._groupsManager.GetPlayerByGroup(groupIndex, playerIndexInsideGroup);
        int[] items = _playerData.GetAllItemsCount();
        countPlayer += playerIndexInsideGroup;
        for (int i = 0; i < items.Length; i++)
        {
            m_Canvas_Inventaires[countPlayer].transform.GetChild(i).GetComponent<Image>().color = items[i] > 0 ? Color.white : Color.black;
            if(items[i] > 0)
            {
                m_Canvas_Inventaires[countPlayer].transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = items[i].ToString();
            }
            else
            {
                m_Canvas_Inventaires[countPlayer].transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    public void transformDoorLockedToUnlocked()
    {
        foreach (var coord in _gameManager._mapManager.m_CoordSet)
        {
            if(coord.Value._event == Events.EventCoord.DoorLocked)
            {
                coord.Value._event = Events.EventCoord.DoorUnlocked;
            }
        }
    }
    
    public void UpdateLanguage(string language)
    {
        _explications_Health = _gameManager._language.GetText("inventaire_explications_potion_soin");
        _explications_Attack = _gameManager._language.GetText("inventaire_explications_potion_attaque");
        _explications_Armor = _gameManager._language.GetText("inventaire_explications_potion_armure");
        _explications_Coins = _gameManager._language.GetText("inventaire_explications_argent");
        _explications_Shoes = _gameManager._language.GetText("inventaire_explications_chaussure");
        _explications_Glasses = _gameManager._language.GetText("inventaire_explications_lunettes");
        _explications_Compass = _gameManager._language.GetText("inventaire_explications_boussole");
        _explications_Key = _gameManager._language.GetText("inventaire_explications_key");
    }
    #endregion
}
