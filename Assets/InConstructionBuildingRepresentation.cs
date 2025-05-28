using System.Collections.Generic;
using UnityEngine;

public class InConstructionBuildingRepresentation : MonoBehaviour
{
    [SerializeField] private GameObject finishBuilding;
    private TeamColorEnum teamColorToSet;
    [SerializeField] private int timeToBuilt;
    List<Gatherer> unitGatheringResourcesList = new();
    public void SetFinishBuilding(GameObject builtToCreate)
    {
        finishBuilding = builtToCreate;
    }

    internal void EndProcess()
    {
        GameObject newBuilding = Instantiate(finishBuilding, transform.position, Quaternion.identity);
      //  newBuilding.GetComponent<Building>().SetTeamColor(teamColorToSet);
        foreach (var unitGathering in unitGatheringResourcesList)
        {

            unitGathering.enabled = false;
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

    //public void AddToActiveBuildersList(UnitBuilderBuildObject unitBuilderBuildObject)
    //{
    //    unitGatheringResourcesList.Add(unitBuilderBuildObject);
    //}
}
