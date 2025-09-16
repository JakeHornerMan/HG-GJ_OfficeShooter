using UnityEngine;
using System.Collections;

public class Boomerang : MonoBehaviour
{
    [Header("References")]
    public Transform startPoint; // player hand, or wherever it should return

    [Header("Flags")]
    public bool isThrown = false;

    [Header("Travel Settings")]
    public float travelSpeed = 20f; // units per second

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private float rotationSpeed = 360f; // degrees per second
    private Coroutine rotationCoroutine;

    [Header("RGB Settings")]
    private RGBSettings boomerangColor;
    private GameObject chosenParticles;

    [Header("Materials")]
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;

    [Header("Particles")]
    public GameObject redParticles;
    public GameObject greenParticles;
    public GameObject blueParticles;

    private void Awake()
    {
        isThrown = false;
    }

    public void StartBommerang(GameObject enemy, Vector3 hitPosition, RGBSettings color = RGBSettings.BLUE)
    {
        if (isThrown) return; // prevent multiple throws

        SetVisual(color);

        isThrown = true;
        string enemyName = enemy != null ? enemy.name : "none";
        Debug.Log($"[Boomerang] enemy: {enemyName}, hitPosition: {hitPosition}");

        if (enemy != null)
        {
            hitPosition = enemy.transform.position; // lock onto enemy
        }

        // 🔄 start rotation
        if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
        rotationCoroutine = StartCoroutine(RotateLoop());

        StartCoroutine(ThrowRoutine(hitPosition));
    }

    private IEnumerator ThrowRoutine(Vector3 targetPos)
    {
        // Go out
        yield return MoveToPosition(targetPos);

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
}
