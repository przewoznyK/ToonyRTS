using UnityEngine;

public class RoomPlayerProfile
{
    public int playerSlotId;
    public PlayerRoomController playerRoomController;
    public TeamColorEnum teamColor;

    public RoomPlayerProfile(int playerSlotId, PlayerRoomController playerRoomController)
    {
        this.playerSlotId = playerSlotId;
        this.playerRoomController = playerRoomController;
        teamColor = TeamColorEnum.None;
    }
}
