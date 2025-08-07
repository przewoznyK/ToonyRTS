using Mirror;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Building : NetworkBehaviour, IActiveClickable, IStockPile, IGetTeamAndProperties
{
    [SerializeField] public List<Vector3Int> positionToOccupy;
    public TeamColorEnum teamColor;
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
            AccessToClassByTeamColor.Instance.AddStockPileToGlobalList(teamColor, this);

        EntityHealth entityHealth = GetComponent<EntityHealth>();
        entityHealth.onDeathActiom += () => RemoveEntity.Instance.RemoveEntityFromGame(this);
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

    public void SpawnUnit(int unitID, TeamColorEnum teamColor)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        RequestToServerSpawnUnitFromBuilding(unitPrefab, teamColor, meetingPoint.transform.position);
    }

    public void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }

    public void SetPositionToOccupy(List<Vector3Int> positionToOccupy) => this.positionToOccupy = positionToOccupy;

    public List<ObjectPrices> AddResourcesToStockPile(List<ObjectPrices> objectPrices)
    {
        var playerResource = AccessToClassByTeamColor.Instance.GetPlayerResourcesManagerByTeamColor(teamColor);
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
    #region interfaces
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
        if (typeof(T) == typeof(Transform))
            return transform as T;
        else
            Debug.Log("You can only take Transform from this");
        return null;
    }
    #endregion

    #region serverRequests
    public void RequestToServerSpawnUnitFromBuilding(GameObject unitPrefab, TeamColorEnum teamColor, Vector3 meetingPoint)
    {
        if (PlayerRoomController.LocalPlayer.isLocalPlayer)
            PlayerRoomController.LocalPlayer.CmdMSpawnUnit(this.GetComponent<NetworkIdentity>(), unitPrefab, teamColor, meetingPoint);
    }
    public void RespondFromServerSpawnUnit(GameObject unitPrefab, TeamColorEnum teamColor, Vector3 meetingPoint)
    {
        GameObject unitInstance = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        Unit unit = unitInstance.GetComponent<Unit>();
        unit.teamColor = teamColor;
        unit.isGoingToMeetingPoint = true;
        unit.meetingPoint = meetingPoint;
        NetworkServer.Spawn(unitInstance);
    }

    #endregion
}
