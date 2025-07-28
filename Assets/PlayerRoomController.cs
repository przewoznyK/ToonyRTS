using Mirror;
using TMPro;
using UnityEngine;

public class PlayerRoomController : NetworkBehaviour
{
    RoomTestClicker roomTestClicker;
    [SyncVar] public string playerName;

    [Command]
    public void CmdJoinSlot(int slotIndex)
    {
        Debug.Log($"Gracz {playerName} chce do³¹czyæ do slota {slotIndex}");

        RoomManager.Instance.AssignPlayerToSlot(connectionToClient, playerName, slotIndex);
    }
    [Command]
    public override void OnStartLocalPlayer()
    {
        // Nadaj nazwê (np. tymczasowo z losowej liczby)
        playerName = "Player" + Random.Range(1000, 9999);
        roomTestClicker = FindObjectOfType<RoomTestClicker>();
        roomTestClicker.Click();
    }
}
