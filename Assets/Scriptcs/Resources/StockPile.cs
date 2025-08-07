using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StockPile : MonoBehaviour, IGetTeamAndProperties
{
    [SerializeField] private TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingTypeEnum;
    [SerializeField] private ResourceTypesEnum stockPileType;

    private void Start()
    {
      //  AccessToClassByTeamColor.instance.AddStockPileToGlobalList(teamColor, this);    
    }

    public List<ObjectPrices> Add(List<ObjectPrices> objectPrices)
    {
        var playerResource = AccessToClassByTeamColor.Instance.GetPlayerResourcesManagerByTeamColor(teamColor);
        if (stockPileType == ResourceTypesEnum.allTypes)
        {
            playerResource.AddResources(objectPrices);
            foreach (var obj in objectPrices)
            {
               obj.SetValue(0);
            }
        }
        return objectPrices;
    }

    public TeamColorEnum GetTeam()
    {
        return teamColor;
    }
    public EntityTypeEnum GetEntityType()
    {
        return entityType;
    }
    public BuildingTypeEnum GetBuildingType()
    {
        return buildingTypeEnum;
    }
    public T GetProperties<T>() where T : Component
    {
        if (typeof(T) == typeof(StockPile))
            return this as T;
        else
            Debug.Log("You can only take StockPile from this");
        return null;
    }


}