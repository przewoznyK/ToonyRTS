using UnityEngine;
using UnityEngine.UI;

public class UnitSetPropertiesByTeamColor : MonoBehaviour
{
    Unit unit;
    EntityHealth entityHealth;
    [SerializeField] private AttackArea attackArea;
    [SerializeField] private MeshRenderer activator;
    [SerializeField] private Image floatingHealthBarFillColor;
    [SerializeField] private MeshRenderer[] meshsToChangeMaterial;
    [SerializeField] private SkinnedMeshRenderer[] skinnedMeshToChangeMaterial;
    private void Start()
    {
        unit = GetComponent<Unit>();
        entityHealth = GetComponent<EntityHealth>();

        entityHealth.SetTeamColor(unit.teamColor);
        attackArea.SetTeamColor(unit);

        Material teamMaterialColor = TeamColorDatabase.Instance.GetTeamMaterialColor(unit.teamColor);
        activator.material = teamMaterialColor;
        floatingHealthBarFillColor.color = teamMaterialColor.color;

        Material teamMaterialUnit = TeamColorDatabase.Instance.GetTeamMaterialUnit(unit.teamColor);
        foreach (var mesh in meshsToChangeMaterial)
        {
            mesh.material = teamMaterialUnit;
        }
        foreach (var mesh in skinnedMeshToChangeMaterial)
        {
            mesh.material = teamMaterialUnit;
        }


    }
}
