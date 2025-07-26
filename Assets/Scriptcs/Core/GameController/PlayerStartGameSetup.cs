using System;
using UnityEngine;

public class PlayerStartGameSetup : MonoBehaviour
{
    [SerializeField] private BuildingData buildingData;
    internal void Init(PlayerResources playerResources, ConstructionPlacerSystem constructionPlacerSystem, GridData gridData, TeamColorEnum teamColor, int xPosition, int zPosition)
    {
        ConstructionData currentConstructionData = new (buildingData, xPosition, zPosition, teamColor);
        constructionPlacerSystem.PlaceConstruction(playerResources, gridData, currentConstructionData);
    }

}
