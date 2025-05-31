using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class StockPile : MonoBehaviour
{
    [SerializeField] private TeamColorEnum teamColorEnum;
    [SerializeField] private ResourceTypesEnum stockPileType;
    private int _gathered;

    private void OnEnable()
    {
       // _gathered = 0;

    }

    public List<ObjectPrices> Add(List<ObjectPrices> objectPrices)
    {
        var playerResource = AccessToClassByTeamColor.instance.GetPlayerResourcesManagerByTeamColor(teamColorEnum);
        List<ObjectPrices> newObjectPrices = new();
        if (stockPileType == ResourceTypesEnum.allTypes)
        {
            playerResource.AddResources(newObjectPrices);
            var allowedTypes = new List<ResourceTypesEnum>
            {
                ResourceTypesEnum.food,
                ResourceTypesEnum.wood,
                ResourceTypesEnum.gold,
                ResourceTypesEnum.stone
            };

            foreach (ResourceTypesEnum type in allowedTypes)
            {
                newObjectPrices.Add(new ObjectPrices(type, 0));
            }

        }

        return newObjectPrices;

    }
}