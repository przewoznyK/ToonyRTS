using System;
using UnityEngine;
using UnityEngine.AI;

public class GatherableResource : MonoBehaviour, IGetTeamAndProperties
{
    [SerializeField] private TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingType;
    [SerializeField] public ResourceTypesEnum resourceType;
    [SerializeField] private int _totalAvailable = 20;

    public int _available;
    public bool IsDepleted => _available <= 0;

    private void OnEnable()
    {
        _available = _totalAvailable;
    }

    public bool Take()
    {
        if (_available <= 0)
            return false;
        _available--;

        UpdateSize();

        return true;
    }

    private void UpdateSize()
    {
        float scale = (float)_available / _totalAvailable;
        if (scale > 0 && scale < 1f)
        {
            var vectorScale = Vector3.one * scale;
            transform.localScale = vectorScale;
        }
        else if (scale <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log("Doda�: modyfikacje NavMeshSurface aby jednostki mog�y przechodzi� po tym terenie");
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

    public void SetAvailable(int amount) => _available = amount;

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