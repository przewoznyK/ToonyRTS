using Mirror;
using Mirror.BouncyCastle.Asn1.X509;
using UnityEngine;

public class ActiveComponentsAfterCreateBuilding : NetworkBehaviour
{
    [SerializeField] private bool activeOnStartScene;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Building building;
    [SerializeField] private EntityHealth entityHealth;
    [SerializeField] private BuildingSetProperstiesByTeamColor buildingSetPropersties;
    [SerializeField] private FloatingHealthBar floatingHealthBar;

    private void Start()
    {
        if (activeOnStartScene)
            RequestToServerToActiveComponentsForBuilding();
    }

    public void RequestToServerToActiveComponentsForBuilding()
    {
        if (PlayerRoomController.LocalPlayer.isLocalPlayer)
            PlayerRoomController.LocalPlayer.CmdActiveBuildingComponents(this.GetComponent<NetworkIdentity>());
    }

    public void RespondFromServerToActiveComponentsForBuilding()
    {
        if (PlayerRoomController.LocalPlayer.isLocalPlayer)
        {
            boxCollider.enabled = true;
            building.enabled = true;
            entityHealth.enabled = true;
            buildingSetPropersties.enabled = true;
            floatingHealthBar.enabled = true;
        }
    }
}
