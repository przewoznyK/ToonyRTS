using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager
{
    BuildingProduction buildingProduction;
    public ShopManager(BuildingProduction buildingProduction)
    {
        this.buildingProduction = buildingProduction;
    }

    public void BuyUnit(PlayerResources playerResources, Building building, UnitNameEnum unitNameEnum)
    {
        UnitData unitData = UnitDatabase.Instance.GetUnitDataByNameEnum(unitNameEnum);
        
        // Spend resources
        List<ObjectPrices> objectPrices = new List<ObjectPrices>();
        objectPrices = unitData.objectPrices;
        playerResources.SpendResources(objectPrices);

        buildingProduction.CreateProductAndAddToProductionDictionary(building, unitData);
    }

    
}
