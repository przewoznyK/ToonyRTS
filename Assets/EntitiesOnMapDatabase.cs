using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EntitiesOnMapDatabase : MonoBehaviour
{
    public static EntitiesOnMapDatabase Instance;
    public Dictionary<TeamColorEnum, List<Unit>> unitsListByTeamColor = new();

    private void Awake()
    {
        Instance = this;        
    }
    public void AddUnitToList(TeamColorEnum teamColor, Unit unit)
    {
        if (!unitsListByTeamColor.ContainsKey(teamColor))
            unitsListByTeamColor[teamColor] = new List<Unit>();

        unitsListByTeamColor[teamColor].Add(unit);
    }
    public void RemoveUnitFromList(TeamColorEnum teamColor, Unit unit)
    {
        unitsListByTeamColor[teamColor].Remove(unit);
    }

    public Transform GetClosestTransformEnemyByTeamColor(TeamColorEnum teamColor, Vector3 fromPosition, float maxSearchingDistance)
    {;
        var unitList = unitsListByTeamColor[teamColor];
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
