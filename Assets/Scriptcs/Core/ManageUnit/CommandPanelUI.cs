using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanelUI : MonoBehaviour
{
    PlayerResources playerResources;
    ShopManager shopManager;
    BuildingProduction buildingProduction;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Transform productionPanel;
    [SerializeField] private GameObject productRepresentationPrefab;
    List<UnitNameEnum> unitCanBuyList;
    private Image currentProductionImageFill;
    public void Init(PlayerResources playerResources, ShopManager shopManager, BuildingProduction buildingProduction)
    {
        this.playerResources = playerResources;
        this.shopManager = shopManager;
        this.buildingProduction = buildingProduction;
    }

    public void PrepareBuildingUI(Building building)
    {
        unitCanBuyList = building.GetUnitsCanBuyList();

        DisplayProductionQueue(building);

        for (int i = 0; i < unitCanBuyList.Count; i++)
        {
            var currentUnit = UnitDatabase.Instance.GetUnitDataByNameEnum(unitCanBuyList[i]);
            var currentButton = buttons[i];

            currentButton.image.sprite = currentUnit.unitSprite;
            SetButtonColorStatusByPrice(currentButton, currentUnit.objectPrices);
            currentButton.onClick.AddListener(() => shopManager.BuyUnit(building, currentUnit.unitName));


        }
    }
    internal void ActivePanel()
    {
        gameObject.SetActive(true);
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
        for (int i = 0; i < unitCanBuyList.Count; i++)
        {
            var currentUnit = UnitDatabase.Instance.GetUnitDataByNameEnum(unitCanBuyList[i]);
            var currentButton = buttons[i];

            currentButton.image.sprite = currentUnit.unitSprite;
            SetButtonColorStatusByPrice(currentButton, currentUnit.objectPrices);
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
                var child = productRepresentationInstantiate.transform.GetChild(1);
                child.GetComponent<Image>().sprite = product.productSprite;
                if(productsList.Peek() == product)
                {
                    var fillImageChild = productRepresentationInstantiate.transform.GetChild(2);
                    currentProductionImageFill = fillImageChild.GetComponent<Image>();
                }
            }
        }
    }

    public void StartUpdatingProductionImageFill(float productionTime)
    {
        StartCoroutine(UpdateCurrentProductionImageFill(productionTime));
    }

    IEnumerator UpdateCurrentProductionImageFill(float productionTime)
    {
        Debug.Log(productionTime);
        float timer = productionTime;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            currentProductionImageFill.fillAmount = timer / productionTime;
            yield return null;
        }
    }
}
