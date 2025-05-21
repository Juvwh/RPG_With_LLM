using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key;
    void OnEnable()
    {
        UpdateText();
    }
    void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        GetComponent<TMP_Text>().text = LocalizationManager.Instance.GetText(key);
    }
}
