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
            if ((component.GetTeam() & teamColor) == teamColor || (component.GetTeam() == TeamColorEnum.Neutral))
            {
                if (component.GetBuildingType() == BuildingTypeEnum.resource)
                {
                    unitTaskManager.GatherResource(component.GetProperties<GatherableResource>());
                }
                else if((component.GetTeam() & teamColor) != teamColor)
                {
              
                    unitTaskManager.AttackTarget(component.GetProperties<Transform>(), component.GetTeam());
                }
            }
                


        }
    }
    internal void BuildConstruction(GameObject constructionInstantiate)
    {
        unitTaskManager.BuildConstructionTask(constructionInstantiate);
    }


}
