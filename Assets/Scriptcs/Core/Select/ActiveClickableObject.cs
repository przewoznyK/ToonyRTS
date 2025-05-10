using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ActiveClickableObject : MonoBehaviour
{
    ActiveUnits activeUnits;

    private InputAction lpmClick;
    private InputAction lpmMove;
    private InputAction lpmDoubleClick;

    [SerializeField] private LayerMask clickableLayer;

    public ActiveClickableObject(ActiveUnits activeUnits)
    {
        this.activeUnits = activeUnits;
    }

    private void Awake()
    {
        lpmClick = InputManager.Instance.Inputs.actions[InputManager.INPUT_GAME_LPM_Click];
        lpmMove = InputManager.Instance.Inputs.actions[InputManager.INPUT_GAME_LPM_Move];
        lpmDoubleClick = InputManager.Instance.Inputs.actions[InputManager.INPUT_GAME_LPM_Double_Click];
    }

    private void OnEnable()
    {
        lpmClick.performed += OnLpmClick;
        lpmMove.performed += OnLpmMove;
        lpmDoubleClick.performed += OnLpmDoubleClick;
    }

    private void OnDisable()
    {
        lpmClick.performed -= OnLpmClick;
        lpmMove.performed -= OnLpmMove;
        lpmDoubleClick.performed -= OnLpmDoubleClick;
    }
    private void Update()
    {
       // Debug.Log(activeUnits);
    }
    private void OnLpmClick(InputAction.CallbackContext ctx)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, clickableLayer))
        {
            var clickable = hit.collider.GetComponent<IActiveClickable>();
            if (clickable != null)
            {
                if (clickable.ActiveObject() == ObjectTypeEnum.unit)
                {
                    Debug.Log(activeUnits);
                  //  activeUnits.AddUnit(hit.collider.GetComponent<Unit>());
                }
                
            }
        }

        Debug.Log("lpmClick");
    }
    private void OnLpmMove(InputAction.CallbackContext ctx)
    {

        Debug.Log("lpmMove");
    }
    private void OnLpmDoubleClick(InputAction.CallbackContext ctx)
    {

        Debug.Log("lpmDoubleClick");
    }

}
