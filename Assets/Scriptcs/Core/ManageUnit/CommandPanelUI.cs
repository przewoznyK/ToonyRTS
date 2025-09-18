using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class CommandPanelUI : MonoBehaviour
{
    PlayerResources playerResources;
    ShopManager shopManager;
    BuildingProduction buildingProduction;
    InputManager inputManager;
    ConstructionPreviewSystem previewSystem;

    private InputAction RMBClickAction;

    [SerializeField] private Button[] classCommandButtons;
    [SerializeField] private Transform productionPanel;
    [SerializeField] private GameObject productRepresentationPrefab;
    [SerializeField] private Button removeEntityButton;
    [SerializeField] private Button toggleAggresiveApproachButton;
    [SerializeField] private Image toggleAggresiveApproachButtonStatusColor;
    List<UnitNameEnum> unitCanBuyList;
    private Image currentProductionImageFill;
    public Building currentSelectedBuilding { get; private set; }
    private BuildingProductionData buildingProductionData;
    public List<Unit> currentSelectedUnits;

    private bool aggresiveApproach;

    public void Init(PlayerResources playerResources, ShopManager shopManager, BuildingProduction buildingProduction, InputManager inputManager, ConstructionPreviewSystem previewSystem)
    {
        this.playerResources = playerResources;
        this.shopManager = shopManager;
        this.buildingProduction = buildingProduction;
        this.inputManager = inputManager;
        this.previewSystem = previewSystem;
        RMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];
    }

    private void Update()
    {
        if (currentProductionImageFill != null)
        {
            currentProductionImageFill.fillAmount = buildingProductionData.currentTimeProduction / buildingProductionData.timeProduction;
        }
    }
    private void OnEnable()
    {

        RMBClickAction.performed += SetMeetingPositionWithRightMouseButton;
    }
    private void OnDisable()
    {
        if (currentSelectedBuilding != null)
            currentSelectedBuilding.DisableObject();
        RMBClickAction.performed -= SetMeetingPositionWithRightMouseButton;
    }

    public void PrepareBuildingUI(Building building)
    {
        ClearCommandButtons();

        currentSelectedUnits = null;
        currentSelectedBuilding = building;
        productionPanel.gameObject.SetActive(true);
        unitCanBuyList = building.GetUnitsCanBuyList();

        DisplayProductionQueue(building);

        for (int i = 0; i < unitCanBuyList.Count; i++)
        {
            var unitDataForButton = UnitDatabase.Instance.GetUnitDataByNameEnum(unitCanBuyList[i]);
            var currentButton = classCommandButtons[i];
            currentButton.onClick.RemoveAllListeners();
            currentButton.image.sprite = unitDataForButton.unitSprite;
            SetButtonColorStatusByPrice(currentButton, unitDataForButton.objectPrices);
            currentButton.onClick.AddListener(() => shopManager.BuyUnit(playerResources, building, unitDataForButton.unitName));
        }

        removeEntityButton.onClick.RemoveAllListeners();
        removeEntityButton.onClick.AddListener(() => RemoveEntityWithButton(building));

    }
    public void PrepareUnitUI(List<Unit> unitsList)
    {
        ClearCommandButtons();

        currentSelectedBuilding = null;
        currentSelectedUnits = unitsList;
        productionPanel.gameObject.SetActive(false);

        PrepareButtonsByUnitClass(unitsList);

        removeEntityButton.onClick.RemoveAllListeners();
        toggleAggresiveApproachButton.onClick.RemoveAllListeners();
        toggleAggresiveApproachButtonStatusColor.color = Color.white;

        // aggresiveApproach = false;

        toggleAggresiveApproachButton.onClick.AddListener(() => SetAggresiveApproachButton());
        foreach (var unit in unitsList)
        {
            removeEntityButton.onClick.AddListener(() => RemoveEntityWithButton(unit));

        }


    }

    public void PrepareButtonsByUnitClass(List<Unit> unitsList)
    {
        bool allGatherers = unitsList.All(unit => unit is GathererNew);
        if (allGatherers)
        {
            var avalibleBuildingList = BuildingDatabase.Instance.GetAvalibleBuildingList();

            for (int i = 0; i < avalibleBuildingList.Count; i++)
            {
                var buildingDataForButton = avalibleBuildingList[i];
                var currentButton = classCommandButtons[i];
                currentButton.onClick.RemoveAllListeners();
                currentButton.image.sprite = buildingDataForButton.buildingIcon;
                SetButtonColorStatusByPrice(currentButton, buildingDataForButton.objectPrices);
                currentButton.onClick.AddListener(() => previewSystem.StartPreview(currentSelectedUnits, buildingDataForButton));
            }
        }
    }


    public void DisplayProductionQueue(Building building)
    {
        var productsList = buildingProduction.GetProductsFromThisBuilding(building);

        foreach (Transform child in productionPanel)
        {
            GameObject.Destroy(child.gameObject);
        }

        if (productsList != null)
        {
            foreach (var product in productsList)
            {
                var productRepresentationInstantiate = Instantiate(productRepresentationPrefab, productionPanel);
                // Prepare Cancel Button
                var button = productRepresentationInstantiate.GetComponent<Button>();
                var cancelProduction = productRepresentationInstantiate.GetComponent<CancelProductionButton>();
                button.onClick.AddListener(() => cancelProduction.CancelProduction(playerResources, buildingProduction, currentSelectedBuilding, product));
                // Set Icon
                var child = productRepresentationInstantiate.transform.GetChild(1);
                child.GetComponent<Image>().sprite = product.productSprite;
                if(productsList.Peek() == product)
                {
                    // Set First Production Fill to Dicrease Overtime
                    var fillImageChild = productRepresentationInstantiate.transform.GetChild(2);
                    currentProductionImageFill = fillImageChild.GetComponent<Image>();
                    buildingProductionData = buildingProduction.GetProductingData(currentSelectedBuilding);
                }
            }
        }
    }

    // BUTTONS
    public void ClearCommandButtons()
    {
        foreach (var button in classCommandButtons)
        {
            button.onClick.RemoveAllListeners();
            button.image.sprite = null;
        }
    }
    public void SetButtonColorStatusByPrice(Button button, List<ObjectPrices> objectPrices)
    {
        Image background = button.transform.parent.GetComponent<Image>();
        if (playerResources.CanPlayerBuyIt(objectPrices))
        {

            button.enabled = true;
            background.color = Color.green;

        }
        else
        {
            button.enabled = false;
            background.color = Color.red;
        }

    }

    public void RefreshButtonsStatus()
    {
        if (currentSelectedBuilding == null)
            PrepareUnitUI(currentSelectedUnits);
        else
            PrepareBuildingUI(currentSelectedBuilding);
    }

    private void SetMeetingPositionWithRightMouseButton(InputAction.CallbackContext ctx)
    {
        if (currentSelectedBuilding != false)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                currentSelectedBuilding.RequestToServerToUpdateMeetingPoint(hit.point);

            }
        }
    }



    public void RemoveEntityWithButton(Building building)
    {
        RemoveEntity.Instance.RemoveEntityFromGame(building);
    }
    public void RemoveEntityWithButton(Unit unit)
    {

        unit.RequestToServerToRemoveUnit();
        gameObject.SetActive(false);
    }

    public void ToggleAggresiveApproachButton()
    {
        if (gameObject.activeSelf == false) return;
        
        if (aggresiveApproach == false)
            SetAggresiveApproachButton();
        else ResetAggresiveApproachButton();

        aggresiveApproach = !aggresiveApproach;
    }

    private void SetAggresiveApproachButton()
    {
        toggleAggresiveApproachButton.onClick.RemoveAllListeners();

        PlayerController.LocalPlayer.aggressiveApproachCommand = true;
        toggleAggresiveApproachButton.onClick.AddListener(() => ResetAggresiveApproachButton());

        toggleAggresiveApproachButtonStatusColor.color = Color.red;
    }

    public void ResetAggresiveApproachButton()
    {
        toggleAggresiveApproachButton.onClick.RemoveAllListeners();

        PlayerController.LocalPlayer.aggressiveApproachCommand = false;
        toggleAggresiveApproachButton.onClick.AddListener(() => SetAggresiveApproachButton());
        toggleAggresiveApproachButtonStatusColor.color = Color.white;
    }
}
