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
    
    private void Awake()
    {
        var blueTeam = new PlayerController(TeamColorEnum.Blue);
        var controlledUnits = new ControlledUnits();
        var activeUnits = new ActiveUnits();
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
        // Player Resources
        var playerResources = new PlayerResources(summaryPanelUI, commandPanelUI ,300, 30, 20, 10);
        // Shop Manager
        var shopManager = new ShopManager(playerResources, buildingProduction);


        // Init 
        manageSelectionUnits.Init(inputManager, activeUnits);
        commandPanelUI.Init(playerResources, shopManager, buildingProduction, inputManager, previewSystem);
        selectionInfoUI.Init(activeUnits);
        activeClickableObject.Init(inputManager, controlledUnits, activeUnits, selectionInfoUI, commandPanelUI,
        boxVisual);
        buildingProduction.Init(commandPanelUI);
        previewSystem.Init(inputManager, constructionPlacerSystem, gridData);

        // Start Setup
        var playerStartGameSetupInstantiate = Instantiate(playerStartGameSetupPrefab);
        var playerStartGameSetup = GetComponent<PlayerStartGameSetup>();
        playerStartGameSetup.Init(playerResources, constructionPlacerSystem);
    }
}
