using Mirror;
using TMPro;
using UnityEngine.UI;

public class RoomTestClicker : NetworkBehaviour
{
    Button button;
    public TextMeshProUGUI buttonText;
    [SyncVar] public int value;
    void Start()
    {
        button = GetComponent<Button>();
    }
    [Command(requiresAuthority = false)]
    void CmdSend()
    {
        buttonText.text = value.ToString();
        value++;
        //if (!connNames.ContainsKey(sender))
        //    connNames.Add(sender, sender.identity.GetComponent<Player>().playerName);

        //if (!string.IsNullOrWhiteSpace(message))
        //    RpcReceive(connNames[sender], message.Trim());
    }
    public void Click()
    {

    }
    private void Update()
    {
        if(!isLocalPlayer) { return; }
        CmdSend();
    }
}
