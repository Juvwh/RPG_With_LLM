using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evenement : MonoBehaviour {

    public virtual void Continuer(GameManager _gameManager, int groupIndex)
    {
        _gameManager._UI_Manager.m_InputText.text = "";
        _gameManager._UI_Manager.Afficher_InputNonCoherent(false);
        _gameManager._UI_Manager.SetCheckCoherenceInput(false);
        _gameManager._groupsManager.m_Groups[groupIndex].SetReady(false);
        _gameManager._UI_Manager.SwitchGroupCanvas(groupIndex);

        //Lancer la prochaine phase
        //Récuperer l'id du groupe pour comparer plus tard

    }


    public static bool CheckSameGroup(GameManager _gameManager, int groupIndex, string previousID)
    {
        if (_gameManager._groupsManager.m_GroupsCount > groupIndex)
        {
            if(_gameManager._groupsManager.m_Groups[groupIndex]._groupID == previousID)
            {
                return true;
            }
        }
        return false;
    }

    public static int CheckChangeIndex(GameManager _gameManager, int groupIndex, string previousID)
    {
        foreach (Group group in _gameManager._groupsManager.m_Groups)
        {
            if (group._groupID == previousID)
            {
                return group.m_GroupIndex;
            }
        }
        return -1;
    }
}
