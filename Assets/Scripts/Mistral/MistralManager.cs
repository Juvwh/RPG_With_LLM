using UnityEngine;
using System.IO;
using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

// Ce script est le MistralManager, il permet de g�rer les appels � l'API de Mistral pour g�n�rer du texte.
// Il permet �galement de g�rer les diff�rents agents de Mistral.

#region Structure JSON
[System.Serializable]
public class LLMResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string content;
}
#endregion