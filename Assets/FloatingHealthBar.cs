using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private Vector3 targetOffSet;
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = targetPosition.position + targetOffSet;
    }
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthBar.value = currentHealth / maxHealth;
    }
}
