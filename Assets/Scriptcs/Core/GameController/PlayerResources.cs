using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class PlayerResources
{
    SummaryPanelUI summaryPanelUI;
    CommandPanelUI commandPanelUI;
    public int currentFood { get; private set; }
    public int currentWood { get; private set; }
    public int currentGold { get; private set; }
    public int currentRock { get; private set; }

    public PlayerResources(SummaryPanelUI summaryPanelUI, CommandPanelUI commandPanelUI, int currentFood, int currentWood, int currentGold, int currentRock)
    {
        this.commandPanelUI = commandPanelUI;
        this.summaryPanelUI = summaryPanelUI;
        this.currentFood = currentFood;
        this.currentWood = currentWood;
        this.currentGold = currentGold;
        this.currentRock = currentRock;

        summaryPanelUI.UpdateResource(ResourceTypesEnum.food, currentFood);
        summaryPanelUI.UpdateResource(ResourceTypesEnum.wood, currentWood);
        summaryPanelUI.UpdateResource(ResourceTypesEnum.gold, currentGold);
        summaryPanelUI.UpdateResource(ResourceTypesEnum.stone, currentRock);
    }

    public void AddResources(List<ObjectPrices> objectPrices)
    {
        foreach (var price in objectPrices)
        {
            
            switch (price.priceType)
            {
                case ResourceTypesEnum.food:
                    currentFood += price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.food, currentFood);
                    break;
                case ResourceTypesEnum.wood:
                    currentWood += price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.wood, currentWood);
                    break;
                case ResourceTypesEnum.gold:
                    currentGold += price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.gold, currentGold);
                    break;
                case ResourceTypesEnum.stone:
                    currentRock += price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.stone, currentRock);
                    break;
                default:
                    break;
            }
        }
        commandPanelUI.RefreshButtonsStatus();

    }

    public void AddSingleResource(ResourceTypesEnum resourceType, int value)
    {
        switch (resourceType)
        {
            case ResourceTypesEnum.food:
                currentFood += value;
                summaryPanelUI.UpdateResource(ResourceTypesEnum.food, currentFood);
                break;
            case ResourceTypesEnum.wood:
                currentWood += value;
                summaryPanelUI.UpdateResource(ResourceTypesEnum.wood, currentWood);
                break;
            case ResourceTypesEnum.gold:
                currentGold += value;
                summaryPanelUI.UpdateResource(ResourceTypesEnum.gold, currentGold);
                break;
            case ResourceTypesEnum.stone:
                currentRock += value;
                summaryPanelUI.UpdateResource(ResourceTypesEnum.stone, currentRock);
                break;
            default:
                break;
        }

        currentWood += value;
        summaryPanelUI.UpdateResource(ResourceTypesEnum.wood, currentWood);

        commandPanelUI.RefreshButtonsStatus();
    }
    public void SpendResources(List<ObjectPrices> objectPrices)
    {
        foreach (var price in objectPrices)
        {
            switch (price.priceType)
            {
                case ResourceTypesEnum.food:
                    currentFood -= price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.food, currentFood);
                    break;
                case ResourceTypesEnum.wood:
                    currentWood -= price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.wood, currentWood);
                    break;
                case ResourceTypesEnum.gold:
                    currentGold -= price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.gold, currentGold);
                    break;
                case ResourceTypesEnum.stone:
                    currentRock -= price.priceValue;
                    summaryPanelUI.UpdateResource(ResourceTypesEnum.stone, currentRock);
                    break;
                default:
                    break;
            }
        }
        commandPanelUI.RefreshButtonsStatus();
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
                case ResourceTypesEnum.stone:
                    if (currentRock < price.priceValue) return false;
                    break;
                default:
                    break;
            }
        }
        return true;
    }
}
