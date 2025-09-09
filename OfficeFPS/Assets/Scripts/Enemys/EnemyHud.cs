using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EnemyHud : MonoBehaviour
{
    [Header("References")]
    private Transform cam;
    [SerializeField] private Image healthBarContainer;
    [SerializeField] private Image healthBar;

    [Header("Settings")]
    [SerializeField] private float healthShieldChangeTime = 0.5f;

    public void Start()
    {
        cam = Camera.main.transform;
        if (healthBarContainer != null)
        {
            healthBar.enabled = true;
        }
        if (healthBar != null)
        {
            healthBar.enabled = true;
            healthBar.fillAmount = 1f;
        }
    }

    private void LateUpdate()
    {
        // Make the canvas face the camera every frame
        transform.LookAt(transform.position + cam.forward);
    }

    private Coroutine healthCoroutine;
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar == null) return;

        float targetFill = Mathf.Clamp01(currentHealth / maxHealth);

        // Stop any running coroutine before starting a new one
        if (healthCoroutine != null)
            StopCoroutine(healthCoroutine);

        healthCoroutine = StartCoroutine(UpdateBarRoutine(healthBar, targetFill));
    }

    private IEnumerator UpdateBarRoutine(Image bar, float targetFill)
    {
        float elapsed = 0f;
        float startFill = bar.fillAmount;

        while (elapsed < healthShieldChangeTime)
        {
            float t = elapsed / healthShieldChangeTime;
            bar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final value
        bar.fillAmount = targetFill;
    }
}
