using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 0.5f;
    public bool scrollUp;

    private bool isScrolling;

    void Update()
    {
        if (isScrolling)
        {
            float direction = scrollUp ? 1 : -1;
            scrollRect.verticalNormalizedPosition += direction * scrollSpeed * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isScrolling = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isScrolling = false;
    }
}
