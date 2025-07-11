using System.Collections.Generic;
using UnityEngine;

public class InConstructionBuildingRepresentation : MonoBehaviour, IGetTeamAndProperties
{
    [SerializeField] private TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingTypeEnum;
    [SerializeField] private GameObject finishBuilding;
    public List<Vector3Int> positionToOccupy;

[SerializeField] private int timeToBuilt;
    List<Gatherer> unitGatheringResourcesList = new();
    public void SetFinishBuilding(GameObject builtToCreate, List<Vector3Int> positionOccupied)
    {
        finishBuilding = builtToCreate;
        this.positionToOccupy = positionOccupied;
    }

    internal void EndProcess()
    {
        GameObject newBuilding = Instantiate(finishBuilding, transform.position, Quaternion.identity);
        Building building = newBuilding.GetComponent<Building>();
        if(building)
        {
            building.SetTeamColor(teamColor);
            building.SetPositionToOccupy(positionToOccupy);
        }
      

        Destroy(gameObject);
    }

    public void WorkOnBuilding(int value)
    {
        if (timeToBuilt > 0)
        {
            timeToBuilt -= value;
        }
        else
        {
            EndProcess();
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
