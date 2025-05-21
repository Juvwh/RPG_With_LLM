using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Models;
using static Jobs;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Diagnostics;

public class AgentManager : MonoBehaviour
{
    #region Managers
    [Header("Managers")]
    public GameManager _gm;
    LocalizationManager _language;
    #endregion

    #region Debug
    [Header("Debug")]
    private string _fileNameLog;
    #endregion

    #region Groq API
    [Header("Groq API")]
    private static readonly string apiKeyGroq = ""; // TODO:  Enter your Groq API key here
    private static readonly string modelUrlGroq = "https://api.groq.com/openai/v1/chat/completions";
    #endregion

    #region OpenAI
    [Header("OpenAI")]
    private static readonly string apiKeyOpenAI = ""; // TODO:  Enter your OpenAI API key here
    private static readonly string modelUrlOpenAI = "https://api.openai.com/v1/chat/completions";
    #endregion

    #region Mistral Agent ID
    [Header("Agent Mistral ID")]
    private static string apiKeyMistral = "";  // TODO:  Enter your Mistral API key here
    private static string modelUrlMistral = "https://api.mistral.ai/v1/agents/completions"; // Link for Mistral agent
    #endregion

    #region Tasks and Jobs
    [Header("Task Texts")]
    // These texts are the instructions passed to the LLMs to generate the texts.
    private static string _job_text_Intrigue = ""; // Modified at start -> see JSON file
    private static string _job_text_Starter_Room1 = ""; // Modified at start -> see JSON file
    private static string _job_text_Starter_Room2 = ""; // Modified at start -> see JSON file
    private static string _job_text_Starter_Room3 = ""; // Modified at start -> see JSON file
    private static string _job_text_Generate_Room_Summary = "";
    private static string _job_text_LLM_Validation_Input = "";
    private static string _job_text_LLM_Validation_Theme = "";
    private static string _job_text_LLM_Validation_Combat = "";
    private static string _job_text_Combat_Starter = "";
    private static string _job_text_Combat_Boss = "";
    private static string _job_text_Combat_Boss_Starter = "";
    private static string _job_text_Combat_Sequence = "";
    private static string _job_Text_Combat_Narration_Attack = "";
    private static string _job_text_Combat_Narration_Health = "";
    private static string _job_text_Combat_Enemy_Sequence = "";
    private static string _job_text_Sequence = "";
    private static string _job_text_Generate_Hero = "";
    private static string _job_text_Generate_Image_Hero = "";
    private static string _job_text_Generate_Map = "";
    private static string _job_text_Inventory = "";
    private static string _job_text_Tiles_Description = "";
    private static string _job_text_Ending = "";
    private static string _job_text_Begging = "";
    private static string _job_text_Bad_Ending = "";
    private static string _job_text_Good_Ending = "";
    private static string _job_text_Generate_Prompt_Image_GoodEnding = "";
    private static string _job_text_Generate_Prompt_Image_BadEnding = "";
    private static string _job_text_Character_Translation = "";
    #endregion

    private void Start()
    {
        _gm = FindFirstObjectByType<GameManager>();
        if (_gm != null)
        {
            _gm._agent = this;
        }
        _fileNameLog = "_logsPartie_" + System.DateTime.Now.ToString("dd_MM_yyyy___HHmm");
        _fileNameLog += ".csv";
        _language = FindFirstObjectByType<LocalizationManager>();
        UpdateLanguage(_language.currentLanguage);
    }

