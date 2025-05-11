using System.Collections.Generic;
using System.Diagnostics;
public class PlayerResources
{
    public int currentFood { get; private set; }
    public int currentWood { get; private set; }
    public int currentGold { get; private set; }
    public int currentRock { get; private set; }

    public PlayerResources(int currentFood, int currentWood, int currentGold, int currentRock)
    {
        this.currentFood = currentFood;
        this.currentWood = currentWood;
        this.currentGold = currentGold;
        this.currentRock = currentRock;
    }

    public bool CanPlayerBuyIt(List<ObjectPrices> objectPrices)
    {
        foreach (var price in objectPrices)
        {
            switch (price.priceType)
            {
                case ResourceTypesEnum.food:
                    if (currentFood < price.priceValue) return false;
                    break;
                case ResourceTypesEnum.wood:
                    if (currentWood < price.priceValue) return false;
                    break;
                case ResourceTypesEnum.gold:
                    if (currentGold < price.priceValue) return false;
                    break;
                case ResourceTypesEnum.rock:
                    if (currentRock < price.priceValue) return false;
                    break;
                default:
                    break;
            }
        }
        return true;
    }
}
