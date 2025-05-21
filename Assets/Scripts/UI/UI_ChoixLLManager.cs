using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ChoixLLManager : MonoBehaviour
{
    public UI_ChoixLLM[] _allLLM;
    public void _changeSelected(UI_ChoixLLM _selected)
    {
        foreach (UI_ChoixLLM item in _allLLM)
        {
            item.oneIsSelected = true;
            item.OnCanvasGroupChanged_(_selected == item);
        }
    }
}
