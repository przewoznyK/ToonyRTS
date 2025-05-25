using System;
using System.Collections.Generic;
using UnityEngine;
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public int placementID { get; private set; }
    public int placedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int placementID, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.placementID = placementID;
        this.placedObjectIndex = placedObjectIndex;
    }
}
