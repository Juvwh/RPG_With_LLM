using UnityEngine;

[CreateAssetMenu(fileName = "NewInfluence", menuName = "Mesures/Influence")]
public class ScenarioInfluence : ScriptableObject
{
    [Header("CONTEXT")]
    [TextArea(10, 100)]
    public string m_prompt = "";

    [Header("Flatterie")]
    [TextArea(3, 10)]
    public string m_Flatterie = "";
    [Header("Supplication")]
    [TextArea(3, 10)]
    public string m_Supplication = "";
    [Header("Autorite")]
    [TextArea(3, 10)]
    public string m_Autorite = "";
    [Header("Relativisation")]
    [TextArea(3, 10)]
    public string m_Relativisation = "";
    [Header("Normalisation")]
    [TextArea(3, 10)]
    public string m_Normalisation = "";
    [Header("Reformulation")]
    [TextArea(3, 10)]
    public string m_Reformulation = "";
    [Header("Contextualisation")]
    [TextArea(3, 10)]
    public string m_Contextualisation = "";
    [Header("Humour")]
    [TextArea(3, 10)]
    public string m_Humour = "";
    [Header("Triche")]
    [TextArea(3, 10)]
    public string m_Triche = "";
    [Header("Pression")]
    [TextArea(3, 10)]
    public string m_Pression = "";
    [Header("Menssonge")]
    [TextArea(3, 10)]
    public string m_Menssonge = "";
    [Header("Aucune")]
    [TextArea(3, 10)]
    public string m_aucune = "";

}
