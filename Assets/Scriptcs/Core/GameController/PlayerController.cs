using Mirror;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalPlayer { get; private set; }
    public PlayerStockPileList stockPileManager = new PlayerStockPileList();
    public ControlledUnits controlledUnits = new ControlledUnits();
    public PlayerResources playerResources;
    [SyncVar] public TeamColorEnum teamColor;
    public int startPositionX;
    public int startPositionY;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        StartCoroutine(Delay());
        Debug.Log(stockPileManager == null);
    }
    public void CreatePlayerController(TeamColorEnum teamColor, int startPositionX, int startPositionY)
    {
        if (!isLocalPlayer) return;

        var teamProfile = new CreateTeamProfile(teamColor);
        this.teamColor = teamColor;
        this.startPositionX = startPositionX;
        this.startPositionY = startPositionY;
        InitClientSide();
    }

    IEnumerator Delay()
    {
        LocalPlayer = this;
        yield return new WaitForSeconds(2f);

        CreatePlayerController(teamColor, startPositionX, startPositionY);
    }
    
    void InitClientSide()
    {
        GridData gridData = new GridData();
        var playerControlledBuildings = new PlayerControlledBuildings();

        // Shop Manager
        var shopManager = new ShopManager(GameManager.Instance.buildingProduction);
        playerResources = new PlayerResources(GameManager.Instance.summaryPanelUI, GameManager.Instance.commandPanelUI, 3000, 3000, 2000, 1000);

        // Init 
        GameManager.Instance.removeEntity.Init(gridData);
        GameManager.Instance.manageSelectionUnits.Init(GameManager.Instance.inputManager, controlledUnits);
        GameManager.Instance.commandPanelUI.Init(playerResources, shopManager, GameManager.Instance.buildingProduction, GameManager.Instance.inputManager, GameManager.Instance.previewSystem);
        GameManager.Instance.selectionInfoUI.Init(controlledUnits);
        GameManager.Instance.activeClickableObject.Init(GameManager.Instance.inputManager, controlledUnits, GameManager.Instance.selectionInfoUI, GameManager.Instance.commandPanelUI,
        GameManager.Instance.boxVisual, teamColor);
        GameManager.Instance.buildingProduction.Init(GameManager.Instance.commandPanelUI, teamColor);
        GameManager.Instance.previewSystem.Init(playerResources, GameManager.Instance.inputManager, GameManager.Instance.constructionPlacerSystem, gridData, GameManager.Instance.activeClickableObject);
        GameManager.Instance.constructionPlacerSystem.Init(playerResources, GameManager.Instance.activeClickableObject);


        GameManager.Instance.playerStartGameSetup.Init(playerResources, GameManager.Instance.constructionPlacerSystem, gridData, teamColor, startPositionX, startPositionX);

    }

    [Command]
    public void CmdSpawnBuilding(Vector3 position, TeamColorEnum teamColor)
    {
        GameObject prefab = BuildingDatabase.Instance.GetBuildingDataByID(0).buildingPrefab;
        GameObject building = Instantiate(prefab, position, Quaternion.identity);
        building.GetComponent<Building>().teamColor = teamColor;
        NetworkServer.Spawn(building);
    }
    [Command]
    public void CmdMoveUnit(NetworkIdentity requestIdentity, Vector3 targetPos)
    {
        if (requestIdentity != null && requestIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
        {
            taskManager.RespondFromServerMoveUnit(targetPos);
        }
    }

    [Command]
    public void CmdAttackEntity(NetworkIdentity requestIdentity, GameObject entityObject)
    {
        if (requestIdentity != null && requestIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
        {
            taskManager.RespondFromServerToAttackEntity(entityObject);
        }
    }

    [Command]
    public void CmdSpawnUnit(NetworkIdentity buildingId, int unitID, TeamColorEnum teamColor)
    {
        var building = buildingId.GetComponent<Building>();
        if (building != null)
        {
            building.ServerSpawnUnit(unitID, teamColor);
        }
    }



}
