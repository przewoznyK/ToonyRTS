using UnityEngine;

public class GathererNew : Unit
{
    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false && unitTaskManager.requestedTasks.Count > 0)
            unitTaskManager.RequestToServerToResetTasks();

        if (hit.collider.CompareTag("Ground"))
        {
            unitTaskManager.RequestToServerToCreateGoToPositionTask(hit.point);

        }
        else if (hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() & teamColor) != 0)
            {
                if (component.GetBuildingType() == BuildingTypeEnum.resource)
                {
                    unitTaskManager.GatherResourceTask(component.GetProperties<GatherableResource>());
                    return;
                }
            }
            if (component.GetTeam() == teamColor)
            {
                if (component.GetBuildingType() == BuildingTypeEnum.contructionToBuild)
                {
                    GameObject buildingRepresentation = component.GetProperties<InConstructionBuildingRepresentation>().gameObject;
                    BuildConstruction(buildingRepresentation);
                    return;
                }
            }
            if(component.GetTeam() != teamColor)
            {
                unitTaskManager.RequestToServerToCreateAttackEntityTask(component.GetTeam(), component.GetProperties<Transform>());
                return;
            }
        }
    }
    internal void BuildConstruction(GameObject constructionInstantiate)
    {
        unitTaskManager.BuildConstructionTask(constructionInstantiate);
    }
}
