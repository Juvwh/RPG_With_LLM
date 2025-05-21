using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Models;

public class ButtonsManager : MonoBehaviour
{
    private GameManager _gm;
    private void Start()
    {
        _gm = FindObjectOfType<GameManager>();
    }
    public void OnClick_Generate_Intrigue_Button()
    {
        _gm.FindCharacterManager();
        _gm._historyManager.Generate_Campagn();
    }
    public void OnClick_Continue_Button()
    {
        StartCoroutine(ContinueCoroutine());
    }
    public IEnumerator ContinueCoroutine()
    {
        yield return StartCoroutine(_gm._mapManager.GenerateMap(_gm._memoryManager.GetMemoryRoom(0), 0));
        yield return StartCoroutine(_gm._historyManager.StartGame());
    }
    public void On_Click_Ending_Button()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            Destroy(obj,1f);
        }
        SceneManager.LoadScene("StartMenu");
    }
    public void OnClick_Submit_Button()
    {
        if(_gm._UI_Manager.m_ButtonSendPrompt_Exploration.GetComponent<Image>().color == new Color32(46, 23, 0, 255))
        {
            return;
        }
        StartCoroutine(_gm._free.LLM_Validation_Input_Player());
    }
    public void OnClick_Canvas_Character(int characterNumber)
    {
        if (_gm._groupsManager.m_Groups[characterNumber].isDead) { return; }
        _gm._UI_Manager.SwitchGroupCanvas(characterNumber);
    }
    public void OnClick_Confirm_Item_Use_Button()
    {
        _gm._UI_Manager.Afficher_ConfirmUseItem(false);
    }
    public void TriggerEnter_ChangeColor(Image image)
    {
        if(image.color == new Color(0.29f, 0.29f, 0.29f, 1f))
        {
            if(image.color.a != 1f)
            {
                return;
            }
            image.color = new Color(0.69f, 0.69f, 0.69f, 1);
        }
    }
    public void TriggerExit_ChangeColor(Image image)
    {
        if (image.color == new Color(0.69f, 0.69f, 0.69f, 1f))
        {
            image.color = new Color(0.29f, 0.29f, 0.29f, 1f);
        }
        else if (image.color != new Color(0.3f, 0.3f, 0.3f, 0.9f))
        {
            image.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    public void OnClick_Change_LLM(int selectedModel)
    {
        if(_gm == null)
        {
            return;
        }
        switch (selectedModel)
        {
            case 0:
                _gm.m_currentModel = Model.llama_3_3_70b_versatile;
                break;
            case 1:
                _gm.m_currentModel = Model.gemma2_9b_it;
                break;
            case 2:
                _gm.m_currentModel = Model.mistral_3_3_70b;
                break;
            case 3:
                _gm.m_currentModel = Model.openAi;
                break;
            default:
                break;
        }
    }
    public void OnClick_ReGenerate_Button()
    {
        StartCoroutine(_gm._historyManager.RegenerateCampagn());
    }
    public void OnClick_QuitButton()
    {
        _gm._UI_Manager.m_Canvas_Formulaire.SetActive(true);
    }
    public void OnClick_CloseForm_Button()
    {
        _gm._UI_Manager.m_Canvas_Formulaire.SetActive(false);
        _gm._UI_Manager.Show_Loading_Screen();
        GameObject dontDestroy = _gm._charactersManagers.gameObject;
        DestroyImmediate(dontDestroy);
        SceneManager.LoadScene("StartMenu");
    }
}
