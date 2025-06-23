using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Unit unit;
    [SerializeField] private FloatingHealthBar floatingHealthBsr;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;

    private void Start()
    {
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
    }
    public bool TakeDamage(int damage)
    {
        //  unit.GetUnitStats().SubstractHealth(damage);
  
        currentHealth -= damage;
        Debug.Log("ZADALEMM " + damage + " JEST TERAZ " + currentHealth);
        floatingHealthBsr.UpdateHealthBar(currentHealth, maxHealth);
        if (currentHealth <= 0)
        {
            Debug.Log("DEATH");
            //animator.SetTrigger("Death");
            //unit.Death();
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TakeDamage(2);
    }

}
