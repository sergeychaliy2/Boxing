using UnityEngine;
using UnityEngine.UI;

public class PunchableTarget : MonoBehaviour, IDamageable
{
    [field: SerializeField] private float maxHealth = 100f;
    [field: SerializeField] private Slider healthBar;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        GameEvents.OnTargetHealthChanged += UpdateUI;
        GameEvents.OnTargetHealthChanged?.Invoke(currentHealth / maxHealth);
    }

    public void ApplyDamage(float damage)
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        GameEvents.OnTargetHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth == 0f)
            OnDeath();
    }

    private void UpdateUI(float normalizedHealth)
    {
        if (healthBar != null)
            healthBar.value = normalizedHealth;
    }

    private void OnDestroy()
    {
        GameEvents.OnTargetHealthChanged -= UpdateUI;
    }

    private void OnDeath()
    {
        Debug.Log($"{gameObject.name} уничтожен");
    }
}
