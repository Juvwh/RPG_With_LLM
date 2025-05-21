using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combat_PlayerInfo : Combat_CharacterInfos
{
    [Header("Player")]
    public PlayerData m_PlayersLinked;
    [Header("Values")]
    public TextMeshProUGUI m_Text_Value_Precision;
    [Header("UI")]
    public GameObject m_InfoArme;
    public TextMeshProUGUI m_Text_InfoArme;

    [Header("Inventaire")]
    public GameObject m_Healt;
    public GameObject m_Attack;
    public GameObject m_Armor;
    public GameObject m_Coin;

    public GameObject m_HealtCount;
    public GameObject m_AttackCount;
    public GameObject m_ArmorCount;
    public GameObject m_CoinCount;

    public TextMeshProUGUI m_Txt_HealtCount;
    public TextMeshProUGUI m_Txt_AttackCount;
    public TextMeshProUGUI m_Txt_ArmorCount;
    public TextMeshProUGUI m_Txt_CoinCount;

    public TextMeshProUGUI m_Txt_HealtTitle;
    public TextMeshProUGUI m_Txt_AttackTitle;
    public TextMeshProUGUI m_Txt_ArmorTitle;
    public TextMeshProUGUI m_Txt_PrecisionTitle;

    [Header("Inventaire")]
    public bool _canUseInventory;
    public GameObject m_Confirmation;
    public GameObject m_imgConfirmation;
    public GameObject m_CheckOkButton;
    public Vector3[] m_ConfirmationPositions;
    private GameObject[] m_Inventory;

    public int _bonnusAttaque = -1;
    public GameObject m_BonusArme;
    public TextMeshProUGUI m_BonusArmeText;
    public int _bonusArmure = 0;
    public GameObject m_BonusArmure;
    public TextMeshProUGUI m_BonusArmureText;


    [Header("Others")]
    public bool _onContactDescrArme = false;
    public bool _onContactConvas = false;
    public GameObject m_DeathVector;
    public bool _isDead = false;

    [Header("Private")]
    private bool _isDark;

    private void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        m_DeathVector.SetActive(false);
        m_UI = this.gameObject;
        m_Inventory = new GameObject[] { m_Healt, m_Attack, m_Armor, m_Coin };
    }
    public void Reset()
    {
        m_PlayersLinked = null;
        m_IsSelected = false;
        m_IsPlaying = false;
        _onContactDescrArme = false;
        _onContactConvas = false;
        m_DeathVector.SetActive(false);
        _isDead = false;
        _bonnusAttaque = -1;
        _bonusArmure = 0;
        m_BonusArme.SetActive(false);
        m_BonusArmure.SetActive(false);
        _canUseInventory = false;

    }
    public override void ShowShadow(bool tf)
    {
        if (_isDead) { return;}
        if (tf) //Assombrir
        {
            m_SpriteCharacter.color = new Color(0.2f, 0.2f, 0.2f, 1);
            m_Cadre.color = new Color(m_Color.r, m_Color.g, m_Color.b, 0.3f);


            m_Txt_HealtTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Txt_AttackTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Txt_ArmorTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Txt_PrecisionTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);

            Color c = new Color(1, 1, 1, 0.3f);
            m_Txt_HealtCount.color = c;
            m_Txt_AttackCount.color = c;
            m_Txt_ArmorCount.color = c;
            m_Txt_CoinCount.color = c;

            m_Text_Value_Name.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Text_Value_Sante.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Text_Value_Armure.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Text_Value_Arme.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);
            m_Text_Value_Precision.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 0.3f);

            m_Healt.GetComponent<Image>().color = c;
            m_Attack.GetComponent<Image>().color = c;
            m_Armor.GetComponent<Image>().color = c;
            m_Coin.GetComponent<Image>().color = c;
        }
        else //Remettre normal
        {
            m_SpriteCharacter.color = Color.white;
            m_Cadre.color = m_Color;

            m_Text_Value_Name.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);

            m_Text_Value_Name.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Text_Value_Sante.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Text_Value_Armure.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Text_Value_Arme.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Text_Value_Precision.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);

            m_Txt_ArmorTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Txt_HealtTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Txt_AttackTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);
            m_Txt_PrecisionTitle.color = new Color(m_Text_Value_Name.color.r, m_Text_Value_Name.color.g, m_Text_Value_Name.color.b, 1);


            m_Txt_HealtCount.color = Color.white;
            m_Txt_AttackCount.color = Color.white;
            m_Txt_ArmorCount.color = Color.white;
            m_Txt_CoinCount.color = Color.white;

            Color c = new Color(1, 1, 1, 0.3f);
            m_Healt.GetComponent<Image>().color = m_Txt_HealtCount.text == "0" ? c : Color.white;
            m_Attack.GetComponent<Image>().color = m_Txt_HealtCount.text == "0" ? c : Color.white;
            m_Armor.GetComponent<Image>().color = m_Txt_HealtCount.text == "0" ? c : Color.white;
            m_Coin.GetComponent<Image>().color = m_Txt_HealtCount.text == "0" ? c : Color.white;

        }
        CanUseInventory(_canUseInventory);
       _isDark = tf;
    }
    public override void UpdateInventory()
    {
        UpdateItemUI(m_HealtCount, m_Healt.GetComponent<Image>(), m_Txt_HealtCount, m_PlayersLinked.playerInventory_HealthPotions);
        UpdateItemUI(m_AttackCount, m_Attack.GetComponent<Image>(), m_Txt_AttackCount, m_PlayersLinked.playerInventory_AttackPotions);
        UpdateItemUI(m_ArmorCount, m_Armor.GetComponent<Image>(), m_Txt_ArmorCount, m_PlayersLinked.playerInventory_ArmorPotions);
        UpdateItemUI(m_CoinCount, m_Coin.GetComponent<Image>(), m_Txt_CoinCount, m_PlayersLinked.playerInventory_Coins); 
    }

    public override void UpdateInformation()
    {
        m_Text_Value_Name.text = m_PlayersLinked.playerName;
        m_Text_Value_Sante.text = m_PlayersLinked.playerHealth.ToString();
        m_Text_Value_Armure.text = m_PlayersLinked.playerArmorClass.ToString();
        m_Text_Value_Arme.text = m_PlayersLinked.playerArmePuissance.ToString();
        m_Text_Value_Precision.text = m_PlayersLinked.playerAimBonus.ToString();
        m_Text_InfoArme.text = $"{m_PlayersLinked.playerName} {m_PlayersLinked._playerGroup._gameManager._language.GetText("_player_arme")} {m_PlayersLinked.playerArme}.";
        m_SpriteCharacter.sprite = m_PlayersLinked.playerSprite;
        CanUseInventory(_canUseInventory);
    }

    void UpdateItemUI(GameObject countObject, Image itemImage, TextMeshProUGUI countText, int itemCount)
    {
        if (itemCount != 0)
        {
            countObject.SetActive(true);
            itemImage.color = Color.white;
            countText.text = itemCount.ToString();
        }
        else
        {
            countObject.SetActive(false);
            itemImage.color = new Color(0, 0, 0, 1);
        }
    }





    #region Trigger
    public void OnTriggerEnterPlayer()
    {
        if(_isDead) { return; }
        _onContactConvas = true;
        if (_isDark == false)
        {
            m_InfoArme.SetActive(true);
        }
        //Si c'est selectionnable mais assombrit, quand on passe dessus on remet en couleur
        //Si c'esy selectionnable et en couleur, on assombrit
        if (m_IsSelectable && !m_IsSelected)
        {
            if (_gm._combatManager._selected == null) // Si aucun heros n'est encore sélectionné, on va rendre le héros assombrit, sinon on le rend clair
            {
                ShowShadow(true);
            }
            else
            {
                ShowShadow(false);
            }

        }
    }
    public void OnTriggerExitSelection()
    {
        if (_isDead) {return;}
        _onContactConvas = false;
        StartCoroutine(WaitForFrames(5));
        //Si c'est selectionnable mais assombrit, quand on passe dessus on remet en couleur
        //Si c'est selectionnable et en couleur, on assombrit
        if (m_IsSelectable && !m_IsSelected)
        {
            if (_gm._combatManager._selected == null) // Si aucun player n'est encore sélectionné, on va rendre le player clair, sinon on le rend assombrit
            {
                ShowShadow(false);
            }
            else
            {
                ShowShadow(true);
            }

        }
    }

    public void OnClickToSelect()
    {
        if (m_IsSelected || _isDead) { return; } //Si c'est deja selectionné on ne fait rien
        if (m_IsSelectable)
        {
            //Si un autre est selectionné, on le deselectionne et on le passe en assombrit
            if (_gm._combatManager._selected != null)
            {
                _gm._combatManager._selected.ShowShadow(true);
                _gm._combatManager._selected.m_IsSelected = false;
            }
            else
            {
                //Rendre tous les autres ennemies sombres.
                foreach (GameObject _player in _gm._combatManager._HerosGameobject)
                {
                    if (_player.GetComponent<Combat_PlayerInfo>()._isDead || !_player.activeSelf) { continue; }
                    Combat_PlayerInfo _playerInfo = _player.GetComponent<Combat_PlayerInfo>();
                    if (_playerInfo  != this)
                    {
                        _playerInfo.ShowShadow(true);
                    }
                }
            }
            //On selectionne celui-ci à la place
            m_IsSelected = true;
            ShowShadow(false);
            _gm._combatManager._selected = this;

            //On va pouvoir envoyer le prompt si il y a un selectionné
            _gm._combatManager.TurnSendPromptButton(true);
        }
    }

    public void OnTriggerDescrArme(bool tf)
    {
        _onContactDescrArme = tf;
        StartCoroutine(WaitForFrames(5));
    }
    private IEnumerator WaitForFrames(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null; ;
        }

        if(!_onContactConvas && !_onContactDescrArme)
        {
            m_InfoArme.SetActive(false);
        }
    }

    #endregion
    #region Inventaire

    public void CanUseInventory(bool _canUse)
    {
        _canUseInventory = _canUse;
        UpdateInventory();
        Color c = new Color(1, 1, 1, 0.3f);
        if (_canUseInventory)
        {
            m_Healt.GetComponent<Image>().color = m_PlayersLinked.playerInventory_HealthPotions > 0 ? Color.white : c;
            m_Attack.GetComponent<Image>().color = m_PlayersLinked.playerInventory_AttackPotions > 0 ? Color.white : c;
            m_Armor.GetComponent<Image>().color = m_PlayersLinked.playerInventory_ArmorPotions > 0 ? Color.white : c;
            m_Coin.GetComponent<Image>().color = m_PlayersLinked.playerInventory_Coins > 0 ? Color.white : c;
        }
        else
        {
            m_Healt.GetComponent<Image>().color = c;
            m_Attack.GetComponent<Image>().color = c;
            m_Armor.GetComponent<Image>().color = c;
            m_Coin.GetComponent<Image>().color = c;
        }
    }

    public void OnClickItemButton(int _index)
    {
        //On peut interragir uniquement si c'est le tour de ce personnage
        if (!_canUseInventory)  {return; }
        switch (_index)
        {
            case 1:
                if (m_PlayersLinked.playerInventory_HealthPotions > 0 && m_Healt.GetComponent<Image>().color == Color.white)
                {
                    AfficherConfirmationButton(1);
                }
                break;
            case 2:
                if (m_PlayersLinked.playerInventory_AttackPotions > 0 && m_Attack.GetComponent<Image>().color == Color.white)
                {
                    AfficherConfirmationButton(2);
                }
                break;
            case 3:
                if (m_PlayersLinked.playerInventory_ArmorPotions > 0 && m_Armor.GetComponent<Image>().color == Color.white)
                {
                    AfficherConfirmationButton(3);
                }
                break;
            case 4:
                if (m_PlayersLinked.playerInventory_Coins > 0 && m_Coin.GetComponent<Image>().color == Color.white)
                {
                    AfficherConfirmationButton(4);
                }
                break;
            default:
                break;
        }
    }

    public void AfficherConfirmationButton(int _index)
    {
        m_Confirmation.SetActive(true);
        m_imgConfirmation.transform.localPosition = m_ConfirmationPositions[_index-1];
        //Donne au gameobject checkokbutton la fonction OnClickCheckOkButton.
        m_CheckOkButton.GetComponent<Button>().onClick.RemoveAllListeners(); //On Retire l'ancien Listener
        m_CheckOkButton.GetComponent<Button>().onClick.AddListener(() => OnClickCheckOkButton(_index)); //On ajoute le nouveau listener
    }

    public void OnClickCrossButton()
    {
        //Enlever au gameobject checkokbutton la fonction OnClickCheckOkButton.
        m_Confirmation.SetActive(false);
    }
    public void OnClickCheckOkButton(int _index)
    {
        m_Confirmation.SetActive(false);
        switch (_index)
        {
            case 1: OnClick_Inventory_Health(); break;
            case 2: OnClick_Inventory_Attaque();  break;
            case 3: OnCLick_Inventory_Armure(); break;
            case 4: OnClick_Inventory_Coin(); break;
            default: break;
        }
    }

    public void OnClick_Inventory_Health()
    {
        m_PlayersLinked.playerInventory_HealthPotions--;
        m_PlayersLinked.playerHealth = m_PlayersLinked._playerGroup._gameManager._language.GetText("_player_forme");
        m_PlayersLinked.playerHealthNbr = 20;
        UpdateInventory();
        UpdateInformation();
    }
    public void OnClick_Inventory_Attaque()
    {
        m_PlayersLinked.playerInventory_AttackPotions--;
        if(_bonnusAttaque == -1)
        {
            _bonnusAttaque = 2;
        }
        else if(_bonnusAttaque >=10)
        {
            _bonnusAttaque += 10;
        }
        else
        {
            _bonnusAttaque += 2;
        }
        m_BonusArme.SetActive(true);
        m_BonusArmeText.text = $"+{_bonnusAttaque}";
        UpdateInventory();
        UpdateInformation();
    }
    public void OnCLick_Inventory_Armure()
    {
        m_PlayersLinked.playerInventory_ArmorPotions--;
        if (_bonusArmure == -1)
        {
            _bonusArmure = 2;
        }
        else if (_bonusArmure >= 10)
        {
            _bonusArmure += 10;
        }
        else
        {
            _bonusArmure += 2;
        }
        m_BonusArmure.SetActive(true);
        m_BonusArmureText.text = $"+{_bonusArmure}";
        UpdateInventory();
        UpdateInformation();
    }
    public void OnClick_Inventory_Coin()
    {
        CanUseInventory(false);
        m_PlayersLinked.playerInventory_Coins--;
        _gm._UI_Manager.Show_Corruption_Canvas(true);
        _gm._combatManager.Choice_Enemy_For_Corruption();
    }



    #endregion
}
