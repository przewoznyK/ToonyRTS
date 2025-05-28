using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int cellPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(cellPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
            Debug.Log(pos);
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int cellPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        Vector3Int bottomLeft = cellPosition - new Vector3Int(Mathf.FloorToInt(objectSize.x / 2f), 0, Mathf.FloorToInt(objectSize.y / 2f));
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
}