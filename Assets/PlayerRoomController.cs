using Mirror;
using UnityEngine;

public class PlayerRoomController : NetworkBehaviour
{
    [SyncVar] public string playerName;
    void Start()
    {
        if(isLocalPlayer)
        {
            string randomName = "Player" + Random.Range(0, 100);
            CmdSetPlayerName(randomName);
            CmdRegisterWithRoomManager();
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

}
