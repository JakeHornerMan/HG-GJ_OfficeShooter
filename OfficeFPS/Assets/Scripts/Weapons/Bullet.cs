using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;

    [Header("Settings")]
    public float lifeTime = 10f;          // Auto-destroy after this time
    public float baseDamage = 15f;          // Damage applied on hit
    private Coroutine endLifeCoroutine = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // usually bullets don’t need gravity
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // avoid tunneling
    }

    private void Start()
    {
        SpawnBullet();
    }

    public void SpawnBullet()
    {
        EndLife(lifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            EndLife();
            return; // Ignore other bullets
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log($"Bullet hit Player {other.gameObject.name}");
            HealthShieldSystem player = other.gameObject.GetComponentInParent<HealthShieldSystem>();
            if (player != null)
            {
                player.TakeDamage(baseDamage);
                Debug.Log($"[Player] Applied {baseDamage} damage.");
                EndLife();
                return;
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet hit Enemy {other.gameObject.name}");
            EnemyHealth enemy = other.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(baseDamage);
                Debug.Log($"[Enemy] Applied {baseDamage} damage.");
                EndLife();
                return;
            }
        }

        EndLife();
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
