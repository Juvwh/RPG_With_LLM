using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.Networking;

public class HuggingFaceImageManager
{
    private string hfApiKeyHugginfFace = "";  // TODO : Add your Hugging Face API key here
    private const string togetherEndpoint = "https://api.endpoints.huggingface.cloud/inference";
    private string hfModelUrlStableDiffusion = "https://api-inference.huggingface.co/models/stable-diffusion-v1-5/stable-diffusion-v1-5"; // Lien vers StableDiffusion
    private string hfModelUrlBlackForest = "https://api-inference.huggingface.co/models/black-forest-labs/FLUX.1-dev"; // Lien vers BlackForest
    private string hfModelUrlBlackForestFast = "https://api-inference.huggingface.co/models/black-forest-labs/FLUX.1-schnell"; // Lien vers BlackForest schnell
    [System.Serializable]
    public class HuggingFaceRequest
    {
        public string inputs;
        public Parameters parameters;
    }
    [System.Serializable]
    public class Parameters
    {
        public int width;
        public int height;
    }
    public IEnumerator SendPrompt(string prompt, GameManager.LLM_image llm, Action<Texture2D> _callback)
    {

        string _url = GetUrlFromModel(llm);
        Debug.Log("URL : " + _url);
        // Créer l'objet de requête avec des paramètres valides
        HuggingFaceRequest requestData = new HuggingFaceRequest
        {
            inputs = prompt,
            parameters = new Parameters
            {
                width = 512,   
                height = 512
            }
        };


        // Convertir en JSON proprement
        string jsonData = JsonUtility.ToJson(requestData);

        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(_url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + hfApiKeyHugginfFace);
            Debug.Log("SENDING");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur lors de l'appel à l'API de génération d'image : " + request.error);
                Debug.LogError("Réponse complète : " + request.downloadHandler.text);  
                _callback?.Invoke(null);
            }
            else
            {
                byte[] imageBytes = request.downloadHandler.data;
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                Debug.LogError("Réponse complète : " + request.downloadHandler.text);
                _callback?.Invoke(texture);
            }
        }
    }
    private string GetUrlFromModel(GameManager.LLM_image llm)
    {
        switch (llm)
        {
            case GameManager.LLM_image.BlackForest: return hfModelUrlBlackForest;
            case GameManager.LLM_image.StableDiffusion: return hfModelUrlBlackForest;
            case GameManager.LLM_image.BlackForestFast: return hfModelUrlBlackForestFast;
            default: return hfModelUrlBlackForest;
        }
    }
}
