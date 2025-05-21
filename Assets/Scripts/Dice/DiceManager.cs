using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    #region Variables
    private GameManager gameManager;
    public GameObject _parent;
    public GameObject m_Prefab_Dice_4;
    public GameObject m_Prefab_Dice_6;
    public GameObject m_Prefab_Dice_8;
    public GameObject m_Prefab_Dice_10;
    public GameObject m_Prefab_Dice_20;
    private Dice3D dice_4;
    private Dice3D dice_6;
    private Dice3D dice_8;
    private Dice3D dice_10;
    private Dice3D dice_20;
    private Dice3D lastDice_1;
    private Dice3D lastDice_2;
    private Dice3D lastDice_3;
    public int _lastResult = -1;
    public int[] debugDice;
    private int nbr_Dice = 1;
    [Header("UI")]
    public GameObject m_Camera3D;
    public GameObject m_CanvasDice2D;
    public GameObject m_CanvasDice3D;
    public GameObject m_RollDiceButton;
    public GameObject m_ResultatTextGO;
    public TextMeshProUGUI m_ResultatTexttxt;
    public Camera mainCamera;  // La caméra principale
    [Header("Positions")]
    public Vector3 Start_Position_1_Dice = new Vector3(-5, 12, 0);
    public Vector3[] Start_Position_2_Dice;
    public Vector3[] Start_Position_3_Dice;
    #endregion
    #region Methods
    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        gameManager._diceManager = this;
    }

    public IEnumerator roll(int[] nmb_Faces)
    {
        //  Attendre que le lancer soit terminé
        int result = -1;
        yield return StartCoroutine(Roll_Dice(debugDice[0], debugDice[1], debugDice[2], (response) => { result = response; }));
    }
    public IEnumerator Roll_Dice(int nbr_Faces_Dice_1, int nbr_Faces_Dice_2 , int nbr_Faces_Dice_3 , Action<int> _callback)
    {
        _lastResult = -1;
        lastDice_1 = null;
        lastDice_2 = null;
        lastDice_3 = null;


        ShowDiceUI(true);
        Vector3 pos_Dice_1 = new Vector3(0, 0, 0);
        Vector3 pos_Dice_2 = new Vector3(0, 0, 0);
        Vector3 pos_Dice_3 = new Vector3(0, 0, 0);


        if (nbr_Faces_Dice_2 == -1 && nbr_Faces_Dice_3 == -1)
        {
            //un seul dé
            nbr_Dice = 1;
            pos_Dice_1 = Start_Position_1_Dice;
        }
        else if(nbr_Faces_Dice_2 != -1 && nbr_Faces_Dice_3 == -1)
        {
            // deux dés
            nbr_Dice = 2;
            pos_Dice_1 = Start_Position_2_Dice[0];
            pos_Dice_2 = Start_Position_2_Dice[1];
        }
        else 
        {
            //trois dés
            nbr_Dice = 3;
            pos_Dice_1 = Start_Position_3_Dice[0];
            pos_Dice_2 = Start_Position_3_Dice[1];
            pos_Dice_3 = Start_Position_3_Dice[2];
        }

        switch (nbr_Faces_Dice_1)
        {
            case 4:
                //Instancier Un Dice ici
                GameObject _dice_4 = Instantiate(m_Prefab_Dice_4);
                _dice_4.transform.SetParent(_parent.transform, false);
                dice_4 = _dice_4.GetComponent<Dice3D>();
                lastDice_1 = dice_4;
                dice_4.ResetDice(true, pos_Dice_1);
                break;
            case 6:
                GameObject _dice_6 = Instantiate(m_Prefab_Dice_6);
                _dice_6.transform.SetParent(_parent.transform, false);
                dice_6 = _dice_6.GetComponent<Dice3D>();
                lastDice_1 = dice_6;
                dice_6.ResetDice(true, pos_Dice_1);
                break;
            case 8:
                GameObject _dice_8 = Instantiate(m_Prefab_Dice_8);
                _dice_8.transform.SetParent(_parent.transform, false);
                dice_8 = _dice_8.GetComponent<Dice3D>();
                lastDice_1 = dice_8;
                dice_8.ResetDice(true, pos_Dice_1);
                break;
            case 10:
                GameObject _dice_10 = Instantiate(m_Prefab_Dice_10);
                _dice_10.transform.SetParent(_parent.transform, false);
                dice_10 = _dice_10.GetComponent<Dice3D>();
                lastDice_1 = dice_10;
                dice_10.ResetDice(true, pos_Dice_1);
                break;
            case 20:
                GameObject _dice_20 = Instantiate(m_Prefab_Dice_20);
                _dice_20.transform.SetParent(_parent.transform, false);
                dice_20 = _dice_20.GetComponent<Dice3D>();
                lastDice_1 = dice_20;
                dice_20.ResetDice(true, pos_Dice_1);
                break;
            default:
                break;
        }


        if(nbr_Faces_Dice_2 != -1)
        {
            switch (nbr_Faces_Dice_2)
            {
                case 4:

                    GameObject _dice_4 = Instantiate(m_Prefab_Dice_4);
                    _dice_4.transform.SetParent(_parent.transform, false);
                    dice_4 = _dice_4.GetComponent<Dice3D>();
                    lastDice_2 = dice_4;
                    dice_4.ResetDice(true, pos_Dice_2);
                    break;
                case 6:
                    GameObject _dice_6 = Instantiate(m_Prefab_Dice_6);
                    _dice_6.transform.SetParent(_parent.transform, false);
                    dice_6 = _dice_6.GetComponent<Dice3D>();
                    lastDice_2 = dice_6;
                    dice_6.ResetDice(true, pos_Dice_2);
                    break;
                case 8:
                    GameObject _dice_8 = Instantiate(m_Prefab_Dice_8);
                    _dice_8.transform.SetParent(_parent.transform, false);
                    dice_8 = _dice_8.GetComponent<Dice3D>();
                    lastDice_2 = dice_8;
                    dice_8.ResetDice(true, pos_Dice_2);
                    break;
                case 10:
                    GameObject _dice_10 = Instantiate(m_Prefab_Dice_10);
                    _dice_10.transform.SetParent(_parent.transform, false);
                    dice_10 = _dice_10.GetComponent<Dice3D>();
                    lastDice_2 = dice_10;
                    dice_10.ResetDice(true, pos_Dice_2);
                    break;
                case 20:
                    GameObject _dice_20 = Instantiate(m_Prefab_Dice_20);
                    _dice_20.transform.SetParent(_parent.transform, false);
                    dice_20 = _dice_20.GetComponent<Dice3D>();
                    lastDice_2 = dice_20;
                    dice_20.ResetDice(true, pos_Dice_2);
                    break;
                default:
                    break;
            }
        }

        if (nbr_Faces_Dice_3 != -1)
        {
            switch (nbr_Faces_Dice_3)
            {
                case 4:
                    GameObject _dice_4 = Instantiate(m_Prefab_Dice_4);
                    _dice_4.transform.SetParent(_parent.transform, false);
                    dice_4 = _dice_4.GetComponent<Dice3D>();
                    lastDice_3 = dice_4;
                    dice_4.ResetDice(true, pos_Dice_3);
                    break;
                case 6:
                    GameObject _dice_6 = Instantiate(m_Prefab_Dice_6);
                    _dice_6.transform.SetParent(_parent.transform, false);
                    dice_6 = _dice_6.GetComponent<Dice3D>();
                    lastDice_3 = dice_6;
                    dice_6.ResetDice(true, pos_Dice_3);
                    break;
                case 8:
                    GameObject _dice_8 = Instantiate(m_Prefab_Dice_8);
                    _dice_8.transform.SetParent(_parent.transform, false);
                    dice_8 = _dice_8.GetComponent<Dice3D>();
                    lastDice_3 = dice_8;
                    dice_8.ResetDice(true, pos_Dice_3);
                    break;
                case 10:
                    GameObject _dice_10 = Instantiate(m_Prefab_Dice_10);
                    _dice_10.transform.SetParent(_parent.transform, false);
                    dice_10 = _dice_10.GetComponent<Dice3D>();
                    lastDice_3 = dice_10;
                    dice_10.ResetDice(true, pos_Dice_3);
                    break;
                case 20:
                    GameObject _dice_20 = Instantiate(m_Prefab_Dice_20);
                    _dice_20.transform.SetParent(_parent.transform, false);
                    dice_20 = _dice_20.GetComponent<Dice3D>();
                    lastDice_3 = dice_20;
                    dice_20.ResetDice(true, pos_Dice_3);
                    break;
                default:
                    break;
            }
        }
        yield return new WaitUntil(() => _lastResult != -1);
        StartCoroutine(waitBeforeReturn());
        _callback?.Invoke(_lastResult);
    }
    public void ShowDiceUI(bool show)
    {
        m_Camera3D.SetActive(show);
        m_CanvasDice3D.SetActive(show);
        m_CanvasDice2D.SetActive(show);
        m_RollDiceButton.SetActive(show);
        m_ResultatTextGO.SetActive(false);
    }
    public void OnClick_RollDice()
    {
        m_RollDiceButton.SetActive(false);

        if(nbr_Dice == 1)
        {
            lastDice_1.RollDice();
        }
        else if (nbr_Dice == 2)
        {
            lastDice_1.RollDice();
            lastDice_2.RollDice();
        }
        else
        {
            lastDice_1.RollDice();
            lastDice_2.RollDice();
            lastDice_3.RollDice();
        }
    }
    IEnumerator waitBeforeReturn()
    {
        yield return new WaitForSeconds(1.8f);
        ShowDiceUI(false);
        //Supprimer tous les gameObject de dé
        
        if(lastDice_1 != null)
        {
            Destroy(lastDice_1.gameObject);
        }
        if (lastDice_2 != null)
        {
            Destroy(lastDice_2.gameObject);
        }
        if (lastDice_3 != null)
        {
            Destroy(lastDice_3.gameObject);
        }
        lastDice_1 = null;
        lastDice_2 = null;
        lastDice_3 = null;
    }
    public void DisplayResult()
    {
        if (nbr_Dice == 1){_lastResult = lastDice_1.GetResult();}
        else if (nbr_Dice == 2){_lastResult = lastDice_1.GetResult() + lastDice_2.GetResult();}
        else{_lastResult = lastDice_1.GetResult() + lastDice_2.GetResult() + lastDice_3.GetResult();}

        m_ResultatTexttxt.text =  gameManager._language.GetText("__dice_resultats") + _lastResult.ToString();
        m_ResultatTextGO.SetActive(true);
    }
    public int GetLastResult()
    {
        return _lastResult;
    }
    #endregion
}
