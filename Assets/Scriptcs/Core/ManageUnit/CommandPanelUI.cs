using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandPanelUI : MonoBehaviour
{
    PlayerResources playerResources;
    [SerializeField] private List<UnitNameEnum> unitToBuyList = new();
    [SerializeField] private Button[] buttons;

    public void Init(PlayerResources playerResources)
    {
        this.playerResources = playerResources;
    }

    public void SetButtonsTypeList(List<UnitNameEnum> unitToBuyList)
    {
        this.unitToBuyList = unitToBuyList;
        for (int i = 0; i < unitToBuyList.Count; i++)
        {
            var currentUnit = UnitDatabase.Instance.GetUnitDataByNameEnum(unitToBuyList[i]);
            var currentButton = buttons[i];
            currentButton.image.sprite = currentUnit.unitSprite;
            SetButtonStatusByPrice(currentButton, currentUnit.objectPrices);
        }
    }

    internal void ActivePanel()
    {
        gameObject.SetActive(true);
    }

    public void SetButtonStatusByPrice(Button button, List<ObjectPrices> objectPrices)
    {
        Image background = button.GetComponent<Image>();
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
}
