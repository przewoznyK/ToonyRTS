using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class RoomLocalManager : MonoBehaviour
{
    [Header("SLOT0")]
    public Button slot0ToggleTeamColorSelectionPanelButton;
    public Image slot0ToggleTeamColorSelectionPanelButtonImage;
    public GameObject slot0TeamColorSelectionPanel;
    public Button slot0ChangeTeamColorToBlueButton;
    public Button slot0ChangeTeamColorToRedButton;
    [Header("SLOT1")]
    public Button slot1ToggleTeamColorSelectionPanelButton;
    public Image slot1ToggleTeamColorSelectionPanelButtonImage;
    public GameObject slot1TeamColorSelectionPanel;
    public Button slot1ChangeTeamColorToBlueButton;
    public Button slot1ChangeTeamColorToRedButton;
}
