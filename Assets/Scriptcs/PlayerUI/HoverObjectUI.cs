using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverObjectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image hoverImage;
    private void OnEnable()
    {
        SetAlpha(0f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color color = hoverImage.color;
        color.a = alpha;
        hoverImage.color = color;
    }
}

