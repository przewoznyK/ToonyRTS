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

            if (gathererTaskManager.currentGatherableResource.Take())
                gathererTaskManager.currentGathered = obj.priceValue;

            if (gathererTaskManager.CheckIfGathererHaveToReturnToStockPile())
                gathererTaskManager.ReturnToStockPile();
            else if(gathererTaskManager.currentGatherableResource._available <= 0)
                gathererTaskManager.GoToNextResource();

        }
    }

    public void BuildingConstruction()
    {
        if (gathererTaskManager.currentConstionBuildingTarget)
            if(gathererTaskManager.currentConstionBuildingTarget.WorkOnBuilding(3) == false)
                gathererTaskManager.ResetGathererProperties();
    }
}
