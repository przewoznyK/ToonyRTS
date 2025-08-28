using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class InConstructionBuildingRepresentation : NetworkBehaviour, IGetTeamAndProperties
{
    public TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingTypeEnum;
    public GameObject finishBuilding;
    public List<Vector3Int> positionToOccupy;
    public List<GathererNew> gatherersBuildingThisConstruction;


    [SerializeField] private int timeToBuilt;
    public void SetFinishBuilding(GameObject builtToCreate, List<Vector3Int> positionOccupied)
    {
        finishBuilding = builtToCreate;
        this.positionToOccupy = positionOccupied;
    }

    internal void EndProcess()
    {
        foreach (var gatherer in gatherersBuildingThisConstruction)
        {
            gatherer.unitTaskManager.StopBuildingThisConstruction();
        }
        PlayerController.LocalPlayer.CmdContructionEndProcess(this.GetComponent<NetworkIdentity>(), finishBuilding);
        PlayerController.LocalPlayer.CmdRemoveGameObject(this.gameObject);
    }

    public bool WorkOnBuilding(int value)
    {
        if (timeToBuilt > 0)
        {
            timeToBuilt -= value;
            return true;
        }
        else
        {
            EndProcess();
            return false;
        }
    }

    public TeamColorEnum GetTeam()
    {
        return teamColor;
    }

    public EntityTypeEnum GetEntityType()
    {
        return entityType;
    }
    public BuildingTypeEnum GetBuildingType()
    {
        return buildingTypeEnum;
    }

    public T GetProperties<T>() where T : Component
    {
        if (typeof(T) == typeof(InConstructionBuildingRepresentation))
            return this as T;
        else
            Debug.Log("You can only take InConstructionBuildingRepresentation from this");
        return null;
    }


}
