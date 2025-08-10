using Mirror;
using System.Collections;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    GridData gridData;
    int startPositionX;
    int startPositionY;
    [SyncVar] public TeamColorEnum teamColor;
    public void CreatePlayerController(TeamColorEnum teamColor, GridData gridData, int startPositionX, int startPositionY)
    {
        var teamProfile = new CreateTeamProfile(teamColor);
        Debug.Log("USTAWIAM TEAM COLOR" + teamColor);
        this.teamColor = teamColor;
        this.gridData = gridData;
        this.startPositionX = startPositionX;
        this.startPositionY = startPositionY;
        RpcInitClient();
    }

    [ClientRpc]
    public void RpcInitClient()
    {
        if (!isLocalPlayer) return; // Upewnij siê, ¿e tylko lokalny gracz robi setup
        Debug.Log($"[CLIENT RPC] Ustawiam gracza {teamColor}");
        InitClientSide();
    }

    void InitClientSide()
    {


        var controlledUnits = new ControlledUnits();
        var playerControlledBuildings = new PlayerControlledBuildings();

        // Shop Manager
        var shopManager = new ShopManager(GameManager.Instance.buildingProduction);
        var playerResources = new PlayerResources(GameManager.Instance.summaryPanelUI, GameManager.Instance.commandPanelUI, 3000, 3000, 2000, 1000);

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

        // Start Setup

        GameManager.Instance.accessToClassByTeamColor.AddPlayerResourceManagerToGlobalList(teamColor, playerResources);
        GameManager.Instance.accessToClassByTeamColor.AddControlledUnitsManagerToGlobalList(teamColor, controlledUnits);
        GameManager.Instance.accessToClassByTeamColor.AddControlledBuildingsManagerToGlobalList(teamColor, playerControlledBuildings);

        GameManager.Instance.playerStartGameSetup.Init(playerResources, GameManager.Instance.constructionPlacerSystem, gridData, teamColor, startPositionX, startPositionX);

    }
}
