using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanelUI : MonoBehaviour
{
    PlayerResources playerResources;
    ShopManager shopManager;
    [SerializeField] private Button[] buttons;
    List<UnitNameEnum> unitCanBuyList;
    public void Init(PlayerResources playerResources, ShopManager shopManager)
    {
        this.playerResources = playerResources;
        this.shopManager = shopManager;
    }

    public void PrepareBuildingUI(Building buildingToPrepare)
    {
        unitCanBuyList = buildingToPrepare.GetUnitsCanBuyList();

        for (int i = 0; i < unitCanBuyList.Count; i++)
        {
            var currentUnit = UnitDatabase.Instance.GetUnitDataByNameEnum(unitCanBuyList[i]);
            var currentButton = buttons[i];

            currentButton.image.sprite = currentUnit.unitSprite;
            SetButtonColorStatusByPrice(currentButton, currentUnit.objectPrices);
            currentButton.onClick.AddListener(() => shopManager.BuyUnit(buildingToPrepare, currentUnit.unitName));

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
}
