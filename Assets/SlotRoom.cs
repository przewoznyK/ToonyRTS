using TMPro;
using UnityEngine;

public class SlotRoom : MonoBehaviour
{
    [SerializeField] private GameObject joinButton;
    [SerializeField] private GameObject playerSelection;
    [SerializeField] private TextMeshProUGUI playerNameText;
    public void JoinToSlot()
    {
        joinButton.SetActive(false);
        playerSelection.SetActive(true);
    }
}
