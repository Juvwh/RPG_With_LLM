using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCharacter : MonoBehaviour
{
    public TextMeshProUGUI m_NameTextObject;
    public GameObject m_imageBoxPlayer;
    public Image m_PlayerImage;
    public GameObject m_ButtonModifier;
    public GameObject m_ButtonAdd;
    public GameObject m_ButtonRetirer;

    public PlayerData m_PlayerData;

    public void SetSelectedCharacter(PlayerData pd, Image img)
    {
        m_PlayerData = pd;
        m_NameTextObject.text = pd.playerName;
        m_PlayerImage.sprite = img.sprite;
        m_NameTextObject.text = pd.playerName;
        m_imageBoxPlayer.SetActive(true);
        m_ButtonModifier.SetActive(true);
        m_ButtonRetirer.SetActive(true);
        m_ButtonAdd.SetActive(false);

    }

    public void RemoveSelectedCharacter()
    {
        m_PlayerData = null;
        m_PlayerImage.sprite = null;
        m_NameTextObject.text = "";
        m_imageBoxPlayer.SetActive(false);
        m_ButtonModifier.SetActive(false);
        m_ButtonRetirer.SetActive(false);
        m_ButtonAdd.SetActive(true);
    }



}
