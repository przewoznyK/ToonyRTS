using System.Collections.Generic;
using UnityEngine;

public class InConstructionBuildingRepresentation : MonoBehaviour, IGetTeamAndProperties
{
    [SerializeField] private TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private GameObject finishBuilding;

    [SerializeField] private int timeToBuilt;
    List<Gatherer> unitGatheringResourcesList = new();
    public void SetFinishBuilding(GameObject builtToCreate)
    {
        finishBuilding = builtToCreate;
    }

    internal void EndProcess()
    {
        GameObject newBuilding = Instantiate(finishBuilding, transform.position, Quaternion.identity);
        newBuilding.GetComponent<Building>().SetTeamColor(teamColor);
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

    public T GetProperties<T>() where T : Component
    {
        if (typeof(T) == typeof(InConstructionBuildingRepresentation))
            return this as T;
        else
            Debug.Log("You can only take InConstructionBuildingRepresentation from this");
        return null;
    }

}
