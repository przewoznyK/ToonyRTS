using UnityEngine;
using UnityEngine.UI;

public class BuildingSetProperstiesByTeamColor : MonoBehaviour
{
    Building building;
    EntityHealth entityHealth;
    [SerializeField] private Image floatingHealthBarFillColor;
    [SerializeField] private MeshRenderer[] meshsToChangeMaterial;
    private void Start()
    {
        building = GetComponent<Building>();
        entityHealth = GetComponent<EntityHealth>();

        entityHealth.SetTeamColor(building.teamColor);

        Material teamMaterialColor = TeamColorDatabase.Instance.GetTeamMaterialColor(building.teamColor);
        Debug.Log(teamMaterialColor);
        floatingHealthBarFillColor.color = teamMaterialColor.color;

        Material teamMaterialUnit = TeamColorDatabase.Instance.GetTeamBuildingUnit(building.teamColor);
        foreach (var mesh in meshsToChangeMaterial)
        {
            mesh.material = teamMaterialUnit;
        }
    }
}
