using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private TeamColorEnum teamColor;

    internal void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EntityHealth>(out EntityHealth entityHealth))
        {
            if (entityHealth.teamColor != teamColor)
            {
                Debug.Log("DODAC DAMAGE");
                entityHealth.TakeDamage(3);
            }
        }
    }
}
