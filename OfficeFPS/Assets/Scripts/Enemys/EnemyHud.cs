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

    public void SetColorHealthBar(RGBSettings enemyType)
    {
        if (healthBar != null)
        {
            switch (enemyType)
            {
                case RGBSettings.RED:
                    healthBar.color = Color.red;
                    break;
                case RGBSettings.GREEN:
                    healthBar.color = Color.green;
                    break;
                case RGBSettings.BLUE:
                    healthBar.color = Color.blue;
                    break;
                default:
                    healthBar.color = Color.black;
                    break;
            }
        }
    }

    public void HideHealthBar()
    {
        if (healthBarContainer != null)
        {
            healthBarContainer.enabled = false;
        }
        if (healthBar != null)
        {
            healthBar.enabled = false;
        }
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
