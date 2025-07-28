using Mirror;
using UnityEngine;

public class MainMenuManage : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuButtonPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject roomPanel;
    public void OpenCreateRoomPanel()
    {
        createRoomPanel.SetActive(true);
        mainMenuButtonPanel.SetActive(false);
    }

    public void BackToMainMenuButton()
    {
        mainMenuButtonPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(false);
    }
    public void OpenRoomPanel()
    {
        mainMenuButtonPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        roomPanel.SetActive(true);
    }
    public void CreateRoom()
    {
        OpenRoomPanel();
        NetworkManager.singleton.StartHost();
    }

    public void JoinRoom()
    {
        NetworkManager.singleton.networkAddress = "127.0.0.1";
        OpenRoomPanel();
        NetworkManager.singleton.StartClient();
    }
}
