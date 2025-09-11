using System.Collections.Generic;
using UnityEngine;
public interface IStockPile
{
    Transform stockPilePosition { get; }
    Vector2 stockPileSize { get; }
    public List<ObjectPrices> AddResourcesToStockPile(List<ObjectPrices> objectPrices);
}
