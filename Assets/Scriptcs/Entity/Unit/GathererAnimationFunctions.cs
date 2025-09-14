using System.Linq;
using UnityEngine;
public class GathererAnimationFunctions : UnitAnimationFunctions
{
    [SerializeField] private GathererTaskManager gathererTaskManager;
    public void AddResource()
    {
        Debug.Log("AddResource");
        if (gathererTaskManager.currentGatherableResource.gameObject.activeSelf == false)
        {
            Debug.Log("wylaczone szok");
            gathererTaskManager.GoToNextResource();
            return;
        }
        var obj = gathererTaskManager.gatheredResources.FirstOrDefault(resource => resource.priceType == gathererTaskManager.currentResourceTypeGathering);
        if (obj == null) return;

        obj.AddValue(1);
        gathererTaskManager.currentGathered = obj.priceValue;
        if (gathererTaskManager.currentGatherableResource.Take(gathererTaskManager) == false)
        {
            Debug.Log("JUZ NIE TAKE");
            gathererTaskManager.GoToNextResource();
        }
        else Debug.Log("DAJEL KOPIE");
        if (gathererTaskManager.maxCarried <= gathererTaskManager.currentGathered)
        {
            if (gathererTaskManager.CheckIfGathererHaveToReturnToStockPile())
            {
                Debug.Log("ReturnToStockPile");
                gathererTaskManager.ReturnToStockPile();
                return;

            }
        }

    }

    public void BuildingConstruction()
    {
        if (gathererTaskManager.currentConstionBuildingTarget)
        {
            gathererTaskManager.currentConstionBuildingTarget.WorkOnBuilding(3);

        }

    }
}
