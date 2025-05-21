using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class S_FadeInZoomEffect : MonoBehaviour
{
    public RawImage rawImageToStylize; // Le RawImage sur lequel appliquer l'effet
    public float fadeDuration = 2f;
    public float zoomDuration = 2f;
    public Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f); // Taille initiale r�duite

    public void StartEffect()
    {
        // R�cup�re la couleur actuelle du RawImage
        Color currentColor = rawImageToStylize.color;
        currentColor.a = 1.0f;
        rawImageToStylize.color = currentColor;


        if (rawImageToStylize.texture != null)
        {
            // D�marrer avec une transparence compl�te et une �chelle r�duite
            rawImageToStylize.canvasRenderer.SetAlpha(0.0f);
            rawImageToStylize.transform.localScale = startScale;

            // Lancer l'effet de fondu et de zoom simultan�ment
            StartCoroutine(FadeInAndZoom());
        }

    }

    IEnumerator FadeInAndZoom()
    {
        // Commencer � faire appara�tre le RawImage
        rawImageToStylize.CrossFadeAlpha(1.0f, fadeDuration, false);

        // Zoom progressif
        float currentTime = 0f;
        Vector3 targetScale = Vector3.one;

        while (currentTime < zoomDuration)
        {
            currentTime += Time.deltaTime;
            rawImageToStylize.transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime / zoomDuration);
            yield break;
        }

        rawImageToStylize.transform.localScale = targetScale; // Fixer la taille finale
    }
}
