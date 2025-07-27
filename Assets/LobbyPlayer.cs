using Mirror;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar] public string playerName;
    [SyncVar] public string teamColor;
    [SyncVar] public string nationality;
}
