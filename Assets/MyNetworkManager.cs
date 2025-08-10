using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public GameObject playerControllerPrefab;

    public override void OnServerSceneChanged(string sceneName)
    {

        Debug.Log("ZMIANA SCENY na " + sceneName);
        if (sceneName == "GameScene")
        {
            var gridData = new GridData();
            foreach (var conn in NetworkServer.connections.Values)
            {
                if (conn.identity != null)
                {
                    // Pobierz dane z lobby
                    var roomPlayer = conn.identity.GetComponent<PlayerRoomController>();
                    TeamColorEnum color = roomPlayer.teamColor;
                    Vector2 startPos = roomPlayer.startPositionOnMap;

                    // Stw�rz nowy PlayerController w scenie gry
                    GameObject newPC = Instantiate(playerControllerPrefab);
                    var pc = newPC.GetComponent<PlayerController>();

                    // Dodaj dla tego po��czenia
                    NetworkServer.ReplacePlayerForConnection(conn, newPC);

                    // Wywo�aj metod� inicjalizuj�c�
                    pc.CreatePlayerController(color, gridData, (int)startPos.x, (int)startPos.y);
                }
            }
        }

        //IEnumerator Delay()
        //{
        //    //yield return new WaitForSeconds(5f);

        //    //// Tutaj masz pewno��, �e serwer jest aktywny i scena za�adowana
        //    //foreach (var conn in NetworkServer.connections.Values)
        //    //{
        //    //    var playerData = conn.identity.GetComponent<PlayerRoomController>();
        //    //    Debug.Log("TWORZE NOWYCH " + playerData.teamColor);

        //    //    var playerController = conn.identity.GetComponent<PlayerController>();
        //    //    if (playerController == null) Debug.Log("NIE MA CONTROLLERA");
        //    //    else playerController.CreatePlayerController(playerData.teamColor, gridData, (int)playerData.startPositionOnMap.x, (int)playerData.startPositionOnMap.y);

        //    //}
        //}
    }
}
