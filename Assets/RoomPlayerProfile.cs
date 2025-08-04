using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayerProfile
{
    public int playerSlotId;
    public PlayerRoomController playerRoomController;
    public string teamColorName;

    public RoomPlayerProfile(int playerSlotId, PlayerRoomController playerRoomController)
    {
        this.playerSlotId = playerSlotId;
        this.playerRoomController = playerRoomController;
        teamColorName = "None";
    }
}
