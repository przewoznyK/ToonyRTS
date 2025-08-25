using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Button[] commandButtons;
    [SerializeField] private Transform productionPanel;
    [SerializeField] private GameObject productRepresentationPrefab;
    List<UnitNameEnum> unitCanBuyList;
    private Image currentProductionImageFill;
    public Building currentSelectedBuilding;
    private BuildingProductionData buildingProductionData;
    public List<Unit> currentSelectedUnits;

    [SerializeField] private Button removeEntityButton;
    public void Init(PlayerResources playerResources, ShopManager shopManager, BuildingProduction buildingProduction, InputManager inputManager, ConstructionPreviewSystem previewSystem)
    {
        this.playerResources = playerResources;
        this.shopManager = shopManager;
        this.buildingProduction = buildingProduction;
        this.inputManager = inputManager;
        this.previewSystem = previewSystem;
        RMBClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RMB_Click];
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
            var currentButton = commandButtons[i];
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
        var buildingList = BuildingDatabase.Instance.GetBuildingList();
        
     

        for (int i = 0; i < buildingList.Count; i++)
        {
            var buildingDataForButton = buildingList[i];
            var currentButton = commandButtons[i];
            currentButton.onClick.RemoveAllListeners();
            currentButton.image.sprite = buildingDataForButton.buildingIcon;
            SetButtonColorStatusByPrice(currentButton, buildingDataForButton.objectPrices);

            currentButton.onClick.AddListener(() => previewSystem.StartPreview(currentSelectedUnits, buildingDataForButton));
        }

        removeEntityButton.onClick.RemoveAllListeners();
        foreach (var unit in unitsList)
        {
            removeEntityButton.onClick.AddListener(() => RemoveEntityWithButton(unit));
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
        foreach (var button in commandButtons)
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
    private void OnEnable()
    {

        RMBClickAction.performed += SetMeetingPositionWithRightMouseButton;
    }
    private void OnDisable()
    {
        if(currentSelectedBuilding != null)
            currentSelectedBuilding.DisableObject();
        RMBClickAction.performed -= SetMeetingPositionWithRightMouseButton;
    }

    private void Update()
    {
        if(currentProductionImageFill != null)
        {
            currentProductionImageFill.fillAmount = buildingProductionData.currentTimeProduction / buildingProductionData.timeProduction;
        }
    }
    public void RemoveEntityWithButton(Building building)
    {
        RemoveEntity.Instance.RemoveEntityFromGame(building);
    }
    public void RemoveEntityWithButton(Unit unit)
    {
        RemoveEntity.Instance.RemoveEntityFromGame(unit);
        gameObject.SetActive(false);
    }


}
