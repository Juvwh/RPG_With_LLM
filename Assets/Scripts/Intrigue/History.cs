using System;

[Serializable]
public struct HistoryResume
{
    #region Variables
    private static int currentID = 0; // Variable statique pour suivre le dernier ID attribué

    public string Resume { get; private set; }
    public int ID { get; private set; }
    public bool isChangingGroup { get; private set; }
    public string previousGroupId { get; private set; }
    public string newGroupId { get; private set; }
    #endregion
    #region Methods
    public HistoryResume(string resume, bool _isChangingGroup = false, string _previousGroupId = null, string _newGroupId = null, int id = -1)
    {
        Resume = resume;
        if(id != -1)
        {
            ID = id;
        }
        else
        {
            ID = currentID; // Assigner l'ID actuel
            currentID++;    // Incrémenter pour le prochain
        }
        isChangingGroup = _isChangingGroup; // Assigner la valeur de changement de groupe
        previousGroupId = _previousGroupId; // Assigner l'ID du groupe précédent
        newGroupId = _newGroupId; // Assigner l'ID du nouveau groupe
    }
    public string GetResume()
    {
        return Resume;
    }
    public int GetID()
    {
        return ID;
    }
    public void SetResume(string resume)
    {
        Resume = resume;
    }
    public void SetID(int id)
    {
        ID = id;
    }
    public override string ToString()
    {
        return $"ID: {ID}, Resume: {Resume}";
    }
    #endregion
}
[Serializable]
public struct History
{
    private static int currentID = 0; // Variable statique pour suivre le dernier ID attribué

    public string text { get; private set; }
    public int ID { get; private set; }
    public bool isChangingGroup { get; private set; }
    public string previousGroupId { get; private set; }
    public string newGroupId { get; private set; }

    public History(string txt, bool _isChangingGroup = false, string _previousGroupId = null, string _newGroupId = null, int id = -1)
    {
        text = txt;
        if(id != -1)
        {
            ID = id;
        }
        else
        {
            ID = currentID; // Assigner l'ID actuel
            currentID++;    // Incrémenter pour le prochain
        }
        isChangingGroup = _isChangingGroup; // Assigner la valeur de changement de groupe
        previousGroupId = _previousGroupId; // Assigner l'ID du groupe précédent
        newGroupId = _newGroupId; // Assigner l'ID du nouveau groupe
    }

    public string GetText()
    {
        return text;
    }
    public int GetID()
    {
        return ID;
    }
    public void SetText(string txt)
    {
        text = txt;
    }
    public void SetID(int id)
    {
        ID = id;
    }

    public override string ToString()
    {
        return $"ID: {ID}, Text: {text}";
    }
}
