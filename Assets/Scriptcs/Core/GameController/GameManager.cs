using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public InputManager inputManager;
    public ActiveClickableObject activeClickableObject;
    public ManageSelectionUnits manageSelectionUnits;
    public PlayerStartGameSetup playerStartGameSetup;
    public RemoveEntity removeEntity;
    public BuildingProduction buildingProduction;


    [Header("PlayerUI")]
    public SelectionInfoUI selectionInfoUI;
    public CommandPanelUI commandPanelUI;
    public SummaryPanelUI summaryPanelUI;
    [Header("HoldSelectionUnitsCanvas")]
    public RectTransform boxVisual;
    [Header("ConstructionSystem")]

    public ConstructionPreviewSystem previewSystem;
    public ConstructionPlacerSystem constructionPlacerSystem;
    public Transform previewSystemTransform;
    GridData gridData;
    private void Awake()
    {
        Instance = this;

    }    
}
