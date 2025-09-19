using UnityEngine;
using System.Collections;

public class Boomerang : MonoBehaviour
{
    [Header("References")]
    public Transform startPoint; // player hand, or wherever it should return

    [Header("Boomerang Settings")]
    public float travelSpeed = 20f; // units per second
    public float boomerangDamage = 10f;

    [Header("Arc Settings")]
    public float arcDepth = 2f; // how far the boomerang arcs left/right

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
        Debug.Log($"[Boomerang] enemy: {enemyName}, hitPosition: {hitPosition}, @ time: {Time.time}");

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

    // private IEnumerator MoveToEnemy(GameObject enemy)
    // {
    //     // Vector3 targetPos = 
    //     while (Vector3.Distance(transform.position,  enemy.transform.position + new Vector3(0f, 0.5f, 0f)) > 0.05f)
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, enemy.transform.position + new Vector3(0f, 0.5f, 0f), travelSpeed * Time.deltaTime);
    //         yield return null;
    //     }

    //     transform.position = enemy.transform.position + new Vector3(0f, 0.5f, 0f); // snap exactly
    // }

    // private IEnumerator MoveToPosition(Vector3 destination)
    // {
    //     while (Vector3.Distance(transform.position, destination) > 0.05f)
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, destination, travelSpeed * Time.deltaTime);
    //         yield return null;
    //     }

    //     transform.position = destination; // snap exactly
    // }

    private IEnumerator MoveToPositionHome()
    {
        while (Vector3.Distance(transform.position, startPoint.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.position, travelSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPoint.position; // snap exactly
    }

    private IEnumerator MoveToEnemy(GameObject enemy)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = enemy.transform.position + new Vector3(0f, 0.5f, 0f);

        Vector3 midPoint = (startPos + targetPos) * 0.5f;
        midPoint += transform.right * arcDepth; // curve outward

        yield return MoveAlongArc(startPos, midPoint, targetPos);
    }

    private IEnumerator MoveToPosition(Vector3 destination)
    {
        Vector3 startPos = transform.position;

        Vector3 midPoint = (startPos + destination) * 0.5f;
        midPoint += transform.right * arcDepth; // curve outward

        yield return MoveAlongArc(startPos, midPoint, destination);
    }

    // private IEnumerator MoveToPositionHome()
    // {
    //     Vector3 startPos = transform.position;
    //     Vector3 destination = startPoint.position;

    //     // Middle control point bends inward
    //     Vector3 midPoint = (startPos + destination) * 0.5f;
    //     midPoint -= transform.right * 2f; // arc the other way back

    //     yield return MoveAlongArc(startPos, midPoint, destination);
    // }

    private IEnumerator MoveAlongArc(Vector3 start, Vector3 control, Vector3 end)
    {
        float t = 0f;
        float duration = Vector3.Distance(start, end) / travelSpeed;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Quadratic Bezier formula
            Vector3 m1 = Vector3.Lerp(start, control, t);
            Vector3 m2 = Vector3.Lerp(control, end, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            yield return null;
        }

        transform.position = end; // snap exactly
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
        Debug.Log("[Boomerang] OnTriggerEnter: " + Time.time + " with " + other.name);
        
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("[Boomerang] Hit enemy: " + other.name);
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log("[Boomerang] Got EnemyHealth: " + other.name);

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
