using System.Collections;
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
        var newProduct = new Product(productSprite, unitData.productionTime);

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
        StartCoroutine(Production(building, productionDictionary[building]));
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

    IEnumerator Production(Building building, Queue<Product> products)
    {

        float productionTime = products.Peek().productionTime;
        commandPanelUI.StartUpdatingProductionImageFill(productionTime);
        yield return new WaitForSeconds(productionTime);
        products.Dequeue();
        commandPanelUI.DisplayProductionQueue(building);
        if (products.Count > 0)
            StartCoroutine(Production(building, products));
        else
        {
            productionDictionary.Remove(building);

        }
    }

}
