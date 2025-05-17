using UnityEngine;
public class Product
{
    public int productId { get; private set; }
    public ProductTypeEnum productType { get; private set; }
    public Sprite productSprite { get; private set; }
    public float productionTime { get; private set; }
    public Product(int productId, ProductTypeEnum productType, Sprite productSprite, float productionTime)
    {
        this.productId = productId;
        this.productType = productType;
        this.productSprite = productSprite;
        this.productionTime = productionTime;

    }
}
