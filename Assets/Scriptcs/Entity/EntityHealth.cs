using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private FloatingHealthBar floatingHealthBsr;
    public TeamColorEnum teamColor { get; private set; }
    private int currentHealth;
    [SerializeField] private int maxHealth;

    public Action<Unit> onHurtAction;
    public Action onDeathActiom;
    private void Start()
    {
        currentHealth = maxHealth;
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
    }
    public void TakeDamageFromUnit(Unit fromUnit)
    {
        currentHealth -= fromUnit.damage;
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            onDeathActiom?.Invoke();
            return;
        }
        onHurtAction?.Invoke(fromUnit);
        
    }

    public void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }
}
