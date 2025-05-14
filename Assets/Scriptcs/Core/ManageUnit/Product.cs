using UnityEngine;
public class Product
{
    public Sprite productSprite { get; private set; }

    public Product(Sprite productSprite)
    {
        this.productSprite = productSprite;
    }
}
