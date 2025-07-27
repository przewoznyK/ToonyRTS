using TMPro;
using UnityEngine;

public class PlayerRoomController : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private TeamColorEnum teamColor;
    public void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }
}
