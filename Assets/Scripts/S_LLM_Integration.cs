using UnityEngine;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


public class S_LLM_Integration 
{

    //OLD
    /*private string hfApiKeyMistral = "ln4lhcsWcBGbjy2hUiB8jkyA45Zr29hQ";  // Clé API pour Mistral
    private string hfApiKeyImage = "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";  // Clé API pour l'image (via HuggingFace)
    private string agentId_ALLIntrigue_Mistral = "ag:65bc52ef:20241001:untitled-agent:55bf1524";  // ID de l'agent Mistral
    private string hfModelUrlMistral = "https://api.mistral.ai/v1/agents/completions"; // Lien pour l'agent Mistral
    private string hfModelUrlImage = "https://api-inference.huggingface.co/models/black-forest-labs/FLUX.1-dev"; // Lien vers BlackForest



    #region Public Method
    public string GetAPIKey(string nameLLM)
    {
        switch (nameLLM)
        {
            case "Mistral": return hfApiKeyMistral;
            default: return hfApiKeyMistral;


        }
    }
    public string GetAgentID(string nameAgent)
    {
        switch (nameAgent)
        {
            case "All_Intrigue": return agentId_ALLIntrigue_Mistral;
            default: return agentId_ALLIntrigue_Mistral;

        }
    }
    public string GetURL(string nameLLM)
    {
        switch (nameLLM)
        {
            case "Mistral": return hfModelUrlMistral;
            case "BlackForest": return hfModelUrlImage;
            default: return hfModelUrlMistral;

        }
    }
    #endregion*/

}
