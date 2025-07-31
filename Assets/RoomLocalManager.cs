using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class RoomLocalManager : MonoBehaviour
{
    public Button toggleTeamColorSelectionPanelButton;
    public Image toggleTeamColorSelectionPanelButtonImage;
    public GameObject teamColorSelectionPanel;
    public Button changeTeamColorToBlueButton;
    public Button changeTeamColorToRedButton;

    private void Start()
    {
        toggleTeamColorSelectionPanelButtonImage = toggleTeamColorSelectionPanelButton.GetComponent<Image>();

    }
}
