using UnityEngine;
using UnityEngine.UI;

public class UnitSetPropertiesByTeamColor : MonoBehaviour
{
    MeleeWarrior meleeWarrior;
    EntityHealth entityHealth;
    [SerializeField] private UnitAnimationFunctions animationFunctions;
    [SerializeField] private MeshRenderer activator;
    [SerializeField] private Image floatingHealthBarFillColor;
    [SerializeField] private MeshRenderer[] meshsToChangeMaterial;
    [SerializeField] private SkinnedMeshRenderer[] skinnedMeshToChangeMaterial;
    private void Start()
    {
        meleeWarrior = GetComponent<MeleeWarrior>();
        entityHealth = GetComponent<EntityHealth>();

        entityHealth.SetTeamColor(meleeWarrior.teamColor);
        animationFunctions.Init(meleeWarrior);

        Material teamMaterialColor = TeamColorDatabase.Instance.GetTeamMaterialColor(meleeWarrior.teamColor);
        activator.material = teamMaterialColor;
        floatingHealthBarFillColor.color = teamMaterialColor.color;

        Material teamMaterialUnit = TeamColorDatabase.Instance.GetTeamMaterialUnit(meleeWarrior.teamColor);
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
