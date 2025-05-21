using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class AzureUploader : MonoBehaviour
{
    private string databaseURL = ""; // TODO : Add your database URL here

    public void UploadJSON(string fileName, string jsonData)
    {
        StartCoroutine(SendJSONToAzure(fileName, jsonData));
    }

    private IEnumerator SendJSONToAzure(string fileName, string jsonData)
    {
        string jsonToSend = JsonUtility.ToJson(new JsonData { FileName = fileName, JsonContent = jsonData });
        databaseURL = "";// TODO : Add your database URL here
        using (UnityWebRequest request = new UnityWebRequest(databaseURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonToSend);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
    }

    public void UploadPNG(string fileName, byte[] pngData)
    {
        StartCoroutine(SendPNGToAzure(fileName, pngData));
    }

    private IEnumerator SendPNGToAzure(string fileName, byte[] pngData)
    {
        if (pngData == null || pngData.Length == 0)
        {
            yield break;
        }
        databaseURL = ""; // TODO : Add your database URL here
        using (UnityWebRequest request = new UnityWebRequest(databaseURL, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(pngData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            request.SetRequestHeader("File-Name", fileName);
            yield return request.SendWebRequest();
        }
    }


    [System.Serializable]
    private class JsonData
    {
        public string FileName;
        public string JsonContent;
    }
}