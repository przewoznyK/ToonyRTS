using UnityEngine;

public class RoomLocalManager : MonoBehaviour
{
    [SerializeField] private GameObject teamColorSelectionPanel;
    
    public void ToggleTeamColorSelectionPanel()
    {
        teamColorSelectionPanel.SetActive(!teamColorSelectionPanel.activeSelf);
    }        
}
