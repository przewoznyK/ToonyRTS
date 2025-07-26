using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [field: SerializeField] public GameObject PlayerControllerPrefab { get; private set; }
    [field: SerializeField] public GameObject InputManagerPrefab { get; private set; }
    [field: SerializeField] public GameObject ActiveClickableObjectPrefab { get; private set; }
    [field: SerializeField] public GameObject ManageSelectionUnitsPrefab { get; private set; }
    [field: SerializeField] public GameObject PlayerUIPrefab { get; private set; }
    [field: SerializeField] public GameObject HoldSelectionUnitCanvas { get; private set; }
    [field: SerializeField] public GameObject BuildingProducionPrefab { get; private set; }
    [field: SerializeField] public GameObject ConstructionSystemPrefab { get; private set; }
    [field: SerializeField] public GameObject PlayerStartGameSetupPrefab { get; private set; }
    [field: SerializeField] public GameObject AccessToClassByTeamColorPrefab { get; private set; }
    [field: SerializeField] public GameObject RemoveEntityPrefab { get; private set; }


    private void Awake()
    {
        Instance = this;
        var gridData = new GridData();

        // Global
        var removeEntityInstantiate = Instantiate(RemoveEntityPrefab);
        var removeEntity = removeEntityInstantiate.GetComponent<RemoveEntity>();
        removeEntity.Init(gridData);

        var accessToClassByTeamColorInstantiate = Instantiate(AccessToClassByTeamColorPrefab);
        var accessToClassByTeamColor = accessToClassByTeamColorInstantiate.GetComponent<AccessToClassByTeamColor>();

        // BlueTeam Creator
        GameObject playerControllerInstantiate = Instantiate(PlayerControllerPrefab);
        PlayerController playerController = playerControllerInstantiate.GetComponent<PlayerController>();
        playerController.CreatePlayerController(TeamColorEnum.Blue, accessToClassByTeamColor, gridData);

        // BlueTeam Creator
        GameObject playerControllerInstantiateRed = Instantiate(PlayerControllerPrefab);
        PlayerController playerControllerRed = playerControllerInstantiate.GetComponent<PlayerController>();
        playerControllerRed.CreatePlayerController(TeamColorEnum.Red, accessToClassByTeamColor, gridData);
    }
}
