using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class Building : NetworkBehaviour, IActiveClickable, IStockPile, IGetTeamAndProperties
{
    [SerializeField] private BuildingSetProperstiesByTeamColor setProperstiesByTeamColor;
    [SerializeField] public List<Vector3Int> positionToOccupy;

    [SyncVar(hook = nameof(OnTeamColorChanged))]
    public TeamColorEnum teamColor;

    [SerializeField] private EntityTypeEnum entityType;
    [SerializeField] private BuildingTypeEnum buildingType;
    [SerializeField] private List<UnitNameEnum> unitsToBuy;
    [SerializeField] public Transform meetingPoint;

    [Header("OPTIONAL STOCKPILE")]
    [SerializeField] private ResourceTypesEnum stockPileType;
    [field: SerializeField] public bool isStockPile { get; private set; }
    public Transform stockPilePosition => transform;

    private void Start()
    {
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
    internal void RequestToServerToUpdateMeetingPoint(Vector3 newMeetingPointPosition)
    {
        if (PlayerController.LocalPlayer.isLocalPlayer)
            PlayerController.LocalPlayer.CmdUpdateMeetingPointBuilding(this.GetComponent<NetworkIdentity>(), newMeetingPointPosition);
    }
    public void ServerSpawnUnit(int unitID, TeamColorEnum teamColor)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        Unit unit = unitInstantiate.GetComponent<Unit>();
        unit.isGoingToMeetingPoint = true;
        unit.meetingPoint = meetingPoint.transform.position;
        Debug.Log("ORIGIN " + meetingPoint.transform.position);
        unit.teamColor = teamColor;
        NetworkServer.Spawn(unitInstantiate, connectionToClient);        
    }

    public void SetPositionToOccupy(List<Vector3Int> positionToOccupy) => this.positionToOccupy = positionToOccupy;

    public List<ObjectPrices> AddResourcesToStockPile(List<ObjectPrices> objectPrices)
    {
        if (stockPileType == ResourceTypesEnum.allTypes)
        {
            PlayerController.LocalPlayer.playerResources.AddResources(objectPrices);
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

    void OnTeamColorChanged(TeamColorEnum oldColor, TeamColorEnum newColor)
    {
        setProperstiesByTeamColor.Init();
    }
    #endregion
}
