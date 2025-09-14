using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalPlayer { get; private set; }
    public PlayerStockPileList stockPileManager = new PlayerStockPileList();
    public ControlledUnits controlledUnits = new ControlledUnits();
    public PlayerResources playerResources;
    public CommandPanelUI commandPanelUI;
    [SyncVar] public TeamColorEnum teamColor;
    [SyncVar] public int startPositionX;
    [SyncVar] public int startPositionZ;
    public GameObject contructionRepresentationPrefab;

    public bool aggressiveApproachCommand;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        StartCoroutine(Delay());
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



    private InputAction RMBClickAction;
    private InputAction LMBClickAction;
    private InputAction ShiftClickAction;

    [SerializeField] private LayerMask ignoreLayerMask;
    private bool initialized;

    internal void Init(InputManager inputManager, ControlledUnits controlledUnits)
    {

    }
    private void OnEnable()
    {
        if (initialized)
        {
            RMBClickAction.performed += OnRMBClick;
            LMBClickAction.performed += OnLMBClick;
        }
    }

    private void OnDisable()
    {
        RMBClickAction.performed -= OnRMBClick;
        LMBClickAction.performed -= OnLMBClick;
    }
    void InitClientSide()
    {
        var playerControlledBuildings = new PlayerControlledBuildings();

        commandPanelUI = GameManager.Instance.commandPanelUI;
        // Shop Manager
        var shopManager = new ShopManager(GameManager.Instance.buildingProduction);
        playerResources = new PlayerResources(GameManager.Instance.summaryPanelUI, GameManager.Instance.commandPanelUI, 3000, 3000, 2000, 1000);

        // Init 
        GameManager.Instance.removeEntity.Init(GameManager.Instance.gridDataNetwork);
     //   GameManager.Instance.manageSelectionUnits.Init(GameManager.Instance.inputManager, controlledUnits);
        GameManager.Instance.commandPanelUI.Init(playerResources, shopManager, GameManager.Instance.buildingProduction, GameManager.Instance.inputManager, GameManager.Instance.previewSystem);
        GameManager.Instance.selectionInfoUI.Init(controlledUnits);
        GameManager.Instance.activeClickableObject.Init(GameManager.Instance.inputManager, controlledUnits, GameManager.Instance.selectionInfoUI, GameManager.Instance.commandPanelUI,
        GameManager.Instance.boxVisual, GameManager.Instance.taskVisualization, teamColor);
        GameManager.Instance.buildingProduction.Init(GameManager.Instance.commandPanelUI, teamColor);
        GameManager.Instance.previewSystem.Init(playerResources, GameManager.Instance.inputManager, GameManager.Instance.constructionPlacerSystem, GameManager.Instance.gridDataNetwork, GameManager.Instance.activeClickableObject, teamColor);
        GameManager.Instance.constructionPlacerSystem.Init(playerResources, GameManager.Instance.activeClickableObject);
        GameManager.Instance.commandShortcutKeyManager.Init(GameManager.Instance.inputManager, GameManager.Instance.commandPanelUI);

        GameManager.Instance.playerStartGameSetup.Init(playerResources, GameManager.Instance.constructionPlacerSystem, GameManager.Instance.gridDataNetwork, teamColor, startPositionX, startPositionZ);
        GameManager.Instance.taskVisualization.Init(controlledUnits);

        RMBClickAction = GameManager.Instance.inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];
        LMBClickAction = GameManager.Instance.inputManager.Inputs.actions[InputManager.INPUT_GAME_LMB_Click];
        ShiftClickAction = GameManager.Instance.inputManager.Inputs.actions[InputManager.INPUT_GAME_SHIFT];

        RMBClickAction.performed += OnRMBClick;
        LMBClickAction.performed += OnLMBClick;

        initialized = true;
    }


    #region Unit Server 


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
        NetworkServer.Spawn(unitInstantiate);
    }
    [Command]
    public void CmdSpawnUnitFromBuilding(NetworkIdentity buildingId, int unitID, TeamColorEnum teamColor)
    {
        var building = buildingId.GetComponent<Building>();
        if (building != null)
            building.ServerSpawnUnit(unitID, teamColor);
    }
    [Command]
    public void CmdSpawnBuilding(int buildingId, Vector3Int position, TeamColorEnum teamColor, List<Vector3Int> positionToOccupy, Vector2 buildingSize)
    {
        GameObject prefab = BuildingDatabase.Instance.GetBuildingDataByID(buildingId).buildingPrefab;
        GameObject buildingInstantiate = Instantiate(prefab, position, Quaternion.identity);
        Building building = buildingInstantiate.GetComponent<Building>();
        building.teamColor = teamColor;
        building.buildingSize = buildingSize;

        NetworkServer.Spawn(buildingInstantiate);
        RpcSetActiveGameObject(buildingInstantiate.gameObject, true);
        RpcSetPositionsOccupyToBuilding(building, positionToOccupy);
    }

    [Command]
    public void CmdSpawnConstructionRepresentation(int buildingId, Vector3 position, Vector2 size, List<Unit> selectedUnits, TeamColorEnum teamColor, List<Vector3Int> positionToOccupy, Vector2 buildingSize)
    {
        GameObject contructionRepresentationInstantiate = Instantiate(contructionRepresentationPrefab, position, Quaternion.identity);
        contructionRepresentationInstantiate.transform.rotation = Quaternion.Euler(90, 0, 0);
        contructionRepresentationInstantiate.GetComponent<InConstructionBuildingRepresentation>().buildingSize = size;
        NetworkServer.Spawn(contructionRepresentationInstantiate);

        GameObject prefab = BuildingDatabase.Instance.GetBuildingDataByID(buildingId).buildingPrefab;
        GameObject buildingInstantiate = Instantiate(prefab, position, Quaternion.identity);
        Building building = buildingInstantiate.GetComponent<Building>();
        building.teamColor = teamColor;
        building.buildingSize = buildingSize;

        NetworkServer.Spawn(buildingInstantiate);


        RpcSetFinishBuilding(contructionRepresentationInstantiate.GetComponent<InConstructionBuildingRepresentation>(), buildingInstantiate);
        RpcSetActiveGameObject(buildingInstantiate.gameObject, false);
        RpcSetPositionsOccupyToBuilding(building, positionToOccupy);
        foreach (var unit in selectedUnits)
        {
            if (unit is GathererNew)
            {
                GathererNew gatherer = unit as GathererNew;
                gatherer.unitTaskManager.RespondFromServerToBuildConstructionTask(contructionRepresentationInstantiate);
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
    public void CmdTakeResource(NetworkIdentity networkIdentity)
    {

        if (networkIdentity != null && networkIdentity.TryGetComponent<GatherableResource>(out var gatherableResource))
        {
            if (gatherableResource != null)
                gatherableResource.available--;
        }

    }

    #endregion




    [Command]
    void MoveUnitsInFormation(List<Unit> units, Vector3 target, float spacing)
    {
        int rowLength = Mathf.CeilToInt(Mathf.Sqrt(units.Count));
        int index = 0;

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < rowLength; j++)
            {
                if (index >= units.Count) return;

                Vector3 offset = new Vector3(i * spacing, 0, j * spacing);
                units[index].agent.SetDestination(target + offset);
                index++;
            }
        }
    }


    [Command]
    public void MoveUnitCommand(NetworkIdentity networkIdentity, Vector3 point, bool isShiftPressed)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
        {
            if (isShiftPressed == false)
                taskManager.RespondFromServerToResetTasks();

            taskManager.RespondFromServerToCreateGoToPositionTask(point);
        }
    }

    [Command]
    public void AttackEntityCommand(NetworkIdentity networkIdentity, TeamColorEnum enemyTeamColor, Transform enemyTransform, bool isShiftPressed)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
        {
            if (isShiftPressed == false)
                taskManager.RespondFromServerToResetTasks();

            taskManager.RespondFromServerToCreateAttackEntityTask(enemyTeamColor, enemyTransform);
        }
    }
    [Command]
    public void AggressiveAproachCommand(NetworkIdentity networkIdentity, Vector3 position, bool isShiftPressed)
    {
        if (networkIdentity != null && networkIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
        {
            if (isShiftPressed == false)
                taskManager.RespondFromServerToResetTasks();

            taskManager.RespondFromServerToCreateAggressiveApproachTask(position);
        }
    }

    [Command]
    private void GatherResourceCommand(NetworkIdentity networkIdentity, Vector3 point, GatherableResource gatherableResource, bool isShiftPressed)
    {
        Debug.Log("GATHEERER RESOURCE COMMAND");
        if (networkIdentity != null && networkIdentity.TryGetComponent<GathererTaskManager>(out var gathererTaskManager))
        {
            if (isShiftPressed == false)
                gathererTaskManager.RespondFromServerToResetTasks();

            gathererTaskManager.GatherResourceTask(gatherableResource);
        }
    }

    [Command]
    public void CmdSpawnResource(int prefabId, Vector3 position)
    {
            
            GameObject obj = Instantiate(DefaultMapGenerator.Instance.objectsToSpawn[prefabId], position, Quaternion.identity);
            NetworkServer.Spawn(obj);
    }

    private void Update()
    {
        if (controlledUnits.selectedUnits.Count <= 0) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreLayerMask))
        {
            if (hit.collider.gameObject.TryGetComponent<IHighlight>(out IHighlight IHighlight))
            {
                if (hit.collider.gameObject.CompareTag("Resource"))
                {
                    if (controlledUnits.selectedUnits[0].unitTaskManager.CanReachDestination(hit.collider.gameObject.transform.position))
                    {

                        IHighlight.HightLight();
                    }
                }
       
            }
        }

    }

    public void OnLMBClick(InputAction.CallbackContext ctx)
    {
        if (InputManager.Instance.isMouseOverGameObject || aggressiveApproachCommand == false)
            return;

        bool isShiftPressed = ShiftClickAction.ReadValue<float>() > 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreLayerMask))
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Ground"))
            {
                foreach (var unit in controlledUnits.selectedUnits)
                {
                    AggressiveAproachCommand(unit.netIdentity, hit.point, isShiftPressed);
                }
                commandPanelUI.ResetAggresiveApproachButton();
            }
        }
    }

    public void OnRMBClick(InputAction.CallbackContext ctx)
    {
        if (InputManager.Instance.isMouseOverGameObject)
            return;

        bool isShiftPressed = ShiftClickAction.ReadValue<float>() > 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~ignoreLayerMask))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                float spacing = 1f;
                int unitsPerRow = Mathf.CeilToInt(Mathf.Sqrt(controlledUnits.selectedUnits.Count));
                int index = 0;

                foreach (var unit in controlledUnits.selectedUnits)
                {
                    int row = index / unitsPerRow;
                    int col = index % unitsPerRow;

                    Vector3 offset = new Vector3(col * spacing, 0, row * spacing);
                    MoveUnitCommand(unit.netIdentity, hit.point + offset, isShiftPressed);

                    index++;
                }
            }


            else if (hit.collider.TryGetComponent<IGetTeamAndProperties>(out IGetTeamAndProperties component))
            {
                if ((component.GetTeam() & teamColor) != 0)
                {
                    if (component.GetBuildingType() == BuildingTypeEnum.resource)
                    {
                        foreach (var unit in controlledUnits.selectedUnits)
                        {
                            if (unit is GathererNew)
                                GatherResourceCommand(unit.netIdentity, hit.point, component.GetProperties<GatherableResource>(), isShiftPressed);
                        }
                        return;
                    }
                }
                else if (component.GetTeam() != teamColor)
                {
                    
                    foreach (var unit in controlledUnits.selectedUnits)
                        if (teamColor != component.GetTeam())
                            AttackEntityCommand(unit.netIdentity, component.GetTeam(), component.GetProperties<Transform>(), isShiftPressed);
                }
            }
        }
    }
}
