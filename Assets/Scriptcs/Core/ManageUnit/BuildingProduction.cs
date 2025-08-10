using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProduction : MonoBehaviour
{
    TeamColorEnum teamColorEnum;
    CommandPanelUI commandPanelUI;
    Dictionary<Building, BuildingProductionData> productionDictionary = new();
    private Coroutine UIproductionCoroutine;
    public void Init(CommandPanelUI commandPanelUI, TeamColorEnum teamColorEnum)
    {
        this.commandPanelUI = commandPanelUI;
        this.teamColorEnum = teamColorEnum;
    }
    public void CreateProductAndAddToProductionDictionary(Building building, UnitData unitData)
    {
        var productSprite = unitData.unitSprite;
        var newProduct = new Product(unitData.unitID, ProductTypeEnum.unit, productSprite, unitData.productionTime, unitData.objectPrices);

        AddProductToProductionDictionary(building, newProduct);
    }

    void AddProductToProductionDictionary(Building building, Product product)
    {
        bool existingKey = productionDictionary.ContainsKey(building);
        if (existingKey == false)
            productionDictionary[building] = new BuildingProductionData();

        productionDictionary[building].productQueue.Enqueue(product);
        commandPanelUI.DisplayProductionQueue(building);

        if (existingKey == false)
            productionDictionary[building].activeCoroutine = StartCoroutine(Production(building));
    }

    public Queue<Product> GetProductsFromThisBuilding(Building building)
    {
        if (productionDictionary.TryGetValue(building, out var productionData))
            return productionData.productQueue;
        else
            return null;
    }

    IEnumerator Production(Building building)
    {
        // Start Production
        float productionTime = productionDictionary[building].productQueue.Peek().productionTime;
        productionDictionary[building].timeProduction = productionTime;
        StartCoroutine(productionDictionary[building].StartProduction());
        // Wait X Time
        yield return new WaitUntil(() => productionDictionary[building].endProduction);
      
        productionDictionary[building].endProduction = false;

        PlayerRoomController.LocalPlayer.CmdSpawnUnit(building.GetComponent<NetworkIdentity>() ,productionDictionary[building].productQueue.Dequeue().productId, teamColorEnum);

        // Next producion if exist
        if (productionDictionary[building].productQueue.Count > 0)
        {
            productionDictionary[building].activeCoroutine = StartCoroutine(Production(building));
        }
        else
            productionDictionary.Remove(building);

        if(commandPanelUI.currentSelectedBuilding == building)
            commandPanelUI.DisplayProductionQueue(building);
    }

    public void RemoveProductFromProductionDictionary(Building building, Product product)
    {

        var originalQueue = productionDictionary[building].productQueue;
        var copyQueue = new Queue<Product>(originalQueue);
        var newQueue = new Queue<Product>();

        while (copyQueue.Count > 0)
        {
            var p = copyQueue.Dequeue();
            if (p.Id != product.Id)  
            {
                newQueue.Enqueue(p);
            }
        }
        productionDictionary[building].productQueue = newQueue;
        if (productionDictionary[building].productQueue.Count <= 0)
        {
            StopCoroutine(productionDictionary[building].activeCoroutine);
            productionDictionary.Remove(building);
        }
        commandPanelUI.DisplayProductionQueue(building);
    }

    public BuildingProductionData GetProductingData(Building building)
    {
        if (productionDictionary.ContainsKey(building))
            return productionDictionary[building];
        else
            return null;
    }
}
