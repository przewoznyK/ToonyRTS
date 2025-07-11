using System;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour, IActiveClickable, IStockPile, IGetTeamAndProperties
{
    [SerializeField] public List<Vector3Int> positionToOccupy;
    [SerializeField] private TeamColorEnum teamColor;
    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingType;
    [SerializeField] private List<UnitNameEnum> unitsToBuy;
    [SerializeField] private Transform meetingPoint;

    [Header("OPTIONAL STOCKPILE")]
    [SerializeField] private ResourceTypesEnum stockPileType;
    [field: SerializeField] public bool isStockPile { get; private set; }
    public Transform stockPilePosition => transform;

    private void Start()
    {
        if (isStockPile)
            AccessToClassByTeamColor.instance.AddStockPileToGlobalList(teamColor, this);
    }
    public ObjectTypeEnum CheckObjectType()
    {
        return ObjectTypeEnum.building;
    }

    public List<UnitNameEnum> GetUnitsCanBuyList() => unitsToBuy;

    void IActiveClickable.ActiveObject()
    {
        meetingPoint.gameObject.SetActive(true);
    }
    public void DisableObject()
    {
        meetingPoint.gameObject.SetActive(false);
    }
    internal void SetMeetingPoint(Vector3 newMeetingPointPosition) => meetingPoint.transform.position = newMeetingPointPosition;

    public void SpawnUnit(int unitID, TeamColorEnum teamColorEnum)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        
        var unit = unitInstantiate.GetComponent<Unit>();
        unit.GoMeetingPosition(meetingPoint.transform.position);
        unit.teamColor = teamColorEnum;
    }

    public void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }

    public void SetPositionToOccupy(List<Vector3Int> positionToOccupy) => this.positionToOccupy = positionToOccupy;

    public List<ObjectPrices> AddResourcesToStockPile(List<ObjectPrices> objectPrices)
    {
        var playerResource = AccessToClassByTeamColor.instance.GetPlayerResourcesManagerByTeamColor(teamColor);
        if (stockPileType == ResourceTypesEnum.allTypes)
        {
            playerResource.AddResources(objectPrices);
            foreach (var obj in objectPrices)
            {
                obj.SetValue(0);
            }
        }
        return objectPrices;
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
        return buildingType;
    }
    public T GetProperties<T>() where T : Component
    {
        throw new NotImplementedException();
    }
}
