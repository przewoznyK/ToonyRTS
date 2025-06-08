using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CancelProductionButton : MonoBehaviour
{
    public void CancelProduction(PlayerResources playerResources, BuildingProduction buildingProduction, Building building, Product product)
    {
        playerResources.AddResources(product.objectPrices);
        buildingProduction.RemoveProductFromProductionDictionary(building, product);
    }
}
