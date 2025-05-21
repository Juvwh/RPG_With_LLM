using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GroupOfOne : Group
{
    public void Initialize(PlayerData player)
    {
        base.Initialize(new PlayerData[] { player }); // Appelle le constructeur de la classe Group (cvlasse mère)
    }
    #region Redefine des méthodes
    public override string GetAllNames()
    {
        return $"{m_Players[0].playerName}";
    }
    public override string GetResume(bool forCoherence = false)
    {
        var values = new Dictionary<string, string>
        {
            {"personnage", m_Players[0].playerName},
            {"description",m_Players[0].playerName + " : " + m_Players[0].GetResume()},
            {"santé", $"{m_Players[0].playerName} : {m_Players[0].playerHealth}" },
            {"inventaire", GetAllInventaireString() },
            {"évènements_héros", GetAllHistoryResumeInString()},
            {"carte", GetGroupNeighbors(forCoherence)}
        };

        return  _gameManager._language.GetPrompt("__get_resume_group_of_1", values);
    }
    public override string GetResume_Names()
    {
        return $"{m_Players[0].playerName} ({m_Players[0].playerClass}, {m_Players[0].playerRace}, {m_Players[0].playerHealth}, {m_Players[0].playerResume}  )";
    }
    #endregion
}
