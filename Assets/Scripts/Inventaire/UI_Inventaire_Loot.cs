using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class UI_Inventaire_Loot : MonoBehaviour
{
    #region Variables
    GameManager _gameManager;
    public TextMeshProUGUI m_TXT_Name;
    public TextMeshProUGUI m_TXT_Description;
    public Transform m_ICON_Transform;
    public string m_Loot_Type;
    #endregion
    #region Methods
    private void Start()
    {
        _gameManager = _gameManager = FindFirstObjectByType<GameManager>();
    }
    public void OnClickAttribuer()
    {
        _gameManager._UI_Inventaire_Buttons.OnClickAttribuer(this);
    }

    public void SetName(string name)
    {
        m_TXT_Name.text = name;
    }
    public void SetDescription(string description)
    {
        m_TXT_Description.text = description;
    }

    public void OnTriggerEnterIcon(string loot)
    {

        GameObject _explications_GO = _gameManager._UI_Inventaire_Buttons._explications_GO;
        _explications_GO.SetActive(true);
        _explications_GO.transform.position = m_ICON_Transform.position + new Vector3(0, 70, 0);
        
        string _explications = "";
        switch (loot)
        {
            case "[ARMOR]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Armor; break;
            case "[HEALTH]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Health; break;
            case "[ATTACK]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Attack; break;
            case "[COINS]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Coins; break;
            case "[KEY]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Key; break;
            case "[GLASSES]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Glasses; break;
            case "[COMPASS]":
                _explications = _gameManager._UI_Inventaire_Buttons._explications_Compass; break;
            default:
                break;
        }

        _gameManager._UI_Inventaire_Buttons._explications_Text.text = _explications;
    }

    public void OnTriggerExitIcon()
    {
        _gameManager._UI_Inventaire_Buttons._explications_GO.SetActive(false);
    }
    #endregion
}
