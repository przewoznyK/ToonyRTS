using Mirror;
using Mirror.BouncyCastle.Bcpg;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance;
    public List<RoomPlayerProfile> roomPlayerProfiles = new();
    public List<TextMeshProUGUI> playerSlotsText;
    [SyncVar(hook = nameof(OnSlot0NameChanged))] public string slot0Name;
    [SyncVar(hook = nameof(OnSlot1NameChanged))] public string slot1Name;
    public Image slot0Image;
    [SyncVar(hook = nameof(OnSlot0TeamColorChanged))] public string slot0TeamColorName;
    public Image slot1Image;
    [SyncVar(hook = nameof(OnSlot1TeamColorChanged))] public string slot1TeamColorName;


    void Start()
    {
        Instance = this;
    }

    public void AddPlayerToLobby(PlayerRoomController playerToAdd)
    {
        if (!isServer) return;

        int emptySlotId = GetEmptySlot();

        switch (emptySlotId)
        {
            case 0:
                slot0Name = playerToAdd.playerName;
                break;
            case 1:
                slot1Name = playerToAdd.playerName;
                break;
            default:
                break;
        }
        var newPlayerProfile = new RoomPlayerProfile(emptySlotId, playerToAdd);
        roomPlayerProfiles.Add(newPlayerProfile);
        playerToAdd.TargetSlotLocked(playerToAdd.connectionToClient, emptySlotId);
    }

    public int GetEmptySlot()
    {
        for (int i = 0; i < playerSlotsText.Count; i++)
        {
            if (playerSlotsText[i].text == "") return i;
        }
        return 3;
    }

    void OnSlot0NameChanged(string oldName, string newName)
    {
        if (playerSlotsText[0] != null)
        {
            playerSlotsText[0].text = string.IsNullOrEmpty(newName) ? "Empty" : newName;
        }

    }

    void OnSlot1NameChanged(string oldName, string newName)
    {
        if (playerSlotsText[1] != null)
        {
            playerSlotsText[1].text = string.IsNullOrEmpty(newName) ? "Empty" : newName;
        }

    }
    void OnSlot0TeamColorChanged(string oldName, string newName)
    {
        slot0Image.color = GetColorByName(newName);
    }
    void OnSlot1TeamColorChanged(string oldName, string newName)
    {
        slot1Image.color = GetColorByName(newName);
    }
    public Color GetColorByName(string colorName)
    {
        switch (colorName)
        {
            case "Blue":
                return Color.blue;
            case "Red":
                return Color.red;
            default:
                return Color.white;
        }
    }
    public void ChangeRoomPlayerProfileTeamColor(PlayerRoomController playerRoomController, string colorName)
    {
        var player = roomPlayerProfiles.Find(p => p.playerRoomController == playerRoomController);
        if (player != null)
        {
            player.teamColorName = colorName;
            if(player.playerSlotId == 0)
                slot0TeamColorName = colorName;
            else if (player.playerSlotId == 1)
                slot1TeamColorName = colorName;
        }
    }

    public void StartGame()
    {
        if(NetworkServer.active)
            NetworkManager.singleton.ServerChangeScene("GameScene");
    }
}
