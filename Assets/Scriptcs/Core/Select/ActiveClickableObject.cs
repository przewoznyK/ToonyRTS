using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ActiveClickableObject : MonoBehaviour
{
    #region Constructor Variable
    InputManager inputManager;
    public ControlledUnits controlledUnits;
    SelectionInfoUI selectionInfoUI;
    CommandPanelUI commandPanelUI;
    #endregion
    #region Inputs
    private InputAction LMBClickAction;
    private InputAction LMBHoldAction;
    private InputAction LMBDoubleClickAction;
    private InputAction shiftAction;
    #endregion
    #region Flags
    private bool shiftPressed = false;
    private bool initialized = false;
    private bool lpmHolding = false;
    private bool pointerOverUI;
    #endregion
    #region SelectionBox
    RectTransform boxVisual;
    Rect selectionBox;
    Vector2 startPosition;
    Vector2 endPosition;
    #endregion
    [SerializeField] private LayerMask clickableLayer;
    public void Init(InputManager inputManager, ControlledUnits controlledUnits, SelectionInfoUI selectionInfoUI, CommandPanelUI commandPanelUI ,RectTransform boxVisual)
    {
        this.inputManager = inputManager;
        this.controlledUnits = controlledUnits;
        this.selectionInfoUI = selectionInfoUI;
        this.commandPanelUI = commandPanelUI;
        this.boxVisual = boxVisual;

        LMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_LMB_Click];
        LMBHoldAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_LMB_HOLD];
        LMBDoubleClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_LMB_Double_Click];
        shiftAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_SHIFT];
        

        LMBClickAction.performed += OnLpmClick;
        LMBHoldAction.started += OnLpmHold;
        LMBHoldAction.performed += OnLpmHold;
        LMBHoldAction.canceled += OnLpmHold;
        LMBDoubleClickAction.performed += OnLpmDoubleClick;

        initialized = true;
    }
    private void OnEnable()
    {
        if (initialized == false)
            return;
        LMBClickAction.performed += OnLpmClick;
        LMBHoldAction.started += OnLpmHold;
        LMBHoldAction.performed += OnLpmHold;
        LMBHoldAction.canceled += OnLpmHold;
        LMBDoubleClickAction.performed += OnLpmDoubleClick;
    }

    private void OnDisable()
    {
        LMBClickAction.performed -= OnLpmClick;
        LMBHoldAction.started -= OnLpmHold;
        LMBHoldAction.performed -= OnLpmHold;
        LMBHoldAction.canceled -= OnLpmHold;
        LMBDoubleClickAction.performed -= OnLpmDoubleClick;
    }
    private void OnLpmClick(InputAction.CallbackContext ctx)
    {
        // Active Clickable Object and CommandUI
        if(pointerOverUI == false)
        {
            commandPanelUI.gameObject.SetActive(false);
            selectionInfoUI.gameObject.SetActive(false);
            if (shiftAction.IsPressed() == false)
                ResetSelection();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayer))
            {
                var clickable = hit.collider.GetComponent<IActiveClickable>();
                var teamColor = hit.collider.GetComponent<IGetTeamAndProperties>().GetTeam();
                if (clickable != null && teamColor == TeamColorEnum.Blue)
                {
                    // Active Unit
                    if (clickable.CheckObjectType() == ObjectTypeEnum.unit)
                    {
                        clickable.ActiveObject();
                        Unit unitToPrepare = hit.collider.GetComponent<Unit>();
                        controlledUnits.AddToSelectedUnits(unitToPrepare);
                        commandPanelUI.PrepareUnitUI(controlledUnits.TakeSelectedUnitList());
                        commandPanelUI.gameObject.SetActive(true);

                        // Selection Info if more selected units than 1
                        //selectionInfoUI
                    }
                    // Active Building
                    else if (clickable.CheckObjectType() == ObjectTypeEnum.building)
                    {
                        clickable.ActiveObject();
                        Building buildingToPrepare = hit.collider.GetComponent<Building>();
                        commandPanelUI.PrepareBuildingUI(buildingToPrepare);
                        commandPanelUI.gameObject.SetActive(true);
                    }
                }
            }
        }

    }
    private void OnLpmHold(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            boxVisual.gameObject.SetActive(true);
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
            lpmHolding = true;
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            lpmHolding = false;
            boxVisual.gameObject.SetActive(false);
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawVisual();
            SelectUnits();
        }
    }
    private void OnLpmDoubleClick(InputAction.CallbackContext ctx)
    {

        Debug.Log("lpmDoubleClick");
    }
    private void Update()
    {
        pointerOverUI = EventSystem.current.IsPointerOverGameObject();

        if (lpmHolding)
        {
            endPosition = Input.mousePosition;
            DrawVisual();
            DrawSelection();
        }
    }
    private void ResetSelection()
    {
        foreach (var unit in controlledUnits.selectedUnits)
        {
            unit.DeActiveObject();
        }

        controlledUnits.ClearSelectedUnitsList();
    }

    void DrawVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new (Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        boxVisual.sizeDelta = boxSize;
    }

    void DrawSelection()
    {
        // do X calculations
        if (Input.mousePosition.x < startPosition.x)
        {
            // dragging left
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            // dragging right
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }

        // do Y calculations

        if (Input.mousePosition.y < startPosition.y)
        {
            // dragging down
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            // dragging up
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }

    void SelectUnits()
    {
        // lopp thru all the units
        foreach (var unit in controlledUnits.allUnits)
        {
            // if unit is within the bounds ofthe selection rect
            if (selectionBox.Contains(Camera.main.WorldToScreenPoint(unit.transform.position)))
            {
                // if any unit is within the selection add them to selection
                unit.ActiveObject();
                controlledUnits.AddToSelectedUnits(unit);
                commandPanelUI.PrepareUnitUI(controlledUnits.TakeSelectedUnitList());
                commandPanelUI.gameObject.SetActive(true);

            }
        }
    }

    
}
