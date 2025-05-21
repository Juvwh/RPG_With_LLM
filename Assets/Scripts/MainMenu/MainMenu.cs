using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Jobs;
using static Models;


public class MainMenu : MonoBehaviour
{
    #region Variables
    public GameObject Canvas_RGPD;
    public GameObject Canvas_Langue;
    public GameObject Canvas_Credit;
    public GameObject Canvas_Tests;
    public GameObject Canvas_Questionnaire;
    public GameObject Canvas_BouttonSupplieMoi;
    public TextMeshProUGUI m_TextSupplie;
    public AgentManager _am;
    public TMP_Dropdown _tpm;
    #endregion
    #region Methods
    public void Start()
    {
        Canvas_Langue.SetActive(true);
        StartCoroutine(generateSupplication());
    }
    IEnumerator generateSupplication()
    {
        string[] prompt = { "Supplie-moi, mais avec un ton plus grave.",
                            "Supplie-moi, mais comme si tu étais le Parrain de la mafia, et que tu faisais comprendre qu'il faut remplir ce formulaire.",
                            "Supplie-moi, mais en étant plus convaincant, en me promettant que tu m'offriras plein de choses en échange.",
                            "Supplie-moi, sur un ton très comique, en me faisant rire.",
                            "Supplie-moi, mais en étant très sérieux, comme si ta vie en dépendait.",
                            "Supplie-moi, mais en étant très poli, comme si tu étais un gentleman.",
                            "Supplie-moi, mais en étant très triste, comme si tu avais perdu tout espoir.",
                            "Supplie-moi, mais en étant très heureux, comme si tu venais de gagner à la loterie.",
                            "Supplie-moi, mais en étant très mystérieux, comme si tu cachais un secret.",
                            "Supplie-moi, mais en étant très convaincant, comme si tu avais vraiment besoin de moi.",
                            "Supplie-moi, mais en étant très sincère, comme si tu parlais avec ton cœur.",
                            "Supplie-moi, mais en étant très poétique, comme si tu écrivais un poème.",
                            "Supplie-moi, mais en étant très philosophique, comme si tu voulais me faire réfléchir.",

        };
        int randomIndex = Random.Range(0, prompt.Length);
        string selectedPrompt = prompt[randomIndex];
        bool done = false;
        string _textGenerer = "";
        string _textParser = "";
        while (!done)
        {
            yield return StartCoroutine(_am.Generate_Text(Models.Model.gemma2_9b_it, Job.Supplication, selectedPrompt, (response) =>
            {
                _textGenerer = response;
            }));

            done = Parser.BEGGING(_textGenerer,out _textParser);
            if (!done)
            {
                yield return new WaitForSeconds(1f);
            }
        }
        m_TextSupplie.text = _textParser;
        yield return new WaitForSeconds(1f);
        Canvas_BouttonSupplieMoi.SetActive(true);
    }
    public void _onPlayButtonPressed()
    {
        SceneManager.LoadScene("ThemeAndCharacters");
    }
    public void _OnLangueConfirmPressed()
    {
        Canvas_Langue.SetActive(false);
        Canvas_RGPD.SetActive(true);
    }
    public void _OnFastGamePressed()
    {
        SceneManager.LoadScene("2Launch");
    }
    public void _OnQuestionnairePressed()
    {
        Canvas_Questionnaire.SetActive(true);
    }
    public void _OnTestPressed()
    {
        Canvas_Tests.SetActive(true);
    }
    public void _OnClosedTestPressed()
    {
        Canvas_Tests.SetActive(false);
    }
    public void _OnClosedQuestionnairePressed()
    {
        Canvas_Questionnaire.SetActive(false);
    }
    public void OnClickContinuer()
    {
        Canvas_RGPD.SetActive(false);
    }
    public void _OnCreditsPressed()
    {
        Canvas_Credit.SetActive(true);
    }
    public void _OnClosedCreditsPressed()
    {
        Canvas_Credit.SetActive(false);
    }
    public void _OnClicQuestionnairePrincipal()
    {
        Application.OpenURL("https://forms.office.com/e/cw4wtMW27w");
    }
    public void _OnClicQuestionnaireNarration()
    {
        Application.OpenURL("https://forms.office.com/e/eex83GgeCK");
    }
    public void _OnClicQuestionnaireCombat()
    {
        Application.OpenURL("https://forms.office.com/e/NZr3zK3dyB");
    }
    public void _OnClicSupplieMoiEncore()
    {
        Canvas_BouttonSupplieMoi.SetActive(false);
        StartCoroutine(generateSupplication());
    }
    public void ChangeUIOptionLanguage(int index_language)
    {
        switch (index_language)
        {
            case 0:
                _tpm.value = 0;
                break;
            case 1:
                _tpm.value = 1;
                break;
            default:
                break;
        }
    }
    public void OnValueChangeDropDownMenu(int i)
    {

        LocalizationManager _language = FindObjectOfType<LocalizationManager>();
        _language.OnValueChangeDropDownMenu(i);
    }
#endregion
}
