using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingProduction : MonoBehaviour
{
    CommandPanelUI commandPanelUI;
    Dictionary<Building, Queue<Product>> productionDictionary = new();
    private Coroutine productionCoroutine;
    public void Init(CommandPanelUI commandPanelUI)
    {
        this.commandPanelUI = commandPanelUI;
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
            productionDictionary[building] = new Queue<Product>();

        productionDictionary[building].Enqueue(product);
        commandPanelUI.DisplayProductionQueue(building);

        if (existingKey == false)
            productionCoroutine = StartCoroutine(Production(building));

        
    }

    public Queue<Product> GetProductsFromThisBuilding(Building building)
    {
        if (productionDictionary.TryGetValue(building, out var products))
            return products;
        else
            return null;
    }

    IEnumerator Production(Building building)
    {
        float productionTime = productionDictionary[building].Peek().productionTime;
        StartCoroutine(commandPanelUI.UpdateCurrentProductionImageFill(productionTime));

        yield return new WaitForSeconds(productionTime);
        if (!productionDictionary.ContainsKey(building) || productionDictionary[building] == null || productionDictionary[building].Count == 0)
        {
            productionDictionary.Remove(building);

            yield break;

        }
        building.SpawnUnit(productionDictionary[building].Dequeue().productId);

        if (productionDictionary[building].Count > 0)
        {
            productionCoroutine = StartCoroutine(Production(building));
        }
        else
            productionDictionary.Remove(building);

        commandPanelUI.DisplayProductionQueue(building);
    }

    public void RemoveProductFromProductionDictionary(Building building, Product product)
    {

        var originalQueue = productionDictionary[building];
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
        productionDictionary[building] = newQueue;
        if (productionDictionary[building].Count <= 0)
        {
            StopCoroutine(productionCoroutine);
            productionCoroutine = null;
        }
        commandPanelUI.DisplayProductionQueue(building);
    }
}
