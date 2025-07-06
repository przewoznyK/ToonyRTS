using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private FloatingHealthBar floatingHealthBsr;
    public TeamColorEnum teamColor { get; private set; }
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;

    public Action<Unit> onHurtAction;
    public Action onDeathActiom;
    private void Start()
    {
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
    }
    public void TakeDamage(Unit fromUnit, int damage)
    {
        currentHealth -= damage;
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            onDeathActiom?.Invoke();
            return;
        }
        onHurtAction.Invoke(fromUnit);
        
    }

    public void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }
}
