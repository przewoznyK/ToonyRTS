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

    private InputAction rmbClickAction;

    [SerializeField] private Button[] buttons;
    [SerializeField] private Transform productionPanel;
    [SerializeField] private GameObject productRepresentationPrefab;
    List<UnitNameEnum> unitCanBuyList;
    private Image currentProductionImageFill;
    private Building currentBuilding;
    public void Init(PlayerResources playerResources, ShopManager shopManager, BuildingProduction buildingProduction, InputManager inputManager)
    {
        this.playerResources = playerResources;
        this.shopManager = shopManager;
        this.buildingProduction = buildingProduction;
        this.inputManager = inputManager;

        rmbClickAction = inputManager.Inputs.actions[InputManager.INPUT_GAME_RPM_Click];
        rmbClickAction.performed += SetMeetingPositionWithRightMouseButton;
    }

    public void PrepareBuildingUI(Building building)
    {
        currentBuilding = building;

        unitCanBuyList = building.GetUnitsCanBuyList();

        DisplayProductionQueue(building);

        for (int i = 0; i < unitCanBuyList.Count; i++)
        {
            var currentUnit = UnitDatabase.Instance.GetUnitDataByNameEnum(unitCanBuyList[i]);
            var currentButton = buttons[i];
            currentButton.onClick.RemoveAllListeners();
            currentButton.image.sprite = currentUnit.unitSprite;
            SetButtonColorStatusByPrice(currentButton, currentUnit.objectPrices);
            currentButton.onClick.AddListener(() => shopManager.BuyUnit(building, currentUnit.unitName));
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
                button.onClick.AddListener(() => cancelProduction.CancelProduction(playerResources, buildingProduction, currentBuilding, product));
                // Set Icon
                var child = productRepresentationInstantiate.transform.GetChild(1);
                child.GetComponent<Image>().sprite = product.productSprite;
                if(productsList.Peek() == product)
                {
                    // Set First Production Fill to Dicrease Overtime
                    var fillImageChild = productRepresentationInstantiate.transform.GetChild(2);
                    currentProductionImageFill = fillImageChild.GetComponent<Image>();
                }
            }
        }
    }

    public IEnumerator UpdateCurrentProductionImageFill(float productionTime)
    {
        float timer = productionTime;
        while (timer > 0f)
        {
            if (currentProductionImageFill == null) break;
            timer -= Time.deltaTime;
            currentProductionImageFill.fillAmount = timer / productionTime;
            yield return null;
        }
    }

    // BUTTONS
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
        for (int i = 0; i < unitCanBuyList.Count; i++)
        {
            var currentUnit = UnitDatabase.Instance.GetUnitDataByNameEnum(unitCanBuyList[i]);
            var currentButton = buttons[i];

            currentButton.image.sprite = currentUnit.unitSprite;
            SetButtonColorStatusByPrice(currentButton, currentUnit.objectPrices);
        }
    }

    private void SetMeetingPositionWithRightMouseButton(InputAction.CallbackContext ctx)
    {
        if(currentBuilding != false)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                currentBuilding.SetMeetingPoint(hit.point);

            }
        }
    }

    private void OnDisable()
    {
        rmbClickAction.performed -= SetMeetingPositionWithRightMouseButton;
    }
}
