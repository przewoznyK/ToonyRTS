using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private Vector3 targetOffSet = new Vector3(0, 2.6f, 0);
    private void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = targetPosition.position + targetOffSet;
    }
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        gameObject.SetActive(true);
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }
}
