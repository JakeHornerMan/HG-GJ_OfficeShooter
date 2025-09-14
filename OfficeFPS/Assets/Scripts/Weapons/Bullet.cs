using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private string ownerTag;
    public Material material;
    private WeaponScript weaponScript;
    private EnemyCombat enemyCombat;

    [Header("Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    [Header("Settings")]
    public float lifeTime = 10f;          // Auto-destroy after this time
    public float baseDamage = 15f;           // Damage applied on hit
    private Coroutine endLifeCoroutine = null;
    private RGBSettings bulletColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // usually bullets don’t need gravity
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // avoid tunneling
    }

    private void Start()
    {

    }

    public void SpawnBullet(
        string ownerTag = "Environment",
        RGBSettings bulletType = RGBSettings.BLUE,
        WeaponScript thisWeapon = null,
        EnemyCombat thisEnemy = null,
        float damage = 15f
    )
    {
        baseDamage = damage;
        weaponScript = thisWeapon;
        enemyCombat = thisEnemy;
        this.ownerTag = ownerTag;
        bulletColor = bulletType;
        Debug.Log($"[Bullet] Spawned by {ownerTag} with color {bulletType}");

        // Select material based on enum
        Material chosenMat = null;
        switch (bulletType)
        {
            case RGBSettings.RED:   chosenMat = redMaterial; break;
            case RGBSettings.GREEN: chosenMat = greenMaterial; break;
            case RGBSettings.BLUE:  chosenMat = blueMaterial; break;
        }

        if (chosenMat != null)
        {
            GetComponent<Renderer>().material = chosenMat; 
        }
        else
        {
            Debug.LogWarning("[Bullet] No material assigned for " + bulletType);
        }

        EndLife(lifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (ownerTag == "Player") {
            Debug.Log($"[Bullet] OnCollisonEnter hit {other.gameObject.tag} time: {Time.deltaTime}");
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            EndLife();
            return; // Ignore other bullets
        }

        if (weaponScript != null && ownerTag == "Player") {
            HandlePlayerHitLogic(other);
        }
        
        if(enemyCombat != null)
        {
            HandleOtherHitLogic(other);
        }

        EndLife();
    }

    private void HandlePlayerHitLogic(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet hit Enemy {other.gameObject.name}");
            EnemyHealth enemy = other.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                if (!enemy.isDead)
                {
                    enemy.TakeDamage(baseDamage, bulletColor);
                    Debug.Log($"[Enemy] Applied {baseDamage} damage.");
                    if (enemy.enemyType == bulletColor)
                    {
                        weaponScript.SuccessfulHitEnemy();
                    }
                    else
                    { 
                        weaponScript.UnsuccessfulHitEnemy();
                    }
                    return;
                }
            }
        }
    }

    private void HandleOtherHitLogic(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet hit Enemy {other.gameObject.name}");
            EnemyHealth enemy = other.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage, bulletColor);
                Debug.Log($"[Enemy] Applied {baseDamage} damage.");
                enemyCombat.DidNotHitPlayer();
                // EndLife();
                return;
            }
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Bullet hit Player {other.gameObject.name}");
            HealthShieldSystem player = other.gameObject.GetComponentInParent<HealthShieldSystem>();
            if (player != null)
            {
                player.TakeDamage(baseDamage);
                Debug.Log($"[Player] Applied {baseDamage} damage.");
                // EndLife();
                return;
            }
        }
        enemyCombat.DidNotHitPlayer();
    }

    

    private void EndLife(float duration = 0f)
    {
        if (endLifeCoroutine != null)
            StopCoroutine(endLifeCoroutine);

        endLifeCoroutine = StartCoroutine(EndLifeRoutine(duration));
    }
    
    private IEnumerator EndLifeRoutine(float duration = 0f)
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
