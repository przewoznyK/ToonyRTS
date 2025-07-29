using UnityEngine;
using Mirror;
using TMPro;
public class PlayerTestingMirror : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    void HandleMovement()
    {
        if(isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
            transform.position = transform.position + movement;
        }
    }

    private void Update()
    {
        HandleMovement();
    }

    public void UpdateText(string value)
    {
        text.text = value;
    }
}
