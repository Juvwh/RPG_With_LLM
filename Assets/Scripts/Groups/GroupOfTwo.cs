using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupOfTwo : Group
{
    public void Initialize(PlayerData[] player)

    {
        base.Initialize(player); // Appelle le constructeur de la classe Group (cvlasse mère)
    }
    #region Redefine des méthodes
    public override string GetAllNames()
    {
        return $"{m_Players[0].playerName} et {m_Players[1].playerName}";
    }
    public override string GetResume(bool forCoherence = false)
    {
        var values = new Dictionary<string, string>
        {
            {"personnages", m_Players[0].playerName + "," + m_Players[1].playerName},
            {"description", $"{m_Players[0].playerName} : {m_Players[0].GetResume()} ; {m_Players[1].playerName} : {m_Players[1].GetResume()}"},
            {"santé", $"{m_Players[0].playerName} : {m_Players[0].playerHealth} ; {m_Players[1].playerName} : {m_Players[1].playerHealth}" },
            {"inventaire", GetAllInventaireString() },
            {"événements", GetAllHistoryResumeInString()},
            {"carte", GetGroupNeighbors(forCoherence)}
        };

        return _gameManager._language.GetPrompt("__get_resume_group_of_2", values);
    }
    public override string GetResume_Names()
    {
        return $"{m_Players[0].playerName} ({m_Players[0].playerClass}, {m_Players[0].playerRace}, {m_Players[0].playerHealth}, {m_Players[0].playerResume}  );" +
                $"{m_Players[1].playerName} ({m_Players[1].playerClass}, {m_Players[1].playerRace}, {m_Players[1].playerHealth}, {m_Players[1].playerResume} )";
    }
    #endregion
}
