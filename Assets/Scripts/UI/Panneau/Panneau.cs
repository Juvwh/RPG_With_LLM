using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panneau : MonoBehaviour
{

    public Animator m_Animator;
    public MainMenu _mm;
    public GameObject _infoSupp;

    private void Start()
    {
        m_Animator.SetBool("PlayAnim", true);
        StartCoroutine(waitFalse());

    }
    IEnumerator waitFalse()
    {
        yield return new WaitForSeconds(0.5f);
        m_Animator.SetBool("PlayAnim", false);
    }

    public void OnTriggerEnterAndExit(bool tf)
    {
        m_Animator.SetBool("PlayAnim", tf);
        if (_infoSupp != null)
        {
            _infoSupp.SetActive(tf);
        }
    }
    public void OnClick(int nbr_panneau)
    {
        switch (nbr_panneau)
        {
            case 0:
                OnClick_Lancer();
                break;
            case 1:
                OnClick_PartieRapide();
                break;
            case 2:
                OnClick_TestSpec();
                break;
            case 3:
                OnClick_Questionnaire();
                break;
            case 4:
                OnClick_Credit();
                break;
            default:
                break;
        }
    }

    public void OnClick_Lancer()
    {
        _mm._onPlayButtonPressed();
    }
    public void OnClick_PartieRapide()
    {
        _mm._OnFastGamePressed();
    }
    public void OnClick_TestSpec()
    {
        _mm._OnTestPressed();
    }
    public void OnClick_Questionnaire()
    {
        _mm._OnQuestionnairePressed();
    }
    public void OnClick_Credit()
    {
        _mm._OnCreditsPressed();
    }
}
