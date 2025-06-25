using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private FloatingHealthBar floatingHealthBsr;
    public TeamColorEnum teamColor { get; private set; }
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;

    public Action onDeathActiom;
    private void Start()
    {
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
        onDeathActiom += () => animator.SetTrigger("Death");
    }
    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            onDeathActiom?.Invoke();
            return true;
        }
        return false;
    }

    public void SetTeamColor(TeamColorEnum teamColor)
    {
        this.teamColor = teamColor;
    }
}
