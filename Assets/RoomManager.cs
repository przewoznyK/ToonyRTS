using Mirror;
using Mirror.BouncyCastle.Bcpg;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance;
    public List<RoomPlayerProfile> roomPlayerProfiles = new();
    public List<TextMeshProUGUI> playerSlotsText;
    [SyncVar(hook = nameof(OnSlot0NameChanged))] public string slot0Name;
    [SyncVar(hook = nameof(OnSlot1NameChanged))] public string slot1Name;
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

    public void ChangeRoomPlayerProfileTeamColor(PlayerRoomController playerRoomController, TeamColorEnum teamColor)
    {
        var player = roomPlayerProfiles.Find(p => p.playerRoomController == playerRoomController);
        if (player != null)
        {
            player.teamColor = teamColor;
            Debug.Log("DLA gracza " + player.playerRoomController.playerName + " Zmieniono kolor na: " + player.teamColor);
            player.playerRoomController.ChangeSelectedTeamColor(teamColor);
        }
    }
}
