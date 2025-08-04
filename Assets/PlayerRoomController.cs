using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomController : NetworkBehaviour
{
    [SyncVar] public string playerName;
    public string teamColorName;

    RoomLocalManager roomLocalManager;
    void Start()
    {
        if(isLocalPlayer)
        {
            string randomName = "Player" + Random.Range(0, 100);
            CmdSetPlayerName(randomName);
            CmdRegisterWithRoomManager();
            roomLocalManager = FindFirstObjectByType<RoomLocalManager>();


        }

    }

    [Command]
    void CmdRegisterWithRoomManager()
    {
        RoomManager.Instance.AddPlayerToLobby(this);
    }
    [Command]
    void CmdSetPlayerName(string name)
    {
        playerName = name;
    }


    [TargetRpc]
    public void TargetSlotLocked(NetworkConnection target, int slotId)
    {
        SlotLocked(slotId);
    }
    public void SlotLocked(int slotId)
    {
        if (slotId == 0)
        {
            roomLocalManager.slot0ToggleTeamColorSelectionPanelButton.onClick.AddListener(() => ToggleTeamColorSelectionPanel());
            roomLocalManager.slot0ChangeTeamColorToBlueButton.onClick.AddListener(() => SelectColorTeamButton("Blue"));
            roomLocalManager.slot0ChangeTeamColorToRedButton.onClick.AddListener(() => SelectColorTeamButton("Red"));

        }
        else if (slotId == 1)
        {
            roomLocalManager.slot1ToggleTeamColorSelectionPanelButton.onClick.AddListener(() => ToggleTeamColorSelectionPanel());
            roomLocalManager.slot1ChangeTeamColorToBlueButton.onClick.AddListener(() => SelectColorTeamButton("Blue"));
            roomLocalManager.slot1ChangeTeamColorToRedButton.onClick.AddListener(() => SelectColorTeamButton("Red"));
        }

    }
    public void ToggleTeamColorSelectionPanel()
    {
        roomLocalManager.slot0TeamColorSelectionPanel.SetActive(!roomLocalManager.slot0TeamColorSelectionPanel.activeSelf);
    }

    public void SelectColorTeamButton(string colorName)
    {
        ToggleTeamColorSelectionPanel();
        CmdChangeRoomPlayerProfileTeamColor(colorName);

    }
    [Command]
    public void CmdChangeRoomPlayerProfileTeamColor(string colorName)
    {
        RoomManager.Instance.ChangeRoomPlayerProfileTeamColor(this, colorName);
    }
}
