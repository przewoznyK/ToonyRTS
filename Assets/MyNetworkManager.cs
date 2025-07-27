using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();
    public string roomPassword = "";
    public string roomName = "";

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        //if (lobbyPlayers.Count >= 2)
        //{
        //    conn.Disconnect();
        //    return;
        //}
        Debug.Log("ONSERVERCONNECT");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        LobbyPlayer lp = player.GetComponent<LobbyPlayer>();
        lobbyPlayers.Add(lp);

    }

    public void StartGame()
    {
        Debug.Log("STARTGAME");
       // ServerChangeScene("GameScene");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        Debug.Log("ONSERVERSCENCHANGED");
        //if (sceneName == "GameScene")
        //{
        //    foreach (var lobbyPlayer in lobbyPlayers)
        //    {
        //        // Przenieœ dane gracza do nowego GamePlayera
        //        // Przyk³ad: narodowoœæ, kolor
        //    }
        //}
    }
}
