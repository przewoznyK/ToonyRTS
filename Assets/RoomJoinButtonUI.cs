using UnityEngine;
using UnityEngine.UI;

public class RoomJoinButtonUI : MonoBehaviour
{
    public int slotIndex; // ustawiæ w Inspectorze

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClickJoin);
    }

    void OnClickJoin()
    {
        Debug.Log("CLICK JOIN");
        var player = Mirror.NetworkClient.connection.identity.GetComponent<PlayerRoomController>();
        player.CmdJoinSlot(slotIndex);
    }
}
