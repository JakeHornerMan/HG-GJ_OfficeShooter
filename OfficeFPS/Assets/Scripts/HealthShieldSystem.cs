using UnityEngine;

[System.Serializable]
public class HealthShieldSystem : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 50f;

    [Header("Current Values")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentShield;

    public float CurrentHealth => currentHealth;
    public float CurrentShield => currentShield;
    public float MaxHealth => maxHealth;
    public float MaxShield => maxShield;

    void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0) return;

        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(amount, currentShield);
            currentShield -= shieldDamage;
            amount -= shieldDamage;
            Debug.Log($"Shield absorbed {shieldDamage}. Remaining Shield: {currentShield}");
        }

        if (amount > 0)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);
            Debug.Log($"Health took {amount} damage. Remaining Health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void AddShield(float amount)
    {
        if (amount <= 0) return;
        currentShield = Mathf.Min(currentShield + amount, maxShield);
        Debug.Log($"Added {amount} shield. Current Shield: {currentShield}");
    }

    public void AddHealth(float amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Added {amount} health. Current Health: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
    }
}
