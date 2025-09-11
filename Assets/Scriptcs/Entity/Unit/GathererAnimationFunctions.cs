using System.Linq;
using UnityEngine;
public class GathererAnimationFunctions : UnitAnimationFunctions
{
    [SerializeField] private GathererTaskManager gathererTaskManager;
    public void AddResource()
    {
        var obj = gathererTaskManager.gatheredResources.FirstOrDefault(resource => resource.priceType == gathererTaskManager.currentResourceTypeGathering);
        if (obj != null)
        {
            if (gathererTaskManager.currentGatherableResource == null) return;
            obj.AddValue(1);
            gathererTaskManager.currentGathered = obj.priceValue;
            if (gathererTaskManager.currentGatherableResource.Take(gathererTaskManager) == false)
            {
                if (gathererTaskManager.maxCarried <= gathererTaskManager.currentGathered)
                {
                    if (gathererTaskManager.CheckIfGathererHaveToReturnToStockPile())
                    {
                        gathererTaskManager.ReturnToStockPile();
                          return;

                    }
                }

                gathererTaskManager.GoToNextResource();
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
