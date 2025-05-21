using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeAndPersoButtons : MonoBehaviour
{
    public UI_SelectPerso m_UI_SelectPerso;
    public CharacterGridManager CharacterGridManager;

    public void OnClickButtonTheme(string theme)
    {
        StartCoroutine(OnClickButtonTheme_Coroutine(theme));
    }
    public IEnumerator OnClickButtonTheme_Coroutine(string theme)
    {
        UI_SelectPerso.Theme t = m_UI_SelectPerso.StringToTheme(theme);
        CharacterGridManager = m_UI_SelectPerso.CharacterGridManager;
        if (t == UI_SelectPerso.Theme.MyChoice)
        {
            m_UI_SelectPerso.m_CurrentTheme = t;
            m_UI_SelectPerso.Afficher_CreateTheme();
            yield break;
        }

        yield return StartCoroutine(m_UI_SelectPerso.SwitchTheme(t));
        m_UI_SelectPerso.Afficher_CharacterSelection();
    }

    public void OnClickModifierButton(int numCharacter)
    {
        m_UI_SelectPerso.m_CurrentCharacter = numCharacter;
        m_UI_SelectPerso.Afficher_EditCharacters();

        m_UI_SelectPerso.LoadCharacterSelected();

    }
    public void OnClickAddPersoButton(int numCharacter)
    {
        m_UI_SelectPerso.m_CurrentCharacter = numCharacter;
        m_UI_SelectPerso.Afficher_EditCharacters();
        m_UI_SelectPerso.CreateNewCharacter();
        
    }

    public void OnClickCreateNewCharacter()
    {
        m_UI_SelectPerso.CreateNewCharacter();
    }

    public void OnClickOnEnregistrer()
    {
        m_UI_SelectPerso.SaveCharacter();
    }

    public void OnClickGenerate()
    {
        m_UI_SelectPerso.GenerateNewPersonnage();
    }

    public void OnClickRemove(int character)
    {
        m_UI_SelectPerso.RemoveCharacter(character);
    }

    public void OnClickCommencer()
    {
        m_UI_SelectPerso.Commencer();
    }

    public void OnClickSendCreateTheme()
    {
        StartCoroutine(m_UI_SelectPerso.Verify_Coherence_CreateTheme());
    }

    public void OnClickRetour()
    {
        m_UI_SelectPerso.Afficher_ThemeSelection();
    }
    public void OnClickChoixTheme()
    {
        if(m_UI_SelectPerso.m_CurrentTheme == UI_SelectPerso.Theme.MyChoice)
        {
            m_UI_SelectPerso.Afficher_CreateTheme();
        }
        else
        {
            m_UI_SelectPerso.Afficher_ThemeSelection();
        }
    }
}
