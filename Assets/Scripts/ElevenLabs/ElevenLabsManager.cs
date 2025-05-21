using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ElevenLabsManager : MonoBehaviour
{
    #region Variables
    public string _voiceId = ""; // TODO : Add your voice ID here

    public string _apiKey = ""; // TODO : Add your API key here

    public string _apiUrl = "https://api.elevenlabs.io";

    private AudioClip _audioClip;

    public bool Streaming;
    [Range(0, 4)]
    public int LatencyOptimization;

    public UnityEvent<AudioClip> AudioReceived;
    #endregion
    #region Methods
    public ElevenLabsManager(string apiKey, string voiceId)
    {
        _apiKey = apiKey;
        _voiceId = voiceId;
    }

    public void GetAudio(string text)
    {
        StartCoroutine(DoRequest(text));
    }

    private IEnumerator DoRequest(string message)
    {
        var postData = new TextToSpeechRequest
        {
            text = message,
            model_id = "eleven_multilingual_v2"
        };

        var voiceSetting = new VoiceSettings
        {
            stability = 0.5f,
            similarity_boost = 0.75f,
            style = 0.5f,
            use_speaker_boost = true
        };
        postData.voice_settings = voiceSetting;
        var json = JsonConvert.SerializeObject(postData);
        var uH = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        var stream = (Streaming) ? "/stream" : "";
        var url = $"{_apiUrl}/v1/text-to-speech/{_voiceId}{stream}?optimize_streaming_latency={LatencyOptimization}";
        var request = UnityWebRequest.PostWwwForm(url, json);
        var downloadHandler = new DownloadHandlerAudioClip(url, AudioType.MPEG);
        if (Streaming)
        {
            downloadHandler.streamAudio = true;
        }
        request.uploadHandler = uH;
        request.downloadHandler = downloadHandler;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", _apiKey);
        request.SetRequestHeader("Accept", "audio/mpeg");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }
        AudioClip audioClip = downloadHandler.audioClip;
        AudioReceived.Invoke(audioClip);
        request.Dispose();
    }

    /// <summary>
    /// Envoie une demande de voix à ElevenLabs et retourne un AudioClip
    /// </summary>
    /// <param name="txt_Voice"></param>
    /// <returns></returns>
    public async Task<AudioClip> Generate_Voice(string txt_Voice)
    {
        AudioClip audioClip = await RequestAudioFromElevenLabs(txt_Voice);
        return audioClip;
    }

    /// <summary>
    /// Demande l'audio à ElevenLabs et retourne le résultat une fois prêt
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private async Task<AudioClip> RequestAudioFromElevenLabs(string text)
    {
        TaskCompletionSource<AudioClip> tcs = new TaskCompletionSource<AudioClip>();

        // Écoute la réponse audio
        AudioReceived.AddListener((clip) =>
        {
            tcs.SetResult(clip); // Une fois l'audio reçu, on le retourne
        });

        // Envoie le texte à ElevenLabs
        GetAudio(text);

        // Attendre jusqu'à ce que l'audio soit prêt
        return await tcs.Task;
    }

    [Serializable]
    public class TextToSpeechRequest
    {
        public string text;
        public string model_id;
        public VoiceSettings voice_settings;
    }

    [Serializable]
    public class VoiceSettings
    {
        public float stability;
        public float similarity_boost;
        public float style;
        public bool use_speaker_boost;
    }
    #endregion
}
