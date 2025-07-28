using Mirror;
using UnityEngine;
using TMPro;

public class RoomSlot : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;

    [SerializeField] private GameObject openSlot;
    [SerializeField] private GameObject joinSlotButton;
    [SerializeField] private TextMeshProUGUI nameText;

    void OnPlayerNameChanged(string oldName, string newName)
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        nameText.text = string.IsNullOrEmpty(playerName) ? "Empty" : playerName;

        bool occupied = !string.IsNullOrEmpty(playerName);
        joinSlotButton.SetActive(!occupied);
        openSlot.SetActive(occupied);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
      //  UpdateUI();
    }
}
