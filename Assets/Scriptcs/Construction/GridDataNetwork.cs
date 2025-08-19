using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GridDataNetwork : NetworkBehaviour
{
    public class PlacementDataSync : SyncDictionary<Vector3Int, PlacementData> { }

    public PlacementDataSync placedObjects = new PlacementDataSync();

    [Server]
    public void AddObjectAt(Vector3Int cellPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex, ConstructionData constructionData)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(cellPosition, objectSize);
  
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
           
        }
        constructionData.buildingData.positionToOccupy = positionToOccupy;
    }

    private List<Vector3Int> CalculatePositions(Vector3Int cellPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        Vector3Int bottomLeft = cellPosition - new Vector3Int(Mathf.FloorToInt(objectSize.x / 2f), 0, Mathf.FloorToInt(objectSize.y / 2f));
        bottomLeft.y = 0;
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(bottomLeft + new Vector3Int(x, 0, y));
            }
        }
        
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int cellPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(cellPosition, objectSize);
  
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    internal void RemoveObjectAt(List<Vector3Int> positionToRemove)
    {
        foreach (var pos in positionToRemove)
        {
            placedObjects.Remove(pos);
        }
    }

}