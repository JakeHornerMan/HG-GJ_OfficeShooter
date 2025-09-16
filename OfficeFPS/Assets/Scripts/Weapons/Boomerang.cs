using UnityEngine;
using System.Collections;

public class Boomerang : MonoBehaviour
{
    [Header("References")]
    public Transform startPoint; // player hand, or wherever it should return

    [Header("Boomerang Settings")]
    public float travelSpeed = 20f; // units per second
    public float boomerangDamage = 15f;

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 360f; // degrees per second
    private Coroutine rotationCoroutine;

    [Header("RGB Settings")]
    private RGBSettings boomerangColor;

    [Header("Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    [Header("Particles")]
    public GameObject redParticles;
    public GameObject greenParticles;
    public GameObject blueParticles;
    private GameObject chosenParticles;

    [Header("Flags")]
    public bool isThrown = false;

    private void Awake()
    {
        isThrown = false;
    }

    public void StartBommerang(GameObject enemy, Vector3 hitPosition, RGBSettings color = RGBSettings.BLUE)
    {
        if (isThrown) return; // prevent multiple throws

        boomerangColor = color;
        SetVisual(color);

        isThrown = true;
        string enemyName = enemy != null ? enemy.name : "none";
        Debug.Log($"[Boomerang] enemy: {enemyName}, hitPosition: {hitPosition}");

        // 🔄 start rotation
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(RotateLoop());

        StartCoroutine(ThrowRoutine(hitPosition, enemy));
    }

    private IEnumerator ThrowRoutine(Vector3 targetPos, GameObject enemy)
    {
        if (enemy != null)
        {
            yield return MoveToEnemy(enemy);
        }
        else
        {
            yield return MoveToPosition(targetPos);
        }

        // Come back
        yield return MoveToPositionHome();

        // stop rotation
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }

        chosenParticles.SetActive(false);

        isThrown = false;
        Debug.Log("[Boomerang] returned to start, throw complete.");
    }

    private IEnumerator MoveToEnemy(GameObject enemy)
    {
        Vector3 targetPos = enemy.transform.position + new Vector3(0f, 0.5f, 0f); 
        while (Vector3.Distance(transform.position,  targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, travelSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos; // snap exactly
    }

    private IEnumerator MoveToPosition(Vector3 destination)
    {
        while (Vector3.Distance(transform.position, destination) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, travelSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination; // snap exactly
    }

    private IEnumerator MoveToPositionHome()
    {
        while (Vector3.Distance(transform.position, startPoint.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, travelSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPoint.position; // snap exactly
    }

    private IEnumerator RotateLoop()
    {
        while (true)
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
            yield return null;
        }
    }

    public void SetVisual(RGBSettings bulletType)
    {
        // Select material & particles based on enum
        Material chosenMat = null;
        chosenParticles = null;

        switch (bulletType)
        {
            case RGBSettings.RED:
                chosenMat = redMaterial;
                chosenParticles = redParticles;
                break;

            case RGBSettings.GREEN:
                chosenMat = greenMaterial;
                chosenParticles = greenParticles;
                break;

            case RGBSettings.BLUE:
                chosenMat = blueMaterial;
                chosenParticles = blueParticles;
                break;
        }

        if (chosenMat != null)
        {
            GetComponent<Renderer>().material = chosenMat;
        }
        else
        {
            Debug.LogWarning("[Bullet] No material assigned for " + bulletType);
        }

        // Enable particles
        if (chosenParticles != null)
        {
            chosenParticles.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[Bullet] No particles assigned for " + bulletType);
        }

    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isThrown) return;
        Debug.Log("[Boomerang] Hit enemy: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("[Boomerang] Hit enemy: " + other.name);

                // Example: deal damage
                enemyHealth.BoomerangHit(boomerangDamage, boomerangColor); // adjust damage value
            }
            else
            {
                Debug.LogWarning("[Boomerang] Enemy has no EnemyHealth component: " + other.name);
            }
        }
    }
}
