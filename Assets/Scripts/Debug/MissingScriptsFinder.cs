using UnityEditor;
using UnityEngine;

public class MissingScriptsFinder
{
    //[MenuItem("Tools/Find Missing Scripts")]
    //public static void FindMissingScripts()
    //{
    //    GameObject[] go = GameObject.FindObjectsOfType<GameObject>();
    //    int missingCount = 0;

    //    foreach (GameObject g in go)
    //    {
    //        Component[] components = g.GetComponents<Component>();

    //        for (int i = 0; i < components.Length; i++)
    //        {
    //            if (components[i] == null)
    //            {
    //                Debug.LogWarning("Missing script in: " + g.name, g);
    //                missingCount++;
    //            }
    //        }
    //    }

    //    Debug.Log($"Scan terminé. {missingCount} script(s) manquant(s).");
    //}
}
