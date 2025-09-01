using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalPlayer { get; private set; }
    public PlayerStockPileList stockPileManager = new PlayerStockPileList();
    public ControlledUnits controlledUnits = new ControlledUnits();
    public PlayerResources playerResources;
    [SyncVar] public TeamColorEnum teamColor;
    [SyncVar] public int startPositionX;
    [SyncVar] public int startPositionZ;
    public GameObject contructionRepresentationPrefab;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        StartCoroutine(Delay());
        Debug.Log(stockPileManager == null);
    }
    public void CreatePlayerController(TeamColorEnum teamColor, int startPositionX, int startPositionZ)
    {
        if (!isLocalPlayer) return;

        var teamProfile = new CreateTeamProfile(teamColor);
        this.teamColor = teamColor;
        this.startPositionX = startPositionX;
        this.startPositionZ = startPositionZ;
        InitClientSide();
    }

    IEnumerator Delay()
    {
        LocalPlayer = this;
        yield return new WaitForSeconds(2f);

        CreatePlayerController(teamColor, startPositionX, startPositionZ);
    }
    
    void InitClientSide()
    {
        var playerControlledBuildings = new PlayerControlledBuildings();

        // Shop Manager
        var shopManager = new ShopManager(GameManager.Instance.buildingProduction);
        playerResources = new PlayerResources(GameManager.Instance.summaryPanelUI, GameManager.Instance.commandPanelUI, 3000, 3000, 2000, 1000);

        // Init 
        GameManager.Instance.removeEntity.Init(GameManager.Instance.gridDataNetwork);
        GameManager.Instance.manageSelectionUnits.Init(GameManager.Instance.inputManager, controlledUnits);
        GameManager.Instance.commandPanelUI.Init(playerResources, shopManager, GameManager.Instance.buildingProduction, GameManager.Instance.inputManager, GameManager.Instance.previewSystem);
        GameManager.Instance.selectionInfoUI.Init(controlledUnits);
        GameManager.Instance.activeClickableObject.Init(GameManager.Instance.inputManager, controlledUnits, GameManager.Instance.selectionInfoUI, GameManager.Instance.commandPanelUI,
        GameManager.Instance.boxVisual, teamColor);
        GameManager.Instance.buildingProduction.Init(GameManager.Instance.commandPanelUI, teamColor);
        GameManager.Instance.previewSystem.Init(playerResources, GameManager.Instance.inputManager, GameManager.Instance.constructionPlacerSystem, GameManager.Instance.gridDataNetwork, GameManager.Instance.activeClickableObject, teamColor);
        GameManager.Instance.constructionPlacerSystem.Init(playerResources, GameManager.Instance.activeClickableObject);


        GameManager.Instance.playerStartGameSetup.Init(playerResources, GameManager.Instance.constructionPlacerSystem, GameManager.Instance.gridDataNetwork, teamColor, startPositionX, startPositionZ);

    }


    #region Unit Server 
    // Update Animator Speed
    [Command]
    internal void CmdChangeAnimatorSpeedUnit(NetworkIdentity networkIdentity, int speedValue)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcChangeAnimatorSpeedUnit(networkIdentity, speedValue);
    }
    [ClientRpc]
    void RpcChangeAnimatorSpeedUnit(NetworkIdentity networkIdentity, int speedValue)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerUpdateAnimatorSpeedValue(speedValue);
    }

    [Command]
    internal void CmdSetBoolAnimation(NetworkIdentity networkIdentity, string animationName, bool value)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcSetBoolAnimation(networkIdentity, animationName, value);
    }
    [ClientRpc]
    void RpcSetBoolAnimation(NetworkIdentity networkIdentity, string animationName, bool value)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToSetBoolAnimation(animationName, value);
    }

    // Move Unit
    [Command]
    public void CmdMoveUnit(NetworkIdentity networkIdentity, Vector3 targetPos)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcMoveUnit(networkIdentity, targetPos);
    }
    [ClientRpc]
    public void RpcMoveUnit(NetworkIdentity networkIdentity, Vector3 targetPosition)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToMoveUnit(targetPosition);
    }

    // Update Meeting Point
    [Command]
    public void CmdUpdateMeetingPointBuilding(NetworkIdentity networkIdentity, Vector3 newMeetingPosition)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<Building>(out var building))
            RpcUpdateMeetingPoint(networkIdentity, newMeetingPosition);
    }
    [ClientRpc]
    void RpcUpdateMeetingPoint(NetworkIdentity networkIdentity, Vector3 newMeetingPosition)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<Building>(out var building))
            building.meetingPoint.transform.position = newMeetingPosition;
    }

    // Reset Tasks
    [Command]
    public void CmdResetTasksUnit(NetworkIdentity networkIdentity)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcResetTaskUnit(networkIdentity);
    }
    [ClientRpc]
    internal void RpcResetTaskUnit(NetworkIdentity networkIdentity)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToResetTasks();
    }
    // Go To Position Task
    [Command]
    public void CmdCreateGoToPositionTask(NetworkIdentity networkIdentity, Vector3 positionPoint)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcCreateGoToPositionTask(networkIdentity, positionPoint);
    }
    [ClientRpc]
    internal void RpcCreateGoToPositionTask(NetworkIdentity networkIdentity, Vector3 positionPoint)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToCreateGoToPositionTask(positionPoint);
    }

    // Attack Entity Task
    [Command]
    internal void CmdCreateAttackEntityTask(NetworkIdentity networkIdentity, TeamColorEnum targetTeam, Transform targetEntity)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcCreateAttackEntityTask(networkIdentity, targetTeam, targetEntity);
    }

    [ClientRpc]
    internal void RpcCreateAttackEntityTask(NetworkIdentity networkIdentity, TeamColorEnum targetTeam, Transform targetEntity)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToCreateAttackEntityTask(targetTeam, targetEntity);
    }

    [Command]
    public void CmdCreateAggressiveApproachTask(NetworkIdentity networkIdentity, Vector3 positionPoint)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcCreateAggressiveApproachTask(networkIdentity, positionPoint);
    }
    [ClientRpc]
    internal void RpcCreateAggressiveApproachTask(NetworkIdentity networkIdentity, Vector3 positionPoint)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToCreateAggressiveApproachTask(positionPoint);
    }

    // Attack Entity 
    [Command]
    public void CmdAttackEntity(NetworkIdentity networkIdentity, Transform targetTransform)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcAttackEntity(networkIdentity, targetTransform);
    }
    [ClientRpc]
    void RpcAttackEntity(NetworkIdentity networkIdentity, Transform targetTransform)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToAttackEntity(targetTransform);
    }

    // Build Construction
    [Command]
    internal void CmdBuildConstructionTask(NetworkIdentity networkIdentity, GameObject construction)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GathererTaskManager>(out var taskManager))
            RpcBuildConstructionTask(networkIdentity, construction);
    }
    [ClientRpc]
    void RpcBuildConstructionTask(NetworkIdentity networkIdentity, GameObject construction)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GathererTaskManager>(out var gathererTaskManager))
            gathererTaskManager.RespondFromServerToBuildConstructionTask(construction);
    }

    #endregion

    #region Spawn Objects

    [Command]
    public void CmdSpawnUnitOnStart(int unitID, TeamColorEnum teamColor, int xPosition, int zPosition)
    {
        GameObject unitPrefab = UnitDatabase.Instance.GetUnitDataByID(unitID).unitPrefab;
        GameObject unitInstantiate = Instantiate(unitPrefab, transform.position, Quaternion.identity);
        Unit unit = unitInstantiate.GetComponent<Unit>();
        unit.transform.position = new Vector3(xPosition, 0, zPosition);
        unit.isGoingToMeetingPoint = false;
        unit.teamColor = teamColor;
        NetworkServer.Spawn(unitInstantiate, connectionToClient);
    }
    [Command]
    public void CmdSpawnUnitFromBuilding(NetworkIdentity buildingId, int unitID, TeamColorEnum teamColor)
    {
        var building = buildingId.GetComponent<Building>();
        if (building != null)
            building.ServerSpawnUnit(unitID, teamColor);
    }
    [Command]
    public void CmdSpawnBuilding(int buildingId, Vector3Int position, TeamColorEnum teamColor, List<Vector3Int> positionToOccupy)
    {
        GameObject prefab = BuildingDatabase.Instance.GetBuildingDataByID(buildingId).buildingPrefab;
        GameObject buildingInstantiate = Instantiate(prefab, position, Quaternion.identity);
        Building building = buildingInstantiate.GetComponent<Building>();
        building.teamColor = teamColor;

        NetworkServer.Spawn(buildingInstantiate);
        RpcSetActiveGameObject(buildingInstantiate.gameObject, true);
        RpcSetPositionsOccupyToBuilding(building, positionToOccupy);
    }

    [Command]
    public void CmdSpawnConstructionRepresentation(int buildingId, Vector3 position, Vector2 size, List<Unit> selectedUnits, TeamColorEnum teamColor, List<Vector3Int> positionToOccupy)
    {
        GameObject contructionRepresentationInstantiate = Instantiate(contructionRepresentationPrefab, position, Quaternion.identity);
        contructionRepresentationInstantiate.transform.rotation = Quaternion.Euler(90, 0, 0);
 
        NetworkServer.Spawn(contructionRepresentationInstantiate);

        GameObject prefab = BuildingDatabase.Instance.GetBuildingDataByID(buildingId).buildingPrefab;
        GameObject buildingInstantiate = Instantiate(prefab, position, Quaternion.identity);
        Building building = buildingInstantiate.GetComponent<Building>();
        building.teamColor = teamColor;

        NetworkServer.Spawn(buildingInstantiate);


        RpcSetFinishBuilding(contructionRepresentationInstantiate.GetComponent<InConstructionBuildingRepresentation>(), buildingInstantiate);
        RpcSetActiveGameObject(buildingInstantiate.gameObject, false);
        RpcSetPositionsOccupyToBuilding(building, positionToOccupy);
        foreach (var unit in selectedUnits)
        {
            if (unit is GathererNew)
            {
                GathererNew gatherer = unit as GathererNew;
                gatherer.BuildConstruction(contructionRepresentationInstantiate);
            }
        }

    }

    [ClientRpc]
    public void RpcSetFinishBuilding(InConstructionBuildingRepresentation inConstructionBuildingRepresentation, GameObject finishBuilding) =>
        inConstructionBuildingRepresentation.finishBuilding = finishBuilding;

    [Command]
    internal void CmdAddObjectToGridData(NetworkIdentity networkIdentity, PlacementData data, List<Vector3Int> positionToOccupy)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GridDataNetwork>(out var gridDataNetwork))
            RpcAddObjectToGridData(networkIdentity, data, positionToOccupy);
    }
    [ClientRpc]
    internal void RpcAddObjectToGridData(NetworkIdentity networkIdentity, PlacementData data, List<Vector3Int> positionToOccupy)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GridDataNetwork>(out var gridDataNetwork))
            gridDataNetwork.RespondFromServerToAddObjectToGridData(data, positionToOccupy);
    }

    [Command]
    internal void CmdRemoveObjectFromGridData(NetworkIdentity networkIdentity, List<Vector3Int> positionToRemove)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GridDataNetwork>(out var gridDataNetwork))
            RpcRemoveObjectFromoGridData(networkIdentity, positionToRemove);
    }
    [ClientRpc]
    internal void RpcRemoveObjectFromoGridData(NetworkIdentity networkIdentity, List<Vector3Int> positionToRemove)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GridDataNetwork>(out var gridDataNetwork))
            gridDataNetwork.RespondFromServerToRemoveObjectFromGridData(positionToRemove);
    }

    [Command]
    public void CmdContructionEndProcess(NetworkIdentity networkIdentity, GameObject building)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<GameObject>(out var gameObject))
             NetworkServer.Destroy(gameObject);
        RpcSetActiveGameObject(building, true);
    }
    [ClientRpc]
    public void RpcSetActiveGameObject(GameObject gameObject, bool value) => gameObject.SetActive(value);
    [ClientRpc]
    public void RpcSetPositionsOccupyToBuilding(Building building, List<Vector3Int> positionsToOccupy) => building.positionToOccupy = positionsToOccupy;
    [Command]
    public void CmdRemoveGameObject(GameObject gameObject) => NetworkServer.Destroy(gameObject);

    [Command]
    public void CmdTakeResource(NetworkIdentity resourceNetId)
    {
        var resource = resourceNetId.GetComponent<GatherableResource>();
        if (resource != null)
            resource.available--;
    }
    #endregion
}
