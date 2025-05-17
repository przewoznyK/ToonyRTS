using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager
{
    PlayerResources playerResources;
    BuildingProduction buildingProduction;
    public ShopManager(PlayerResources playerResources, BuildingProduction buildingProduction)
    {
        this.playerResources = playerResources;
        this.buildingProduction = buildingProduction;
    }

    public void BuyUnit(Building building, UnitNameEnum unitNameEnum)
    {
        UnitData unitData = UnitDatabase.Instance.GetUnitDataByNameEnum(unitNameEnum);
        
        // Spend resources
        List<ObjectPrices> objectPrices = new List<ObjectPrices>();
        objectPrices = unitData.objectPrices;
        playerResources.SpendResources(objectPrices);

        buildingProduction.CreateProductAndAddToProductionDictionary(building, unitData);
    }
}
