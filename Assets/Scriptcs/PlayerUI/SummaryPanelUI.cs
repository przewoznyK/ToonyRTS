using TMPro;
using UnityEngine;

public class SummaryPanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI rockText;

    public void UpdateResource(ResourceTypesEnum resourceType, int value)
    {
        switch (resourceType)
        {
            case ResourceTypesEnum.food:
                foodText.text = value.ToString();
                break;
            case ResourceTypesEnum.wood:
                woodText.text = value.ToString();
                break;
            case ResourceTypesEnum.gold:
                goldText.text = value.ToString();
                break;
            case ResourceTypesEnum.rock:
                rockText.text = value.ToString();
                break;
            default:
                break;
        }
    }
}
