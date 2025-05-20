using System.Collections.Generic;
using UnityEngine;
public class Product
{
    private static int nextId = 1;
    public int Id { get; private set; }
    public int productId { get; private set; }
    public ProductTypeEnum productType { get; private set; }
    public Sprite productSprite { get; private set; }
    public float productionTime { get; private set; }
    public List<ObjectPrices> objectPrices { get; private set; } = new List<ObjectPrices>();
    public Product(int productId, ProductTypeEnum productType, Sprite productSprite, float productionTime, List<ObjectPrices> objectPrices)
    {
        this.Id = nextId++;
        this.productId = productId;
        this.productType = productType;
        this.productSprite = productSprite;
        this.productionTime = productionTime;
        this.objectPrices = objectPrices;
    }
}
