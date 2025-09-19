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

    [Header("Color Materials")]
    [SerializeField] private int colorMaterialSlot = 1;
    [SerializeField] private MeshRenderer[] colorRenderers;
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public Material resetMaterial;

    void Awake()
    {
        renderers = this.gameObject.GetComponentsInChildren<Renderer>();
        enemyBehaviour = GetComponent<EnemyBehaviour>();
        AwakeEnemy(GetRandomRGB());
    }

    private RGBSettings GetRandomRGB()
    {
        int random = Random.Range(0, 3); // RED, GREEN, BLUE only
        return (RGBSettings)random;
    }

    public void AwakeEnemy(RGBSettings enemyColor = RGBSettings.BLUE)
    {
        isDead = false;
        currentHealth = maxHealth;
        enemyHud.SetColorHealthBar(enemyColor);
        RestoreAlphaOnMaterials();
        SetEnemyColor(enemyColor);
        enemyHud.UpdateHealthBar(currentHealth, maxHealth);
        enemyType = enemyColor;
        enemyBehaviour.AwakeEnemyBehaviour();
    }

    public void SetEnemyColor(RGBSettings color)
    {
        if (colorRenderers == null || colorRenderers.Length == 0)
        {
            Debug.LogWarning("[EnemyHealth] No colorRenderers assigned!");
            return;
        }

        Material chosenMat = null;

        switch (color)
        {
            case RGBSettings.RED: chosenMat = redMaterial; break;
            case RGBSettings.GREEN: chosenMat = greenMaterial; break;
            case RGBSettings.BLUE: chosenMat = blueMaterial; break;
            case RGBSettings.NONE: chosenMat = resetMaterial; break;
            default:
                Debug.LogWarning("[EnemyHealth] Unsupported color: " + color);
                return;
        }

        if (chosenMat == null)
        {
            Debug.LogWarning($"[EnemyHealth] Missing material for {color}!");
            return;
        }

        foreach (MeshRenderer rend in colorRenderers)
        {
            if (rend == null) continue;

            Material[] mats = rend.materials;
            if (colorMaterialSlot >= 0 && colorMaterialSlot < mats.Length)
            {
                mats[colorMaterialSlot] = chosenMat;
                rend.materials = mats;
            }
            else
            {
                Debug.LogWarning($"[EnemyHealth] Material slot {colorMaterialSlot} out of range on {rend.name}");
            }
        }

        enemyType = color;
        enemyHud.SetColorHealthBar(color);
    }

    public void RestoreAlphaOnMaterials()
    {
        foreach (var rend in colorRenderers)
        {
            if (rend == null) continue;
            foreach (var mat in rend.materials)
            {
                if (mat == null) continue;
                Color c = mat.color;
                c.a = 1f;
                mat.color = c;
            }
        }
    }

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
                Debug.Log($"[HealthShieldSystem] {gameObject.name} dropping pickup Shield.");
                DropShield(comboVal);
            }
            if (bulletType == RGBSettings.GREEN)
            {
                Debug.Log($"[HealthShieldSystem] {gameObject.name} dropping pickup Health.");
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

    public void BoomerangHit(float damage, RGBSettings color)
    {
        if (isDead) return;
        Debug.Log($"[BoomerangHit] {gameObject.name} BoomerangHit called with damage: {damage} and color: {color}");

        if (color == enemyType)
        {
            Debug.Log($"[BoomerangHit] {gameObject.name} Reseting Color");
            enemyType = RGBSettings.NONE;
            SetEnemyColor(enemyType);
            enemyHud.SetColorHealthBar(enemyType);
        }
        else
        {
            Debug.Log($"[BoomerangHit] {gameObject.name} No Color Change, Double Damage");
            damage *= 2; // double damage if same color
        }

        if (damage <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        enemyHud.UpdateHealthBar(currentHealth, maxHealth);
        Debug.Log($"[BoomerangHit] {gameObject.name} Health took {damage} damage. Remaining Health: {currentHealth}");

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
        dropPos.y += 1f;

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

        GameObject shield = Instantiate(healthPrefab, dropPos, Quaternion.identity);
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
        StartCoroutine(DeathCoroutine());
    }

    private void DisableEnemy()
    {
        transform.position = shadowRealm;
        if (deathEffectParticles != null)
            deathEffectParticles.SetActive(false);
        enemyBehaviour.DisableEnemyBehaviour();
        this.gameObject.SetActive(false);
    }

    public IEnumerator DeathCoroutine()
    {
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float newAlpha = Mathf.Lerp(1f, 0f, t);

            foreach (var rend in colorRenderers)
            {
                if (rend == null) continue;

                foreach (var mat in rend.materials)
                {
                    if (mat == null) continue;
                    Color c = mat.color;
                    c.a = newAlpha;
                    mat.color = c;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure alpha = 0
        foreach (var rend in colorRenderers)
        {
            if (rend == null) continue;
            foreach (var mat in rend.materials)
            {
                if (mat == null) continue;
                Color c = mat.color;
                c.a = 0f;
                mat.color = c;
            }
        }
        yield return new WaitForSeconds(0.5f);

        DisableEnemy();
    }



}
