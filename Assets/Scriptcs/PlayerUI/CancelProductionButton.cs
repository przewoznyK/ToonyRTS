using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancelProductionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image hoverImage;
    private void Start()
    {
        SetAlpha(0f);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetAlpha(1f);
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

    public void CancelProduction(PlayerResources playerResources, BuildingProduction buildingProduction, Building building, Product product)
    {
        playerResources.AddResources(product.objectPrices);
        buildingProduction.RemoveProductFromProductionDictionary(building, product);
    }
}
