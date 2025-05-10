using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        var blueTeam = new PlayerController(TeamColorEnum.Blue);
    }
}
