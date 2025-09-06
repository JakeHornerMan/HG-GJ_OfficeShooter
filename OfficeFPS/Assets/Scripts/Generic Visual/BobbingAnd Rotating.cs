using UnityEngine;
using System.Collections;

public class BobbingAndRotating : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float bobDuration = 1f; // time to go up or down

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // rotate around Y axis
    [SerializeField] private float rotationAmount = 180f; // degrees per half cycle

    private Vector3 startPos;
    private Quaternion startRot;
    private Coroutine bobCoroutine;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        StartBobbing();
    }

    public void StartBobbing()
    {
        if (bobCoroutine != null) StopCoroutine(bobCoroutine);
        bobCoroutine = StartCoroutine(BobLoop());
    }

    public void StopBobbing()
    {
        if (bobCoroutine != null)
        {
            StopCoroutine(bobCoroutine);
            bobCoroutine = null;
            transform.localPosition = startPos;
            transform.localRotation = startRot;
        }
    }

    private IEnumerator BobLoop()
    {
        while (true)
        {
            // --- Move UP and rotate 180 ---
            float elapsed = 0f;
            while (elapsed < bobDuration)
            {
                float t = elapsed / bobDuration;
                transform.localPosition = startPos + Vector3.up * Mathf.Lerp(0, bobHeight, t);
                transform.localRotation = startRot * Quaternion.AngleAxis(Mathf.Lerp(0, rotationAmount, t), rotationAxis);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // --- Move DOWN and rotate remaining 180 ---
            elapsed = 0f;
            while (elapsed < bobDuration)
            {
                float t = elapsed / bobDuration;
                transform.localPosition = startPos + Vector3.up * Mathf.Lerp(bobHeight, 0, t);
                transform.localRotation = startRot * Quaternion.AngleAxis(rotationAmount + Mathf.Lerp(0, rotationAmount, t), rotationAxis);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
