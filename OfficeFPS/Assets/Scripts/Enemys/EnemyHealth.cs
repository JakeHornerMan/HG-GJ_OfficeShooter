using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    public EnemyHud enemyHud;
    public EnemySound playerSounds;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private RGBSettings enemyType;
    [SerializeField] private float damageMultiplier = 5f;


    [Header("Current Values")]
    [SerializeField] private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyHud.SetColorHealthBar(enemyType);
    }

    public void TakeDamage(float amount, RGBSettings bulletType)
    {
        Debug.Log($"[HealthShieldSystem] {gameObject.name} TakeDamage called with amount: {amount}");
        if (amount <= 0) return;
        if (bulletType == enemyType)
        {
            Debug.Log($"[HealthShieldSystem] {gameObject.name} is weak to {bulletType} damage.");
            amount *= damageMultiplier; // Double damage if the bullet type matches enemy type
        }
        
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        enemyHud.UpdateHealthBar(currentHealth, maxHealth);
        Debug.Log($"Health took {amount} damage. Remaining Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void AddHealth(float amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($" {gameObject.name} Added {amount} health. Current Health: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died!");
    }

}
