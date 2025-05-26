using System;
using UnityEngine;

public class ConstructionPlacerSystem : MonoBehaviour
{
    internal void PlaceConstruction(GridData gridData, ConstructionData currentConstructionData)
    {
        gridData.AddObjectAt(currentConstructionData.positionToOccupy, currentConstructionData.buildingData.size, 1 ,1 );
        currentConstructionData.previewObjectToPlaceMeshRenderer.material = currentConstructionData.originalMaterial;
    }
}
