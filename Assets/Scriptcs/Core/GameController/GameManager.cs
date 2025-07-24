using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject inputManagerPrefab;
    [SerializeField] private GameObject activeClickableObjectPrefab;
    [SerializeField] private GameObject manageSelectionUnitsPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject holdSelectionUnitCanvas;
    [SerializeField] private GameObject buildingProducionPrefab;
    [SerializeField] private GameObject constructionSystemPrefab;
    [SerializeField] private GameObject playerStartGameSetupPrefab;
    [SerializeField] private GameObject accessToClassByTeamColorPrefab;
    [SerializeField] private GameObject removeEntityPrefab;
    private void Awake()
    {
        //  var blueTeam = new PlayerController(TeamColorEnum.Blue);
       // var blueTeam = TeamColorEnum.Blue;
        var controlledUnits = new ControlledUnits();

        var gridData = new GridData();
        
       

        // Input Manager
        GameObject inputManagerInstatiate = Instantiate(inputManagerPrefab);
        var inputManager = inputManagerInstatiate.GetComponent<InputManager>();

        // Active Clickable Object
        GameObject activeClickableObjectInstatiate = Instantiate(activeClickableObjectPrefab);
        var activeClickableObject = activeClickableObjectInstatiate.GetComponent<ActiveClickableObject>();
        
        // Manage Selection Units
        GameObject manageSelectionUnitsInstatiate = Instantiate(manageSelectionUnitsPrefab);
        var manageSelectionUnits = manageSelectionUnitsInstatiate.GetComponent<ManageSelectionUnits>();
        // Building Production
        GameObject buildingProductionInstatiate = Instantiate(buildingProducionPrefab);
        var buildingProduction = buildingProductionInstatiate.GetComponent<BuildingProduction>();
        // Construction System
        GameObject constructionSystemInstatiate = Instantiate(constructionSystemPrefab);
        var previewSystemTransform = constructionSystemInstatiate.transform.GetChild(0);
        var previewSystem = previewSystemTransform.GetComponent<ConstructionPreviewSystem>();
        var construcionPlacerSystemTransform = constructionSystemInstatiate.transform.GetChild(1);
        var constructionPlacerSystem = construcionPlacerSystemTransform.GetComponent<ConstructionPlacerSystem>();

        // PLAYER UI
        GameObject playerUIPrefabInstantiate = Instantiate(playerUIPrefab);
        var playerUIChild = playerUIPrefabInstantiate.transform.GetChild(0);
        var selectionInfoUI = playerUIChild.GetComponent<SelectionInfoUI>();
        
        playerUIChild = playerUIPrefabInstantiate.transform.GetChild(1);
        var commandPanelUI = playerUIChild.GetComponent<CommandPanelUI>();

        playerUIChild = playerUIPrefabInstantiate.transform.GetChild(2);
        var summaryPanelUI = playerUIChild.GetComponent<SummaryPanelUI>();

        // Hold Selection
        var holdSelectionUnitCanbasInstantiate = Instantiate(holdSelectionUnitCanvas);
        var holdSelectionUnitCanbasChild = holdSelectionUnitCanbasInstantiate.transform.GetChild(0);
        var boxVisual = holdSelectionUnitCanbasChild.GetComponent<RectTransform>();

        // Shop Manager
        var shopManager = new ShopManager(buildingProduction);
        // Player Remove Entity
        var removeEntityInstantiate = Instantiate(removeEntityPrefab);
        var removeEntity = removeEntityInstantiate.GetComponent<RemoveEntity>();

        // Init 
        manageSelectionUnits.Init(inputManager, controlledUnits);
        removeEntity.Init(gridData);
        commandPanelUI.Init(playerResources, shopManager, buildingProduction, inputManager, previewSystem, removeEntity);
        selectionInfoUI.Init(controlledUnits);
        activeClickableObject.Init(inputManager, controlledUnits, selectionInfoUI, commandPanelUI,
        boxVisual);
        buildingProduction.Init(commandPanelUI, TeamColorEnum.Blue);
        previewSystem.Init(inputManager, constructionPlacerSystem, gridData, activeClickableObject);
        constructionPlacerSystem.Init(playerResources, activeClickableObject);

        // Start Setup
        var playerStartGameSetupInstantiate = Instantiate(playerStartGameSetupPrefab);
        var playerStartGameSetup = playerStartGameSetupInstantiate.GetComponent<PlayerStartGameSetup>();
        playerStartGameSetup.Init(playerResources, constructionPlacerSystem, gridData, TeamColorEnum.Blue, 0, 0);



      


        // Global
        var accessToClassByTeamColorInstantiate = Instantiate(accessToClassByTeamColorPrefab);
        var accessToClassByTeamColor = accessToClassByTeamColorInstantiate.GetComponent<AccessToClassByTeamColor>();

        var blueTeam = new PlayerController(TeamColorEnum.Blue, accessToClassByTeamColor);




        var redTeam = new PlayerController(TeamColorEnum.Red, accessToClassByTeamColor);
        playerStartGameSetup.Init(playerResources, constructionPlacerSystem, gridData, TeamColorEnum.Red, 10, 10);
    }
}
