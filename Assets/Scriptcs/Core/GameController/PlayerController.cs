using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalPlayer { get; private set; }
    public PlayerStockPileList stockPileManager = new PlayerStockPileList();
    public ControlledUnits controlledUnits = new ControlledUnits();
    public PlayerResources playerResources;
    [SyncVar] public TeamColorEnum teamColor;
    [SyncVar] public int startPositionX;
    [SyncVar] public int startPositionZ;
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
        GameManager.Instance.previewSystem.Init(playerResources, GameManager.Instance.inputManager, GameManager.Instance.constructionPlacerSystem, GameManager.Instance.gridDataNetwork, GameManager.Instance.activeClickableObject);
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

    // Attack Entity 
    [Command]
    public void CmdAttackEntity(NetworkIdentity requestIdentity, Transform targetTransform)
    {
        if (requestIdentity != null && requestIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            RpcAttackEntity(requestIdentity, targetTransform);
    }
    [ClientRpc]
    void RpcAttackEntity(NetworkIdentity requestIdentity, Transform targetTransform)
    {
        if (requestIdentity != null && requestIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
            taskManager.RespondFromServerToAttackEntity(targetTransform);
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
    public void CmdSpawnBuilding(Vector3 position, TeamColorEnum teamColor)
    {
        GameObject prefab = BuildingDatabase.Instance.GetBuildingDataByID(0).buildingPrefab;
        GameObject building = Instantiate(prefab, position, Quaternion.identity);
        building.GetComponent<Building>().teamColor = teamColor;
        NetworkServer.Spawn(building);
    }


    #endregion
}
