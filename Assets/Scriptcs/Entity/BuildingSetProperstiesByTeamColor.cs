using UnityEngine;
using UnityEngine.UI;

public class BuildingSetProperstiesByTeamColor : MonoBehaviour
{
    [SerializeField] private Building building;
    [SerializeField] private EntityHealth entityHealth;
    [SerializeField] private Image floatingHealthBarFillColor;
    [SerializeField] private MeshRenderer[] meshsToChangeMaterial;
    public void Init()
    {
        entityHealth.SetTeamColor(building.teamColor);
        Debug.Log("building color " + building.teamColor);
        Material teamMaterialColor = TeamColorDatabase.Instance.GetTeamMaterialColor(building.teamColor);
        floatingHealthBarFillColor.color = teamMaterialColor.color;

        Material teamMaterialUnit = TeamColorDatabase.Instance.GetTeamBuildingUnit(building.teamColor);
        foreach (var mesh in meshsToChangeMaterial)
        {
            mesh.material = teamMaterialUnit;
        }
    }
}
