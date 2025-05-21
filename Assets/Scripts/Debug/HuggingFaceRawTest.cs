using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class HuggingFaceRawTest : MonoBehaviour
{
    [TextArea]
    public string prompt = "Astronaut riding a horse on Mars";

    public string huggingFaceToken = "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";  // Mets ta vraie cl� ici
    private string modelUrl = "https://api-inference.huggingface.co/models/black-forest-labs/FLUX.1-schnell";

    void Start()
    {
        StartCoroutine(SendRawTestRequest());
    }

    IEnumerator SendRawTestRequest()
    {
        // Construire la requ�te JSON
        string json = "{\"inputs\":\"" + prompt + "\",\"parameters\":{\"width\":512,\"height\":512}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(modelUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + huggingFaceToken);
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log(" Envoi de la requ�te � Hugging Face...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(" R�ponse re�ue !");
                Debug.Log("Type de contenu : " + request.GetResponseHeader("Content-Type"));

                // Affiche une partie du contenu
                Debug.Log("Contenu brut (d�but) : " + Encoding.UTF8.GetString(request.downloadHandler.data, 0, Mathf.Min(500, request.downloadHandler.data.Length)));
            }
            else
            {
                Debug.LogError("Erreur : " + request.error);
                Debug.LogError("R�ponse brute : " + request.downloadHandler.text);
            }
        }
    }
}
