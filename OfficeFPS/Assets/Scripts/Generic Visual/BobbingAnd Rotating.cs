using UnityEngine;
using System.Collections;

public class BobbingAndRotating : MonoBehaviour
{
    [Header("Enable Features")]
    [SerializeField] private bool enableBobbing = true;
    [SerializeField] private bool enableRotation = true;

    [Header("Bobbing Settings")]
    [SerializeField] private float bobHeight = 0.5f;
    [SerializeField] private float bobDuration = 1f; // time to go up or down

    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up; // rotate around Y axis
    [SerializeField] private float rotationAmount = 180f; // degrees per half cycle

    private Vector3 startPos;
    private Quaternion startRot;
    private Coroutine loopCoroutine;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;

        StartLoop();
    }

    public void StartLoop()
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
        loopCoroutine = StartCoroutine(EffectLoop());
    }

    public void StopLoop()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }

        // reset to start
        transform.localPosition = startPos;
        transform.localRotation = startRot;
    }

    private IEnumerator EffectLoop()
    {
        while (true)
        {
            // --- UP phase ---
            float elapsed = 0f;
            while (elapsed < bobDuration)
            {
                float t = elapsed / bobDuration;

                if (enableBobbing)
                    transform.localPosition = startPos + Vector3.up * Mathf.Lerp(0, bobHeight, t);

                if (enableRotation)
                    transform.localRotation = startRot * Quaternion.AngleAxis(Mathf.Lerp(0, rotationAmount, t), rotationAxis);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // --- DOWN phase ---
            elapsed = 0f;
            while (elapsed < bobDuration)
            {
                float t = elapsed / bobDuration;

                if (enableBobbing)
                    transform.localPosition = startPos + Vector3.up * Mathf.Lerp(bobHeight, 0, t);

                if (enableRotation)
                    transform.localRotation = startRot * Quaternion.AngleAxis(rotationAmount + Mathf.Lerp(0, rotationAmount, t), rotationAxis);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
