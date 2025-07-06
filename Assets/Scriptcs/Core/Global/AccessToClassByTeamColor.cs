using System.Collections.Generic;
using UnityEngine;

public class AccessToClassByTeamColor : MonoBehaviour
{
    public static AccessToClassByTeamColor instance;
    Dictionary<TeamColorEnum, PlayerResources> PlayersResourcesManagerGlobalList = new();
    Dictionary<TeamColorEnum, ControlledUnits> PlayerControlledUnitsManagerGlobalList = new();
    Dictionary<TeamColorEnum, List<IStockPile>> PlayerStockPileGlobalList = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Player Resource Manager List
    public void AddPlayerResourceManagerToGlobalList(TeamColorEnum teamColor, PlayerResources playerResources)
    {
        PlayersResourcesManagerGlobalList.Add(teamColor, playerResources);
    }

    public PlayerResources GetPlayerResourcesManagerByTeamColor(TeamColorEnum teamColor)
    {
        return PlayersResourcesManagerGlobalList[teamColor];
    }

    // Controlled Units List
    public void AddControlledUnitsManagerToGlobalList(TeamColorEnum teamColor, ControlledUnits controlledUnits)
    {
        PlayerControlledUnitsManagerGlobalList.Add(teamColor, controlledUnits);
    }

    public ControlledUnits GetControlledUnitsByTeamColor(TeamColorEnum teamColorEnum)
    {
        return PlayerControlledUnitsManagerGlobalList[teamColorEnum];
    }

    // StockPile List
    public void AddStockPileToGlobalList(TeamColorEnum teamColor, IStockPile stockPile)
    {
        if (!PlayerStockPileGlobalList.ContainsKey(teamColor))
        {
            PlayerStockPileGlobalList[teamColor] = new List<IStockPile>();
        }

        PlayerStockPileGlobalList[teamColor].Add(stockPile);
    }

    public void RemoveStockPileFromGlobalList(TeamColorEnum teamColor, IStockPile stockPile)
    {
        PlayerStockPileGlobalList[teamColor].Remove(stockPile);
    }

    public IStockPile GetClosestStockPileByTeamColor(TeamColorEnum teamColor, Vector3 fromPosition)
    {
        if (!PlayerStockPileGlobalList.TryGetValue(teamColor, out var stockPiles) || stockPiles.Count == 0)
            return null;

        IStockPile closest = null;
        float minDistance = float.MaxValue;

        foreach (var stockPile in stockPiles)
        {
            float distance = Vector3.Distance(fromPosition, stockPile.stockPilePosition.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = stockPile;
            }
        }
        return closest;
    }

    public Transform GetClosestTransformEnemyByTeamColor(TeamColorEnum teamColor, Vector3 fromPosition, float maxSearchingDistance)
    {
        ControlledUnits controlledUnits = GetControlledUnitsByTeamColor(teamColor);
        List<Unit> unitList = controlledUnits.GetAllUnitList();

        if (unitList.Count == 0)
            return null;

        Transform closest = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in unitList)
        {
            float distance = Vector3.Distance(fromPosition, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = enemy.transform;
            }
        }
        return closest;

    }
}
