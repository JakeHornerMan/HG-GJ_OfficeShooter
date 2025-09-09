using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rb;
    private string ownerTag;
    public Material material;

    [Header("Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    [Header("Settings")]
    public float lifeTime = 10f;          // Auto-destroy after this time
    public float baseDamage = 15f;          // Damage applied on hit
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

    public void SpawnBullet(string ownerTag = "Environment", RGBSettings bulletType = RGBSettings.BLUE)
    {
        this.ownerTag = ownerTag;

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
        if (other.gameObject.CompareTag("Bullet"))
        {
            EndLife();
            return; // Ignore other bullets
        }

        if (other.gameObject.CompareTag("Player") && ownerTag != "Player")
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
