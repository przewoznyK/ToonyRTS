using Mirror;
using Mirror.BouncyCastle.Bcpg;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance;
    public List<PlayerRoomController> playerRoomControllers = new();
    public List<TextMeshProUGUI> playerSlotsText;
    [SyncVar(hook = nameof(OnSlot0NameChanged))] public string slot0Name;
    [SyncVar(hook = nameof(OnSlot1NameChanged))] public string slot1Name;
    void Start()
    {
        Instance = this;        
    }

    public void AddPlayerToLobby(PlayerRoomController player)
    {
        if (!isServer) return;
        playerRoomControllers.Add(player);

        int newSlot = GetEmptySlot();

        switch (newSlot)
        {
            case 0:
                slot0Name = player.playerName;
                break;
            case 1:
                slot1Name = player.playerName;
                break;
            default:
                break;
        }
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
}
