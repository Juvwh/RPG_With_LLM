using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combat_CharacterInfos : MonoBehaviour
{
    [Header("Values")]
    public TextMeshProUGUI m_Text_Value_Sante;
    public TextMeshProUGUI m_Text_Value_Armure;
    public TextMeshProUGUI m_Text_Value_Arme;
    public TextMeshProUGUI m_Text_Value_Name;
    [Header("Color and Sprite")]
    public Color m_Color;
    public Image m_SpriteCharacter;
    public Image m_Cadre;
    [Header("Selectable")]
    public bool m_IsSelectable;
    public bool m_IsSelected;
    public bool m_IsPlaying = false;
    [Header("UI")]
    public GameObject m_UI;

    [Header("Manager")]
    public GameManager _gm; //GameManager

    public void Start()
    {
        m_UI = this.gameObject;
        _gm = FindFirstObjectByType<GameManager>();
    }
    public virtual void UpdateInventory() { }
    public virtual void ShowShadow(bool tf) { }
    public virtual void UpdateInformation() { }
    void UpdateItemUI(GameObject countObject, Image itemImage, TextMeshProUGUI countText, int itemCount) { }
    void SetSprite(Sprite sprite) { m_SpriteCharacter.sprite = sprite; }




}
