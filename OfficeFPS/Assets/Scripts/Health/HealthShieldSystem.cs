using UnityEngine;

[System.Serializable]
public class HealthShieldSystem : MonoBehaviour
{
    [Header("References")]
    public HudManager hudManager;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxShield = 50f;

    [Header("Current Values")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentShield;

    [Header("Flag Values")]
    public bool isInvincible = false;

    public float CurrentHealth => currentHealth;
    public float CurrentShield => currentShield;
    public float MaxHealth => maxHealth;
    public float MaxShield => maxShield;



    void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    void Start()
    {
        if (hudManager != null)
        {
            hudManager.UpdateHealthBar(currentHealth, maxHealth);
            hudManager.UpdateShieldBar(currentShield, maxShield);
        }
    }

    public void TakeDamage(float amount, bool ignoreInvincibility = false)
    {
        Debug.Log($"[HealthShieldSystem] TakeDamage called with amount: {amount}");
        if (amount <= 0 || (isInvincible && !ignoreInvincibility)) return;

        if (currentShield > 0)
        {
            float shieldDamage = Mathf.Min(amount, currentShield);
            currentShield -= shieldDamage;
            amount -= shieldDamage;
            hudManager.ShowShieldDamage();
            hudManager.UpdateShieldBar(currentShield, maxShield);
            Debug.Log($"Shield absorbed {shieldDamage}. Remaining Shield: {currentShield}");
        }

        if (amount > 0)
        {
            currentHealth -= amount;
            currentHealth = Mathf.Max(currentHealth, 0);
            Debug.Log($"Health took {amount} damage. Remaining Health: {currentHealth}");
            hudManager.ShowHealthDamage();
            hudManager.UpdateHealthBar(currentHealth, maxHealth);

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
        hudManager.UpdateShieldBar(currentShield, maxShield);
        Debug.Log($"Added {amount} shield. Current Shield: {currentShield}");
    }

    public void AddHealth(float amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        hudManager.UpdateHealthBar(currentHealth, maxHealth);
        Debug.Log($"Added {amount} health. Current Health: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
    }
}
