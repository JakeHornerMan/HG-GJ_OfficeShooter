using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    public EnemyHud enemyHud;
    public EnemySound enemySounds;
    [SerializeField] private GameObject deathEffectParticles;
    public EnemyBehaviour enemyBehaviour;
    public Renderer[] renderers;
    public Vector3 shadowRealm = new Vector3(100f, -100f, 100f);
    public GameObject healthPrefab;
    public GameObject shieldPrefab;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] public RGBSettings enemyType;
    public bool isDead = true;

    [Header("Reset Pooling Settings")]
    public float resetTime = 3f;

    [Header("Current Values")]
    [SerializeField] private float currentHealth;

    void Awake()
    {
        renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        AwakeEnemy(enemyType);
    }

    public void AwakeEnemy(RGBSettings enemyColor = RGBSettings.BLUE)
    {
        // if (isDead) enemyColor = GetRandomColor();
        isDead = false;
        currentHealth = maxHealth;
        enemyHud.SetColorHealthBar(enemyColor);
        enemyHud.UpdateHealthBar(currentHealth, maxHealth);
        enemyType = enemyColor;
        enemyBehaviour.AwakeEnemyBehaviour();
    }

    // public RGBSettings GetRandomColor(bool includeNone = false)
    // {
    //     // Get all values
    //     RGBSettings[] values = (RGBSettings[])System.Enum.GetValues(typeof(RGBSettings));

    //     // If not including NONE, remove it from the pool
    //     if (!includeNone)
    //     {
    //         values = System.Array.FindAll(values, v => v != RGBSettings.NONE);
    //     }

    //     int index = Random.Range(0, values.Length);
    //     return values[index];
    // }

    public void TakeDamage(float amount, RGBSettings bulletType, int comboVal)
    {
        if (isDead) return;

        Debug.Log($"[HealthShieldSystem] {gameObject.name} TakeDamage called with amount: {amount}");
        if (amount <= 0) return;
        if (bulletType == enemyType)
        {
            Debug.Log($"[HealthShieldSystem] {gameObject.name} is weak to {bulletType} damage.");
            if (bulletType == RGBSettings.BLUE)
            {
                DropShield(comboVal);
            }
            if(bulletType == RGBSettings.GREEN)
            {
                DropHealth(comboVal);
            }
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

    private void DropShield(int comboVal)
    {
        if (shieldPrefab == null)
        {
            Debug.LogWarning($"[EnemyHealth] {gameObject.name} has no shieldPrefab assigned!");
            return;
        }

        // Spawn at enemy’s position + small offset (so it doesn’t clip into the ground)
        Vector3 dropPos = transform.position;
        dropPos.y += 0.5f;

        GameObject shield = Instantiate(shieldPrefab, dropPos, Quaternion.identity);
        HealPickup hp = shield.GetComponent<HealPickup>();
        if (hp != null)
        {
            hp.amount += comboVal;    
        }

        Debug.Log($"[EnemyHealth] {gameObject.name} dropped a Shield Pickup!");
    }

    private void DropHealth(int comboVal)
    {
        if (healthPrefab == null)
        {
            Debug.LogWarning($"[EnemyHealth] {gameObject.name} has no healthPrefab assigned!");
            return;
        }

        Vector3 dropPos = transform.position;
        dropPos.y += 0.5f;

        GameObject shield = Instantiate(shieldPrefab, dropPos, Quaternion.identity);
        HealPickup hp = shield.GetComponent<HealPickup>();
        if (hp != null)
        {
            hp.amount += comboVal;    
        }

        Instantiate(healthPrefab, dropPos, Quaternion.identity);
        Debug.Log($"[EnemyHealth] {gameObject.name} dropped a Health Pickup!");
    }

    

    public void AddHealth(float amount)
    {
        if (amount <= 0) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($" {gameObject.name} Added {amount} health. Current Health: {currentHealth}");
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"{gameObject.name} died!");

        if (deathEffectParticles != null)
        {
            deathEffectParticles.SetActive(true);
            deathEffectParticles.GetComponent<ParticleSystem>().Play();
            foreach (ParticleSystem ps in deathEffectParticles.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Play();
            }
        }
        StartCoroutine(FadeOutAndDisable());
    }

    private IEnumerator FadeOutAndDisable()
    {
        float elapsed = 0f;

        //stops from attacking while dieing
        enemyBehaviour.isReseting = true;

        while (elapsed < resetTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / resetTime);
            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        DisableEnemy();
    }

    private void DisableEnemy()
    {
        transform.position = shadowRealm;
        if (deathEffectParticles != null)
            deathEffectParticles.SetActive(false);
        enemyBehaviour.DisableEnemyBehaviour();
        this.gameObject.SetActive(false);
    }

}
