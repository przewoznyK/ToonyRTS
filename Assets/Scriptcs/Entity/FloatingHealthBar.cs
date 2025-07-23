using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private Vector3 targetOffSet = new Vector3(0, 2.6f, 0);
    [SerializeField] private bool isStatic;
    private void Start()
    {
        transform.position = targetPosition.position + targetOffSet;
    }
    private void Update()
    {
        if (isStatic == false)
            transform.position = targetPosition.position + targetOffSet;

        transform.rotation = Camera.main.transform.rotation;

    }
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        gameObject.SetActive(true);
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }
}
