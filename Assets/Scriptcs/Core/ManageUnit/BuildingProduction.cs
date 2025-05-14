using System.Collections.Generic;
using UnityEngine;

public class BuildingProduction : MonoBehaviour
{
    CommandPanelUI commandPanelUI;
    Dictionary<Building, Queue<Product>> productionDictionary = new();

    public void Init(CommandPanelUI commandPanelUI)
    {
        this.commandPanelUI = commandPanelUI;
    }
    public void CreateProductAndAddToProductionDictionary(Building building, UnitData unitData)
    {
        var productSprite = unitData.unitSprite;
        var newProduct = new Product(productSprite);

        AddProductToProductionDictionary(building, newProduct);
    }

    void AddProductToProductionDictionary(Building building, Product product)
    {
        if(!productionDictionary.ContainsKey(building))
        {
            productionDictionary[building] = new Queue<Product>();
        }

        productionDictionary[building].Enqueue(product);

        commandPanelUI.DisplayProductionQueue(building);
        
    }

    public Queue<Product> GetProductsFromThisBuilding(Building building)
    {
        if (productionDictionary.TryGetValue(building, out var products))
        {
            return products;
        }
        else
        {
            return null;
        }
    }

    private void Update()
    {
        foreach (var kvp in productionDictionary)
        {
            if (kvp.Value.Count > 0)
            {
                Debug.Log(kvp.Value.Peek());

            }
        }
    }
}
