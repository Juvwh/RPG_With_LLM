using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChoixLLM : MonoBehaviour
{
    #region Variables
    public GameManager _gm;
    public UI_ChoixLLManager _manager;
    UI_SelectPerso _SelectPerso;
    public GameObject _assombrir;
    private bool _selected = false;
    public Models.Model _llm;
    public bool oneIsSelected = false;
    public GameObject _mHideSpeed;
    public GameObject _mHideIntelligence;
    #endregion
    #region Methods
    public void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        _manager = FindFirstObjectByType<UI_ChoixLLManager>();
        _SelectPerso = FindFirstObjectByType<UI_SelectPerso>();
    }
    public void OnClick()
    {
        _manager._changeSelected(this);
        _gm.m_currentModel = _llm;
        _gm._charactersManagers.m_currentModel = _llm;
        _SelectPerso.m_ButtonConfirmerLLm.SetActive(true);
    }
    public void OnCanvasGroupChanged_(bool tf)
    {
        _assombrir.SetActive(!tf);
        _selected = tf;
    }
    public void OnTriggerEnterExit(bool tf)
    {
        if (_selected)
        {
            return;
        }
        if (tf) // On entre
        {
            //Le canvas est à sélectionne. Si aucun n'est déjà sélectionné, on l'assombrit
            //Si l'un est déjà sélectionné, alors on ne l'assombrie pas.
            _assombrir.SetActive(!oneIsSelected);
        }
        else
        {
            //On sort
            _assombrir.SetActive(oneIsSelected);
        }
    }
    public void OnclicUnHideSpeed() {
        _mHideSpeed.SetActive(false);
    }
    public void OnclicUnHideIntelligence()
    {
        _mHideIntelligence.SetActive(false);
    }
    #endregion
}


