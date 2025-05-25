using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ConstructionPreviewSystem : MonoBehaviour
{
    InputManager inputManager;
    ConstructionPlacerSystem constructionPlacerSystem;
    GridData gridData;
    private InputAction mousePositionAction;
    private InputAction LMBClickAction;
    private InputAction RMBClickAction;

    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private float previewYOffset = 0.06f;
    private GameObject previewConstruction;

    [SerializeField] private Material previewMaterial;
    private Material previewMaterialInstance;
    MeshRenderer previewConstructionMeshRenderer;
    BuildingData buildingData;
    ConstructionData currentConstructionData;
    public bool isOnPreview { get; private set; }
    bool canPlaceConstruction;
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

            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                Vector3 worldPosition = hitInfo.point;
                Vector3Int snappedPosition = SnapToGrid(worldPosition);
                previewConstruction.transform.position = snappedPosition;

                if(gridData.CanPlaceObjectAt(snappedPosition, buildingData.size))
                {
                    previewMaterial.color = Color.green;
                    canPlaceConstruction = true;
                    currentConstructionData.SetPositionToOccupy(snappedPosition);

                }
                else
                {
                    previewMaterial.color = Color.red;
                    canPlaceConstruction = false;

                }
                DrawOutline(previewConstruction.transform.position, buildingData.size[0], buildingData.size[1], 2.5f);
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

        var previewConstructionMesh = previewConstruction.transform.GetChild(0);
        previewConstructionMeshRenderer = previewConstructionMesh.GetComponent<MeshRenderer>();
        currentConstructionData = new ConstructionData(unit, buildingData, previewConstructionMeshRenderer, previewConstructionMeshRenderer.material);
        previewConstructionMeshRenderer.material = previewMaterial;
        isOnPreview = true;
    }

    private void PreparePreview(GameObject previewObject)
    {
        var previewObjectMesh = previewObject.transform.GetChild(0);
        var meshMaterial = previewObjectMesh.GetComponent<Material>();
        meshMaterial = previewMaterial;
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
    public float gridSize = 0.5f;

    public Vector3Int SnapToGrid(Vector3 originalPosition)
    {
        float x = Mathf.Round(originalPosition.x / gridSize) * gridSize;
        float z = Mathf.Round(originalPosition.z / gridSize) * gridSize;

        return new Vector3Int((int)x, 1, (int)z);
    }
    [SerializeField] private LineRenderer lineRenderer;

    public void DrawOutline(Vector3 center, int width, int height, float gridSize)
    {
        lineRenderer.material = previewMaterial;
        Vector3 bottomLeft = center - new Vector3(width / 2f, 0f, height / 2f) * gridSize;

        Vector3[] corners = new Vector3[5];
        corners[0] = bottomLeft;
        corners[1] = bottomLeft + new Vector3(width * gridSize, 0, 0);
        corners[2] = corners[1] + new Vector3(0, 0, height * gridSize);
        corners[3] = bottomLeft + new Vector3(0, 0, height * gridSize);
        corners[4] = corners[0];

        lineRenderer.positionCount = corners.Length;
        lineRenderer.SetPositions(corners);
    }
}
