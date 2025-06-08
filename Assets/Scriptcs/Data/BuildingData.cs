using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Building/Building Data", order = 1)]
public class BuildingData : ScriptableObject
{
    public int buildingID;
    public BuildingNameEnum buildingName;
    public int health;
    public Sprite buildingSprite;
    public GameObject buildingPrefab;
    [field: SerializeField] public Vector2Int size { get; private set; }
    [field: SerializeField] public List<ObjectPrices> objectPrices = new();
    [field: SerializeField] public List<Vector3Int> positionToOccupy;
}
