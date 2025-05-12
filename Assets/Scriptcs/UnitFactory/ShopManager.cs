using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager
{
    PlayerResources playerResources;

    public ShopManager(PlayerResources playerResources)
    {
        this.playerResources = playerResources;
    }

    public void BuyUnit(Building building, UnitNameEnum unitNameEnum)
    {
        UnitData unitData = UnitDatabase.Instance.GetUnitDataByNameEnum(unitNameEnum);
        building.AddToProductionQueue(unitData);

        // Spend resources
        List<ObjectPrices> objectPrices = new List<ObjectPrices>();
        objectPrices = unitData.objectPrices;
        playerResources.SpendResources(objectPrices);
               
    }
}
