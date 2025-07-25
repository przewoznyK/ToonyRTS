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
            if (gathererTaskManager.currentGatherableResource.Take(gathererTaskManager) == false || gathererTaskManager.currentGathered >= gathererTaskManager.maxCarried)
            {
                if (gathererTaskManager.CheckIfGathererHaveToReturnToStockPile())
                    gathererTaskManager.ReturnToStockPile();
                else
                    gathererTaskManager.GoToNextResource();  
            }
        }
    }

    public void BuildingConstruction()
    {
        if (gathererTaskManager.currentConstionBuildingTarget)
            if(gathererTaskManager.currentConstionBuildingTarget.WorkOnBuilding(3) == false)
                gathererTaskManager.ResetGathererProperties();
    }
}
