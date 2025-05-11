using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject inputManagerPrefab;
    [SerializeField] private GameObject activeClickableObjectPrefab;
    [SerializeField] private GameObject manageSelectionUnitsPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject holdSelectionUnitCanvas;
    private InputManager inputManager;
    private ActiveClickableObject activeClickableObject;
    private ManageSelectionUnits manageSelectionUnits;
    // Player UI
    private SelectionInfoUI selectionInfoUI;

    
    private void Awake()
    {
        var blueTeam = new PlayerController(TeamColorEnum.Blue);
        var controlledUnits = new ControlledUnits();
        var activeUnits = new ActiveUnits();
        var playerResources = new PlayerResources(50,30,20,10);
        // Input Manager
        GameObject inputManagerInstatiate = Instantiate(inputManagerPrefab);
        inputManager = inputManagerInstatiate.GetComponent<InputManager>();


        // Active Clickable Object
        GameObject activeClickableObjectInstatiate = Instantiate(activeClickableObjectPrefab);
        activeClickableObject = activeClickableObjectInstatiate.GetComponent<ActiveClickableObject>();
        
        // Manage Selection Units
        GameObject manageSelectionUnitsInstatiate = Instantiate(manageSelectionUnitsPrefab);
        manageSelectionUnits = manageSelectionUnitsInstatiate.GetComponent<ManageSelectionUnits>();
        manageSelectionUnits.Init(inputManager, activeUnits);

        // PLAYER UI
        GameObject playerUIPrefabInstantiate = Instantiate(playerUIPrefab);
        var playerUIChild = playerUIPrefabInstantiate.transform.GetChild(0);
        selectionInfoUI = playerUIChild.GetComponent<SelectionInfoUI>();
        selectionInfoUI.Init(activeUnits);
        
        playerUIChild = playerUIPrefabInstantiate.transform.GetChild(1);
        var commandPanelUI = playerUIChild.GetComponent<CommandPanelUI>();
        commandPanelUI.Init(playerResources);
        playerUIChild = playerUIPrefabInstantiate.transform.GetChild(2);
        var summaryPanelUI = playerUIChild.GetComponent<SummaryPanelUI>();
        summaryPanelUI.Init(playerResources);

        var holdSelectionUnitCanbasInstantiate = Instantiate(holdSelectionUnitCanvas);
        var holdSelectionUnitCanbasChild = holdSelectionUnitCanbasInstantiate.transform.GetChild(0);
        var boxVisual = holdSelectionUnitCanbasChild.GetComponent<RectTransform>();

        activeClickableObject.Init(inputManager, controlledUnits, activeUnits, selectionInfoUI, commandPanelUI,
            boxVisual);
    }
}
