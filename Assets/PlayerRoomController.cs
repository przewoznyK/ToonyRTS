using Mirror;
using UnityEngine;

public class PlayerRoomController : NetworkBehaviour
{
    [SyncVar] public string playerName;
    RoomLocalManager roomLocalManager;
    void Start()
    {
        if(isLocalPlayer)
        {
            string randomName = "Player" + Random.Range(0, 100);
            CmdSetPlayerName(randomName);
            CmdRegisterWithRoomManager();
            roomLocalManager = FindFirstObjectByType<RoomLocalManager>();
            roomLocalManager.toggleTeamColorSelectionPanelButton.onClick.AddListener(() => ToggleTeamColorSelectionPanel());
            roomLocalManager.changeTeamColorToBlueButton.onClick.AddListener(() => SelectColorTeamButton(TeamColorEnum.Blue));

            roomLocalManager.changeTeamColorToRedButton.onClick.AddListener(() => SelectColorTeamButton(TeamColorEnum.Red));

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


    public void ToggleTeamColorSelectionPanel()
    {
        roomLocalManager.teamColorSelectionPanel.SetActive(!roomLocalManager.teamColorSelectionPanel.activeSelf);
    }

    public void SelectColorTeamButton(TeamColorEnum teamColor)
    {
        ToggleTeamColorSelectionPanel();
        CmdChangeRoomPlayerProfileTeamColor(teamColor);

    }
    [Command]
    public void CmdChangeRoomPlayerProfileTeamColor(TeamColorEnum teamColor)
    {
        RoomManager.Instance.ChangeRoomPlayerProfileTeamColor(this, teamColor);
    }

    public void ChangeSelectedTeamColor(TeamColorEnum teamColor)
    {
        Color colorToChangeImage = Color.white;
        switch (teamColor)
        {
            case TeamColorEnum.Blue:
                colorToChangeImage = Color.blue;
                break;
            case TeamColorEnum.Red:
                colorToChangeImage = Color.red;
                break;
            default:
                break;
        }
        roomLocalManager.toggleTeamColorSelectionPanelButtonImage.color = colorToChangeImage;
    }
}
