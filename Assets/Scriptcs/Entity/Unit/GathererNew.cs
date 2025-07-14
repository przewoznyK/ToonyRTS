using System;
using UnityEngine;

public class GathererNew : Unit
{
    public override void PlayerRightMouseButtonCommand(RaycastHit hit, bool isShiftPressed)
    {
        if (isShiftPressed == false)
            unitTaskManager.ResetTasks();

        if (hit.collider.CompareTag("Ground"))
        {
            unitTaskManager.GoToPosition(hit.point);

        }
        else if (hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
        {
            if ((component.GetTeam() & teamColor) != 0)
            {
                if (component.GetBuildingType() == BuildingTypeEnum.resource)
                {
                    unitTaskManager.GatherResource(component.GetProperties<GatherableResource>());
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
                unitTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
                return;
            }
        }
    }
    internal void BuildConstruction(GameObject constructionInstantiate)
    {
        unitTaskManager.BuildConstructionTask(constructionInstantiate);
    }
}
