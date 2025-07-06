using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDatabase : MonoBehaviour
{
    public static BuildingDatabase Instance;
    [SerializeField]
    private List<BuildingData> buildingDataList = new();
    private void Awake()
    {
        Instance = this;
    }
    public BuildingData GetBuildingDataByID(int ID)
    {
        return buildingDataList.Find(building => building.buildingID == ID);
    }

    public BuildingData GetBuildingDataByNameEnum(BuildingNameEnum buildingName)
    {
        return buildingDataList.Find(building => building.buildingName == buildingName);
    }

    internal List<BuildingData> GetBuildingList()
    {
        return buildingDataList;
    }
}
