using Mirror;
using UnityEngine;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance;

    public RoomSlot[] slots; // przypisujesz w Inspectorze

    private void Awake()
    {
        Instance = this;
    }

    [Server]
    public void AssignPlayerToSlot(NetworkConnectionToClient conn, string name, int slotIndex)
    {
        Debug.Log("DODAJE " + name + " DO SLOTA " + slotIndex);
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        RoomSlot slot = slots[slotIndex];
        slot.playerName = name;
        slot.UpdateUI();
    }
}
