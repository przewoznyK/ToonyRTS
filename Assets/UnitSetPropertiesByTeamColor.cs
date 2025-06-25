using UnityEngine;
using UnityEngine.UI;

public class UnitSetPropertiesByTeamColor : MonoBehaviour
{
    Unit unit;
    EntityHealth entityHealth;
    [SerializeField] private AttackArea attackArea;
    [SerializeField] private MeshRenderer activator;
    [SerializeField] private Image floatingHealthBarFillColor;

    private void Start()
    {
        unit = GetComponent<Unit>();
        entityHealth = GetComponent<EntityHealth>();
        activator.material = TeamColorDatabase.Instance.GetTeamMaterial(unit.teamColor);
        floatingHealthBarFillColor.color = TeamColorDatabase.Instance.GetTeamMaterial(unit.teamColor).color;
        entityHealth.SetTeamColor(unit.teamColor);
        attackArea.SetTeamColor(unit.teamColor);
    }
}
