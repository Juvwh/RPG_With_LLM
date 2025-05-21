using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TXT_OpenURL : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text Text = GetComponent<TMP_Text>();

        // if not using Overlay but use camera instead => change null to Camera.main
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, eventData.position, Camera.main);

        if(linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = Text.textInfo.linkInfo[linkIndex];
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }  
}