    /// <summary>
    /// Generates text using the specified model and job.
    /// </summary>
    /// <param name="model">The model to use for text generation.</param>
    /// <param name="job">The job to perform.</param>
    /// <param name="prompt">The prompt to send to the model.</param>
    /// <param name="_callback">Callback function to handle the generated text.</param>
    /// <returns>Coroutine for generating the text.</returns>
    public IEnumerator Generate_Text(Model model, Job job, string prompt, Action<string> _callback)
    {
        // Check if the application is running to avoid sending requests after the application is closed
        if (!Application.isPlaying)
        {
            _callback?.Invoke(null);
        }
        // Get the model name and job text
        string _modelName = GetModelName(model);
        string _job = GetJobText(job);
        string toReturn = "";

        // Keep a measurement file and measure the response time
        string filePath = Path.Combine(Application.streamingAssetsPath, "TimeRecord.csv");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Send the prompt to the API, depending on Groq or Mistral
        if (model == Model.mistral_3_3_70b)
        {
            string agentID_Mistral = ""; // TODO: Enter your Mistral agent ID here
            prompt = Parser.CLEAN(prompt);

            System.Random random = new System.Random();
            int randomSeed = random.Next(1, int.MaxValue);

            var requestBody = new
            {
                agent_id = agentID_Mistral,
                messages = new[]
                {
                    new { role = "system", content = _job },
                    new { role = "user", content = prompt }
                },
                random_seed = randomSeed
            };

            string jsonData = JsonConvert.SerializeObject(requestBody);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = new UnityWebRequest(modelUrlMistral, "POST"))
            {
                request.SetRequestHeader("Authorization", "Bearer " + apiKeyMistral);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();
                stopwatch.Stop();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseString = request.downloadHandler.text;
                    LLMResponse parsedResponse = JsonConvert.DeserializeObject<LLMResponse>(responseString);
                    string responseText = parsedResponse.choices[0].message.content;
                    toReturn = responseText;
                    if (_gm != null)
                    {
                        Record_Log_CSV(_gm, prompt, toReturn, model, job, stopwatch.ElapsedMilliseconds, _fileNameLog, _language);
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Error calling Mistral: " + request.error);
                    toReturn = "Error: " + request.error;
                }
            }
        }
        else if (model == Model.openAi)
        {
            string modelNameOpenAI = "gpt-4o-mini"; // or "gpt-4-turbo" depending on what you want to use

            var requestBody = new
            {
                model = modelNameOpenAI,
                messages = new[]
                {
                    new { role = "system", content = _job },
                    new { role = "user", content = prompt }
                },
                temperature = 0.5,
                max_tokens = 1024,
                top_p = 1,
                stream = false
            };

            string jsonData = JsonConvert.SerializeObject(requestBody);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest request = new UnityWebRequest(modelUrlOpenAI, "POST"))
            {
                request.SetRequestHeader("Authorization", "Bearer " + apiKeyOpenAI);
                request.SetRequestHeader("Content-Type", "application/json");
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                yield return request.SendWebRequest();
                stopwatch.Stop();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseString = request.downloadHandler.text;
                    LLMResponse parsedResponse = JsonConvert.DeserializeObject<LLMResponse>(responseString);
                    string responseText = parsedResponse.choices[0].message.content;
                    toReturn = responseText;

                    if (_gm != null)
                    {
                        Record_Log_CSV(_gm, prompt, toReturn, model, job, stopwatch.ElapsedMilliseconds, _fileNameLog, _language);
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Error calling OpenAI: " + request.error);
                    toReturn = "Error: " + request.error;
                }
            }
        }
        else
        {
            using (UnityWebRequest request = new UnityWebRequest(modelUrlGroq, "POST"))
            {
                request.SetRequestHeader("Authorization", "Bearer " + apiKeyGroq);

                var requestBody = new
                {
                    model = _modelName,
                    messages = new[]
                    {
                        new { role = "system", content = _job },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.5,
                    max_completion_tokens = 1024,
                    top_p = 1,
                    stop = (string)null,
                    stream = false
                };

                string jsonData = JsonConvert.SerializeObject(requestBody);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();
                stopwatch.Stop();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseString = request.downloadHandler.text;
                    LLMResponse parsedResponse = JsonConvert.DeserializeObject<LLMResponse>(responseString);
                    string responseText = parsedResponse.choices[0].message.content;
                    toReturn = responseText;
                    if (_gm != null)
                    {
                        Record_Log_CSV(_gm, prompt, toReturn, model, job, stopwatch.ElapsedMilliseconds, _fileNameLog, _language);
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("Error calling Groq: " + request.error);
                    toReturn = "Error: " + request.error;
                }
            }
        }

        // Stop the stopwatch and record the response time
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string record = $"{model},{job},{prompt.Length},{stopwatch.ElapsedMilliseconds},{timestamp}\n";

        // Verify the response for error handling (optional)
        VerifyResponse(toReturn, job, model);
        _callback?.Invoke(toReturn);
    }

    /// <summary>
    /// Verifies the response for errors.
    /// </summary>
    /// <param name="response">The response to verify.</param>
    /// <param name="job">The job associated with the response.</param>
    /// <param name="model">The model used for the response.</param>
    private static void VerifyResponse(string response, Job job, Model model)
    {
        if (response.StartsWith("Erreur") || response.StartsWith("TlsException") || response.StartsWith("TaskCanceledException") || response.StartsWith("Too Many") || response == "")
        {
            if (response == "")
            {
                response = "The LLM response was empty";
            }
            UnityEngine.Debug.LogError($"Model: {model}, Job: {job}, Error:\n{response}");
        }
    }

    /// <summary>
    /// Gets the model name based on the model type.
    /// </summary>
    /// <param name="modelType">The model type.</param>
    /// <returns>The model name.</returns>
    private static string GetModelName(Model modelType)
    {
        switch (modelType)
        {
            case Model.llama_3_3_70b_versatile:
                return "llama-3.3-70b-versatile";
            case Model.gemma2_9b_it:
                return "gemma2-9b-it";
            case Model.mistral_3_3_70b:
                return "mistral-3.3-70b";
            case Model.deepseek_r1_distill_llama_70b:
                return "deepseek-r1-distill-llama-70b";
            case Model.openAi:
                return "gpt-4o-mini";
            default:
                return "Error in choosing the LLM for the text.";
        }
    }

    /// <summary>
    /// Gets the job text based on the job.
    /// </summary>
    /// <param name="job">The job.</param>
    /// <returns>The job text.</returns>
    private static string GetJobText(Job job)
    {
        switch (job)
        {
            case Job.Intrigue:
                return _job_text_Intrigue;
            case Job.Starter_Room1:
                return _job_text_Starter_Room1;
            case Job.Starter_Room2:
                return _job_text_Starter_Room2;
            case Job.Starter_Room3:
                return _job_text_Starter_Room3;
            case Job.CheckCoherenceInput:
                return _job_text_LLM_Validation_Input;
            case Job.Salle1_Generer_Free:
                return _job_text_Sequence;
            case Job.GenererHero:
                return _job_text_Generate_Hero;
            case Job.GenererDescriptionImageHero:
                return _job_text_Generate_Image_Hero;
            case Job.GenererFormeMap:
                return _job_text_Generate_Map;
            case Job.CheckCoherenceTheme:
                return _job_text_LLM_Validation_Theme;
            case Job.Combat_narration:
                return _job_Text_Combat_Narration_Attack;
            case Job.combat_start:
                return _job_text_Combat_Starter;
            case Job.Inventaire:
                return _job_text_Inventory;
            case Job.Case_Description:
                return _job_text_Tiles_Description;
            case Job.GenererResumeRoom:
                return _job_text_Generate_Room_Summary;
            case Job.FreeCombat:
                return _job_text_Combat_Sequence;
            case Job.ChechCoherenceInputCombat:
                return _job_text_LLM_Validation_Combat;
            case Job.Combat_Narration_Soin:
                return _job_text_Combat_Narration_Health;
            case Job.Combat_Tour_Ennemi:
                return _job_text_Combat_Enemy_Sequence;
            case Job.Fin:
                return _job_text_Ending;
            case Job.Supplication:
                return _job_text_Begging;
            case Job.FreeBoss:
                return _job_text_Combat_Boss;
            case Job.CombatStartBoss:
                return _job_text_Combat_Boss_Starter;
            case Job.GoodEnd:
                return _job_text_Good_Ending;
            case Job.BadEnd:
                return _job_text_Bad_Ending;
            case Job.PromptImageBonneFin:
                return _job_text_Generate_Prompt_Image_GoodEnding;
            case Job.PromptImageMauvaiseFin:
                return _job_text_Generate_Prompt_Image_BadEnding;
            case Job.Traducteur:
                return _job_text_Character_Translation;
            default:
                return "Error in choosing the task for the text.";
        }
    }

    /// <summary>
    /// Records the log in CSV format.
    /// </summary>
    /// <param name="_gm">The game manager.</param>
    /// <param name="prompt">The prompt sent to the model.</param>
    /// <param name="response">The response from the model.</param>
    /// <param name="model">The model used.</param>
    /// <param name="job">The job performed.</param>
    /// <param name="responseTime">The response time in milliseconds.</param>
    /// <param name="fileName">The file name for the log.</param>
    /// <param name="_language">The language manager.</param>
    private static void Record_Log_CSV(GameManager _gm, string prompt, string response, Model model, Job job, long responseTime, string fileName, LocalizationManager _language)
    {
        string debugFilePath = Path.Combine(Application.streamingAssetsPath, "Prompts_Responses_LastSession.csv");
        string allFilePath = Path.Combine(Application.streamingAssetsPath, "Prompts_Responses_AllTimes.csv");

        int promptTokens = CountTokens(prompt);
        int responseTokens = CountTokens(response);
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Use `;` as a separator for Excel in French
        string formattedLine = $"\"{prompt.Replace("\"", "\"\"")}\";\"{response.Replace("\"", "\"\"")}\";\"{model}\";\"{job}\";{responseTime};\"{timestamp}\";{promptTokens};{responseTokens};{_language.currentLanguage}\n";

        // Save in UTF-8 with BOM
        if (_gm.m_GenerateLogOnAzur)
        {
            //LogManager.SendLog(fileName, $"\"{prompt.Replace("\"", "\"\"")}\";\"{response.Replace("\"", "\"\"")}\";\"{model}\";\"{job}\";{responseTime};\"{timestamp}\";{promptTokens};{responseTokens};{_language.currentLanguage}\n");
            //LogManagerAllTime.SendLog($"\"{prompt.Replace("\"", "\"\"")}\";\"{response.Replace("\"", "\"\"")}\";\"{model}\";\"{job}\";{responseTime};\"{timestamp}\";{promptTokens};{responseTokens};{_language.currentLanguage}\n");
        }
        else
        {
            AppendTextToFile(debugFilePath, formattedLine);
            AppendTextToFile(allFilePath, formattedLine);
        }
    }

    /// <summary>
    /// Appends text to a file.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="content">The content to append.</param>
    private static void AppendTextToFile(string filePath, string content)
    {
        // Check if the file exists
        using (StreamWriter writer = new StreamWriter(filePath, true, new UTF8Encoding(true)))
        {
            writer.WriteLine(content);
        }
    }

    /// <summary>
    /// Simple method to count tokens (approximation with words).
    /// </summary>
    /// <param name="text">The text to count tokens in.</param>
    /// <returns>The number of tokens.</returns>
    private static int CountTokens(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        return text.Split(new char[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    /// <summary>
    /// Changes the LLM text automatically.
    /// </summary>
    public void ChangeLLMTextAuto()
    {
        if (_gm.m_currentModel == Model.llama_3_3_70b_versatile)
        {
            _gm.m_currentModel = Model.gemma2_9b_it;
            ChangeUIOptionLLM();
        }
        else
        {
            _gm.m_currentModel = Model.llama_3_3_70b_versatile;
            ChangeUIOptionLLM();
        }
    }

    /// <summary>
    /// Changes the UI option for LLM.
    /// </summary>
    public void ChangeUIOptionLLM()
    {
        if (_gm._UI_Manager == null) { return; }
        switch (_gm.m_currentModel)
        {
            case Model.llama_3_3_70b_versatile:
                _gm._UI_Manager.m_DropdownTextLLM.value = 0;
                break;
            case Model.gemma2_9b_it:
                _gm._UI_Manager.m_DropdownTextLLM.value = 1;
                break;
            case Model.mistral_3_3_70b:
                _gm._UI_Manager.m_DropdownTextLLM.value = 2;
                break;
            case Model.openAi:
                _gm._UI_Manager.m_DropdownTextLLM.value = 3;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Updates the language for job texts.
    /// </summary>
    /// <param name="language">The language to update to.</param>
    public void UpdateLanguage(string language)
    {
        _job_text_Intrigue = _language.GetText("_job_text_Intrigue");
        _job_text_Starter_Room1 = _language.GetText("_job_text_Starter_Room1");
        _job_text_Starter_Room2 = _language.GetText("_job_text_Starter_Room2");
        _job_text_Starter_Room3 = _language.GetText("_job_text_Starter_Room3");
        _job_text_LLM_Validation_Input = _language.GetText("_job_text_CheckCoherenceInput");
        _job_text_Sequence = _language.GetText("_job_text_Free");
        _job_text_Generate_Hero = _language.GetText("_job_text_GenererHero");
        _job_text_Generate_Image_Hero = _language.GetText("_job_text_GenererImageHero");
        _job_text_Generate_Map = _language.GetText("_job_text_GenererFormeMap");
        _job_text_LLM_Validation_Theme = _language.GetText("_job_text_CheckCoherenceTheme");
        _job_text_Combat_Starter = _language.GetText("_job_combat_start");
        _job_Text_Combat_Narration_Attack = _language.GetText("_job_combat_narration");
        _job_text_Inventory = _language.GetText("_job_text_Inventaire");
        _job_text_Tiles_Description = _language.GetText("_job_text_CaseDescritpion");
        _job_text_Generate_Room_Summary = _language.GetText("_job_text_GenererResumeRoom");
        _job_text_Combat_Sequence = _language.GetText("_job_text_FreeCombat");
        _job_text_LLM_Validation_Combat = _language.GetText("_job_text_CheckCoherenceInputCombat");
        _job_text_Combat_Narration_Health = _language.GetText("_job_text_Combat_Narration_Soin");
        _job_text_Combat_Enemy_Sequence = _language.GetText("_job_text_Combat_Tour_Ennemi");
        _job_text_Ending = _language.GetText("_job_text_Fin");
        _job_text_Begging = _language.GetText("_job_text_Supplication");
        _job_text_Combat_Boss = _language.GetText("_job_text_CombatBoss");
        _job_text_Combat_Boss_Starter = _language.GetText("_job_text_CombatStartBoss");
        _job_text_Bad_Ending = _language.GetText("_job_text_BadEnd");
        _job_text_Good_Ending = _language.GetText("_job_text_GoodEnd");
        _job_text_Generate_Prompt_Image_GoodEnding = _language.GetText("_job_text_GenererPromptImageBonneFin");
        _job_text_Generate_Prompt_Image_BadEnding = _language.GetText("_job_text_GenererPromptImageMauvaiseFin");
        _job_text_Character_Translation = _language.GetText("_job_text_traductionPersonnage");
    }
}

#region JSON Structure
[System.Serializable]
public class ParsedResponse
{
    public List<Choice> Choices { get; set; }
}
#endregion
