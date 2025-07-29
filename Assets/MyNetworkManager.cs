using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public string roomPassword = "";
    public string roomName = "";

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log("ONSERVERCONNECT");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
    }

    public void StartGame()
    {
        Debug.Log("STARTGAME");
    }

    public override void OnServerSceneChanged(string sceneName)
    {

    }
}
