using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class S_LoadingAnimationEvent : MonoBehaviour
{
    public Text m_LoadingText;
    public string[] possiblesText = new string[20];
    public string[] possiblesText_EN = new string[20];
    public string[] possiblesText_FR = new string[20];
    private int m_Index = -1;
    private HashSet<int> m_Loaded = new HashSet<int>();
    public LocalizationManager m_LocalizationManager;
    public void Start()
    {
        m_LocalizationManager = FindObjectOfType<LocalizationManager>();
        UpdateLanguage(m_LocalizationManager.currentLanguage);
    }
    public void UpdateLanguage(string language)
    {
        switch (language)
        {
            case "fr":
                possiblesText = possiblesText_FR;
                break;
            case "en":
                possiblesText = possiblesText_EN;
                break;
            default:
                break;
        }
    }
    public void ChangeText()
    {
        m_Index++;
        if(m_Index == possiblesText.Length)
        {
            m_Index = 0;
        }
        m_LoadingText.text = possiblesText[m_Index];
    }
}
