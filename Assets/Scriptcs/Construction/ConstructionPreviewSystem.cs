using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConstructionPreviewSystem : MonoBehaviour
{
    InputManager inputManager;
    ConstructionPlacerSystem constructionPlacerSystem;
    GridData gridData;
    private InputAction mousePositionAction;
    private InputAction LMBClickAction;
    private InputAction RMBClickAction;

    [SerializeField] private Grid gridComponent;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float previewYOffset = 0.06f;
    private GameObject previewConstruction;

    [SerializeField] private Material previewMaterial;
    [SerializeField] private GameObject previewBuildingTile;
    private Material previewMaterialInstance;
    MeshRenderer previewConstructionMeshRenderer;
    BuildingData buildingData;
    ConstructionData currentConstructionData;
    public bool isOnPreview { get; private set; }
    bool canPlaceConstruction;
    private float gridSize = 1f;
    internal void Init(InputManager inputManager, ConstructionPlacerSystem constructionPlacerSystem, GridData gridData)
    {
        this.inputManager = inputManager;
        this.constructionPlacerSystem = constructionPlacerSystem;
        this.gridData = gridData;

        mousePositionAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_MOUSE_POSITION];
        LMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_LMB_Click];
        RMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];

    }

    private void OnEnable()
    {
        
        LMBClickAction.performed += AcceptConstructionPosition;
        RMBClickAction.performed += CancelPreview;
    }
    private void Update()
    {
        if (isOnPreview)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                Vector3 worldPosition = hitInfo.point;
                Vector3Int cellPosition = gridComponent.WorldToCell(worldPosition);

                Vector3 snappedWorldPosition = gridComponent.CellToWorld(cellPosition);
                previewConstruction.transform.position = new Vector3(snappedWorldPosition.x, previewYOffset, snappedWorldPosition.z);

                if(gridData.CanPlaceObjectAt(cellPosition, buildingData.size))
                {
                    previewMaterial.color = Color.green;
                    canPlaceConstruction = true;
                    currentConstructionData.SetPositionToOccupy(cellPosition);

                }
                else
                {
                    previewMaterial.color = Color.red;
                    canPlaceConstruction = false;

                }
             //   DrawOutline(cellPosition, buildingData.size);
            }
        }
        
    }
    private void OnDisable()
    {
        LMBClickAction.performed -= AcceptConstructionPosition;
        RMBClickAction.performed -= CancelPreview;
        currentConstructionData = null;
        canPlaceConstruction = false;
        isOnPreview = false;

        gridVisualization.SetActive(false);
    }
    public void StartPreview(List<Unit> unit, BuildingData buildingData)
    {
        this.enabled = true;
        this.buildingData = buildingData;
    

        gridVisualization.SetActive(true);
        previewConstruction = Instantiate(buildingData.buildingPrefab);
        for (int x = 0; x < buildingData.size.x; x++)
        {
            for (int y = 0; y < buildingData.size.y; y++)
            {
                Vector3 tilePosition = previewConstruction.transform.position + new Vector3(x, 0.5f, y);
              GameObject tile =  Instantiate(previewBuildingTile, tilePosition, Quaternion.identity, previewConstruction.transform);
                tile.transform.localScale = gridComponent.cellSize;
                tile.transform.Rotate(90, 0, 0);
            }
        }
        var previewConstructionMesh = previewConstruction.transform.GetChild(0);
        previewConstructionMeshRenderer = previewConstructionMesh.GetComponent<MeshRenderer>();
        currentConstructionData = new ConstructionData(unit, buildingData, previewConstructionMeshRenderer, previewConstructionMeshRenderer.material);
        previewConstructionMeshRenderer.material = previewMaterial;
        isOnPreview = true;
    }

    private void AcceptConstructionPosition(InputAction.CallbackContext context)
    {
        if (canPlaceConstruction)
        {
            constructionPlacerSystem.PlaceConstruction(gridData, currentConstructionData);
            this.enabled = false;
        }
    }
    private void CancelPreview(InputAction.CallbackContext context)
    {
        Debug.Log("Cancel");

    }


    public void DrawOutline(Vector3Int cellPosition, Vector2Int size)
    {
        lineRenderer.material = previewMaterial;
        Vector3 cellSize = gridComponent.cellSize;


        Vector3 bottomLeft = gridComponent.CellToWorld(cellPosition);

        Vector3[] corners = new Vector3[5];
        corners[0] = bottomLeft;
        corners[1] = bottomLeft + new Vector3(size.x * cellSize.x, 0, 0);
        corners[2] = bottomLeft + new Vector3(size.x * cellSize.x, 0, size.y * cellSize.z);
        corners[3] = bottomLeft + new Vector3(0, 0, size.y * cellSize.z);
        corners[4] = corners[0]; 

        for (int i = 0; i < corners.Length; i++)
            corners[i].y += 0.05f;

        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
        lineRenderer.widthMultiplier = 0.05f;
    }
}


