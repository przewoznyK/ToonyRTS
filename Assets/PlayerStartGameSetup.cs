using System;
using UnityEngine;

public class PlayerStartGameSetup : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    internal void Init(PlayerResources playerResources, ConstructionPlacerSystem constructionPlacerSystem, GridData gridData)
    {
        ConstructionData currentConstructionData = new (buildingData, 0, 0);
        constructionPlacerSystem.PlaceConstruction(gridData, currentConstructionData);
    }

}
