using UnityEngine;
public class Product
{
    public Sprite productSprite { get; private set; }
    public float productionTime { get; private set; }
    public Product(Sprite productSprite, float productionTime)
    {
        this.productSprite = productSprite;
        this.productionTime = productionTime;
    }
}
