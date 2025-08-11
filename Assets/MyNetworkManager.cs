using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public GameObject playerControllerPrefab;

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.identity != null)
                {
                    GameObject oldPlayer = conn.identity.gameObject;

                    var roomPlayer = oldPlayer.GetComponent<PlayerRoomController>();
                    NetworkServer.Destroy(oldPlayer);

                    GameObject newPlayer = Instantiate(playerControllerPrefab);
                    var pc = newPlayer.GetComponent<PlayerController>();
                    pc.teamColor = roomPlayer.teamColor;
                    pc.startPositionX = (int)roomPlayer.startPositionOnMap.x;
                    pc.startPositionY = (int)roomPlayer.startPositionOnMap.y;

                    NetworkServer.ReplacePlayerForConnection(conn, newPlayer);
                }
            }
        }
    }
}
