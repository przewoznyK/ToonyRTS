using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void CreatePlayerController(TeamColorEnum teamColor, AccessToClassByTeamColor accessToClassByTeamColor, GridData gridData)
    {
        var teamProfile = new CreateTeamProfile(teamColor);

        var controlledUnits = new ControlledUnits();
        var playerControlledBuildings = new PlayerControlledBuildings();

        // Input Manager
        GameObject inputManagerInstatiate = Instantiate(GameManager.Instance.InputManagerPrefab);
        var inputManager = inputManagerInstatiate.GetComponent<InputManager>();

        // Active Clickable Object
        GameObject activeClickableObjectInstatiate = Instantiate(GameManager.Instance.ActiveClickableObjectPrefab);
        var activeClickableObject = activeClickableObjectInstatiate.GetComponent<ActiveClickableObject>();

        // Manage Selection Units
        GameObject manageSelectionUnitsInstatiate = Instantiate(GameManager.Instance.ManageSelectionUnitsPrefab);
        var manageSelectionUnits = manageSelectionUnitsInstatiate.GetComponent<ManageSelectionUnits>();
        // Building Production
        GameObject buildingProductionInstatiate = Instantiate(GameManager.Instance.BuildingProducionPrefab);
        var buildingProduction = buildingProductionInstatiate.GetComponent<BuildingProduction>();
        // Construction System
        GameObject constructionSystemInstatiate = Instantiate(GameManager.Instance.ConstructionSystemPrefab);
        var previewSystemTransform = constructionSystemInstatiate.transform.GetChild(0);
        var previewSystem = previewSystemTransform.GetComponent<ConstructionPreviewSystem>();
        var construcionPlacerSystemTransform = constructionSystemInstatiate.transform.GetChild(1);
        var constructionPlacerSystem = construcionPlacerSystemTransform.GetComponent<ConstructionPlacerSystem>();

        // PLAYER UI
        GameObject playerUIPrefabInstantiate = Instantiate(GameManager.Instance.PlayerUIPrefab);
        var playerUIChild = playerUIPrefabInstantiate.transform.GetChild(0);
        var selectionInfoUI = playerUIChild.GetComponent<SelectionInfoUI>();

        playerUIChild = playerUIPrefabInstantiate.transform.GetChild(1);
        var commandPanelUI = playerUIChild.GetComponent<CommandPanelUI>();

        playerUIChild = playerUIPrefabInstantiate.transform.GetChild(2);
        var summaryPanelUI = playerUIChild.GetComponent<SummaryPanelUI>();

        // Hold Selection
        var holdSelectionUnitCanbasInstantiate = Instantiate(GameManager.Instance.HoldSelectionUnitCanvas);
        var holdSelectionUnitCanbasChild = holdSelectionUnitCanbasInstantiate.transform.GetChild(0);
        var boxVisual = holdSelectionUnitCanbasChild.GetComponent<RectTransform>();

        // Shop Manager
        var shopManager = new ShopManager(buildingProduction);
        var playerResources = new PlayerResources(summaryPanelUI, commandPanelUI, 3000, 3000, 2000, 1000);

        // Init 
        manageSelectionUnits.Init(inputManager, controlledUnits);
        commandPanelUI.Init(playerResources, shopManager, buildingProduction, inputManager, previewSystem);
        selectionInfoUI.Init(controlledUnits);
        activeClickableObject.Init(inputManager, controlledUnits, selectionInfoUI, commandPanelUI,
        boxVisual);
        buildingProduction.Init(commandPanelUI, TeamColorEnum.Blue);
        previewSystem.Init(playerResources, inputManager, constructionPlacerSystem, gridData, activeClickableObject);
        constructionPlacerSystem.Init(playerResources, activeClickableObject);

        // Start Setup
        var playerStartGameSetupInstantiate = Instantiate(GameManager.Instance.PlayerStartGameSetupPrefab);
        var playerStartGameSetup = playerStartGameSetupInstantiate.GetComponent<PlayerStartGameSetup>();
    //    playerStartGameSetup.Init(playerResources, constructionPlacerSystem, gridData, TeamColorEnum.Blue, 0, 0);

        accessToClassByTeamColor.AddPlayerResourceManagerToGlobalList(teamColor, playerResources);
        accessToClassByTeamColor.AddControlledUnitsManagerToGlobalList(teamColor, controlledUnits);
        accessToClassByTeamColor.AddControlledBuildingsManagerToGlobalList(teamColor, playerControlledBuildings);


    }
}
