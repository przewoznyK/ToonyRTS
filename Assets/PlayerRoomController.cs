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

    #region Lobby
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
            roomLocalManager.slot0ToggleTeamColorSelectionPanelButton.onClick.AddListener(() => ToggleTeamColorSelectionPanel(slotId));
            roomLocalManager.slot0ChangeTeamColorToBlueButton.onClick.AddListener(() => SelectColorTeamButton(slotId, "Blue"));
            roomLocalManager.slot0ChangeTeamColorToRedButton.onClick.AddListener(() => SelectColorTeamButton(slotId, "Red"));

        }
        else if (slotId == 1)
        {
            roomLocalManager.slot1ToggleTeamColorSelectionPanelButton.onClick.AddListener(() => ToggleTeamColorSelectionPanel(slotId));
            roomLocalManager.slot1ChangeTeamColorToBlueButton.onClick.AddListener(() => SelectColorTeamButton(slotId, "Blue"));
            roomLocalManager.slot1ChangeTeamColorToRedButton.onClick.AddListener(() => SelectColorTeamButton(slotId, "Red"));
        }

    }
    public void ToggleTeamColorSelectionPanel(int slotId)
    {
        if(slotId == 0) 
            roomLocalManager.slot0TeamColorSelectionPanel.SetActive(!roomLocalManager.slot0TeamColorSelectionPanel.activeSelf);
        else if(slotId == 1)
            roomLocalManager.slot1TeamColorSelectionPanel.SetActive(!roomLocalManager.slot1TeamColorSelectionPanel.activeSelf);
    }

    public void SelectColorTeamButton(int slotId, string colorName)
    {
        ToggleTeamColorSelectionPanel(slotId);
        CmdChangeRoomPlayerProfileTeamColor(colorName);

    }
    [Command]
    public void CmdChangeRoomPlayerProfileTeamColor(string colorName)
    {
        RoomManager.Instance.ChangeRoomPlayerProfileTeamColor(this, colorName);
    }
    #endregion
    [Command]
    public void CmdMoveUnit(NetworkIdentity unitIdentity, Vector3 targetPos)
    {
        Debug.Log($"CmdMoveUnit executed on server for {unitIdentity.gameObject.name}");
        if (unitIdentity != null && unitIdentity.TryGetComponent<UnitTaskManager>(out var taskManager))
        {
            taskManager.RespondFromServerMoveAgentTo(targetPos);
        }
    }
}
