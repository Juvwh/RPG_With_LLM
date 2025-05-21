using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ToggleSwitch : MonoBehaviour
{
    public CombatManager _combatManager;
    public float slider_posX_OnAttaquer;
    public float slider_posX_OnSoigner;
    public bool isAttaquer = true;
    public float slider_Speed;
    public GameObject m_Slider;

    public void OnClick()
    {
        if (isAttaquer)
        {
            isAttaquer = false;
            transform.DOLocalMoveX(slider_posX_OnSoigner, slider_Speed);
            _combatManager.Combat_Player_Initialise_Health_Sequence();

        }
        else
        {
            isAttaquer = true;
            transform.DOLocalMoveX(slider_posX_OnAttaquer, slider_Speed);
            _combatManager.Combat_Player_Initialise_Attack_Sequence();
        }
    }

    public void Reset()
    {
        isAttaquer = true;
        transform.DOLocalMoveX(slider_posX_OnAttaquer, slider_Speed);
    }
}