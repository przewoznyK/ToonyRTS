using Mirror;
using System;
using UnityEngine;
using UnityEngine.AI;

public class GatherableResource : NetworkBehaviour, IGetTeamAndProperties
{
    [SerializeField] private TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingType;
    [SerializeField] public ResourceTypesEnum resourceType;

    [SerializeField] private int totalAvailable = 20;

    [SyncVar(hook = nameof(OnAvailableChanged))]
    public int available;

    private void OnEnable()
    {
        available = totalAvailable;
    }

    public bool Take(GathererTaskManager gathererTaskManager)
    {
        PlayerController.LocalPlayer.CmdTakeResource(this.netIdentity);
        Debug.Log(available);
        if (available <= 0)
        {
            gathererTaskManager.currentGatherableResource = null;
            PlayerController.LocalPlayer.CmdRemoveGameObject(this.gameObject);
            return false;
        }
        return true;
    }

    private void OnAvailableChanged(int oldValue, int newValue)
    {
        float scale = (float)available / totalAvailable;
        if (scale > 0 && scale < 1f)
        {
            var vectorScale = Vector3.one * scale;
            transform.localScale = vectorScale;
        }
    }

    [ContextMenu("Snap")]
    private void Snap()
    {
        if (NavMesh.SamplePosition(transform.position, out var hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }

    public void SetAvailable(int amount) => available = amount;

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
        return buildingType;
    }

    public T GetProperties<T>() where T : Component
    {
        if (typeof(T) == typeof(GatherableResource))
            return this as T;
        else
            Debug.Log("You can only get GatherableResource from this");
        return null;
    }


}