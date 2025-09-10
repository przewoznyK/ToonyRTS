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

    public Unit GetClosestTransformEnemyByTeamColor(TeamColorEnum teamColor, Vector3 fromPosition, float maxSearchingDistance)
    {
        //Debug.Log("GetClosestTransformEnemyByTeamColor 1 " + maxSearchingDistance);
        var unitList = unitsListByTeamColor[teamColor];
        if (unitList == null || unitList.Count == 0)
            return null;
      //  Debug.Log("GetClosestTransformEnemyByTeamColor 2");
        Unit closestEnemy = null;
        float minDistance = maxSearchingDistance * maxSearchingDistance;

        foreach (var enemy in unitList)
        {
            if (enemy == null)
                continue;
            float distanceSqr = (fromPosition - enemy.transform.position).sqrMagnitude;

            Debug.Log("SZUKAM ENEMY " + distanceSqr + "   " + minDistance);
            
            if (distanceSqr < minDistance)
            {
                Debug.Log("MAM CLOSEST ENEMY");
                minDistance = distanceSqr;
                closestEnemy = enemy;
            }
        }
        
       // Debug.Log("GetClosestTransformEnemyByTeamColor 3 " + closestEnemy);
        return closestEnemy;
    }

}
