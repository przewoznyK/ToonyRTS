using TMPro;
using UnityEngine;

public class SummaryPanelUI : MonoBehaviour
{
    PlayerResources playerResources;

    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI rockText;
    public void Init(PlayerResources playerResources)
    {
        this.playerResources = playerResources;

        foodText.text = playerResources.currentFood.ToString();
        woodText.text = playerResources.currentWood.ToString();
        goldText.text = playerResources.currentGold.ToString();
        rockText.text = playerResources.currentRock.ToString();
    }
}
