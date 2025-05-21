using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Language_Panneau : MonoBehaviour
{
    public Image panneau_lancer;
    public Image panneau_partieRapide;
    public Image panneau_tests;
    public Image panneau_questionnaire;
    public Image panneau_credit;

    public Sprite[] lancer;
    public Sprite[] partieRapide;
    public Sprite[] tests;
    public Sprite[] questionnaire;
    public Sprite[] credit;


    private void Start()
    {
        LocalizationManager _language = FindObjectOfType<LocalizationManager>();
        UpdateLanguage(_language.currentLanguage);
    }

    public void UpdateLanguage(string currentLangue)
    {
        switch (currentLangue)
        {
            case "fr":
                panneau_lancer.sprite = lancer[0];
                panneau_partieRapide.sprite = partieRapide[0];
                panneau_tests.sprite = tests[0];
                panneau_questionnaire.sprite = questionnaire[0];
                panneau_credit.sprite = credit[0];
                break;
            case "en":
                panneau_lancer.sprite = lancer[1];
                panneau_partieRapide.sprite = partieRapide[1];
                panneau_tests.sprite = tests[1];
                panneau_questionnaire.sprite = questionnaire[1];
                panneau_credit.sprite = credit[1];
                break;
            default:
                break;
        }
    }

}
