using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HudManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image hitMarker;
    [SerializeField] private Image speedLines;
    [SerializeField] private Image shieldIcon;
    [SerializeField] private TextMeshProUGUI ammoCountText;
    [SerializeField] private Image BoostRadial;
    [SerializeField] private Image BoostIcon;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private Image healthDamageIndicator;
    [SerializeField] private Image shieldDamageIndicator;
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private TextMeshProUGUI shieldAmountText;


    [Header("Settings")]
    [SerializeField] private float hitMarkerDuration = 0.2f;
    [SerializeField] private float healthShieldChangeTime = 0.5f;
    [SerializeField] private float damageIndicatorFadeTime = 0.5f;



    private Coroutine hitCoroutine;

    public void Start()
    {
        if (healthBar != null)
        {
            healthBar.enabled = true;
            healthBar.fillAmount = 0f;
        }

        if (shieldBar != null)
        {
            shieldBar.enabled = true;
            shieldBar.fillAmount = 0f;
        }

        if (healthAmountText != null)
        {
            healthAmountText.text = "";
        }

        if (shieldAmountText != null)
        {
            shieldAmountText.text = "";
        }

        if (hitMarker != null)
            {
                hitMarker.enabled = false;
            }

        if (speedLines != null)
        {
            speedLines.enabled = false;
            Color slc = speedLines.color;
            slc.a = 0f;
            speedLines.color = slc;
        }

        if (shieldIcon != null)
        {
            shieldIcon.enabled = false;
            Color sic = shieldIcon.color;
            sic.a = 0f;
            shieldIcon.color = sic;
        }

        if (BoostRadial != null)
        {
            BoostRadial.enabled = false;
            BoostRadial.fillAmount = 0f;
        }

        if (BoostIcon != null)
        {
            BoostIcon.enabled = true;
            BoostRadial.fillAmount = 1f;
        }

        if (healthDamageIndicator != null)
        {
            healthDamageIndicator.enabled = false;
            Color hdc = speedLines.color;
            hdc.a = 150f;
            speedLines.color = hdc;
        }

        if (shieldDamageIndicator != null)
        {
            shieldDamageIndicator.enabled = false;
            Color sdc = speedLines.color;
            sdc.a = 150f;
            speedLines.color = sdc;
        }

    }

    public void ShowHitMarker()
    {
        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitMarkerRoutine());
    }

    private IEnumerator HitMarkerRoutine()
    {
        hitMarker.enabled = true;

        yield return new WaitForSeconds(hitMarkerDuration);

        hitMarker.enabled = false;
        hitCoroutine = null;
    }

    private Coroutine dodgeHudCoroutine;
    public void ShowDodgeHud(float duration)
    {
        if (dodgeHudCoroutine != null)
            StopCoroutine(dodgeHudCoroutine);

        dodgeHudCoroutine = StartCoroutine(DodgeHudRoutine(duration));
    }

    private IEnumerator DodgeHudRoutine(float duration)
    {
        Debug.Log("ShowSpeedLines called");
        speedLines.enabled = true;
        shieldIcon.enabled = true;
        Color slc = speedLines.color;
        Color sic = shieldIcon.color;
        float startAlphaSlc = 0.3f;
        float startAlphaSic = 1f;
        float endAlpha = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            slc.a = Mathf.Lerp(startAlphaSlc, endAlpha, t);
            sic.a = Mathf.Lerp(startAlphaSic, endAlpha, t);
            speedLines.color = slc;
            shieldIcon.color = sic;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transparent at the end
        slc.a = endAlpha;
        speedLines.color = slc;
        speedLines.enabled = false;

        sic.a = endAlpha;
        shieldIcon.color = sic;
        shieldIcon.enabled = false;

        dodgeHudCoroutine = null;
    }

    public void UpdateAmmoCount(int maxAmmo, int currentAmmo)
    {
        if (ammoCountText != null)
        {
            ammoCountText.text = $"{maxAmmo} / {currentAmmo}";
        }
    }

    private Coroutine boostCoroutine;

    public void StartBoostFill(float duration = 3f)
    {
        if (boostCoroutine == null)
            StartCoroutine(BoostFillRoutine(duration));
    }

    private IEnumerator BoostFillRoutine(float duration)
    {
        if (BoostRadial == null || BoostIcon == null)
            yield break;

        // Reset states
        BoostRadial.fillAmount = 0f;
        BoostIcon.fillAmount = 0f;

        BoostRadial.enabled = true;
        BoostIcon.enabled = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float fillValue = Mathf.Lerp(0f, 1f, t);

            BoostRadial.fillAmount = fillValue;
            BoostIcon.fillAmount = fillValue;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully filled
        BoostRadial.fillAmount = 1f;
        BoostIcon.fillAmount = 1f;

        // Hide BoostRadial but keep BoostIcon full
        BoostRadial.enabled = false;
        BoostIcon.enabled = true;

        boostCoroutine = null;
    }

    public void UseBoost()
    {
        if (BoostRadial != null)
        {
            BoostRadial.fillAmount = 0f;
            BoostRadial.enabled = false;
        }

        if (BoostIcon != null)
        {
            BoostIcon.fillAmount = 0f;
            BoostIcon.enabled = false;
        }
    }

    private Coroutine healthCoroutine;
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar == null) return;

        float targetFill = Mathf.Clamp01(currentHealth / maxHealth);

        if (healthAmountText != null)
        {
            healthAmountText.text = $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
        }

        // Stop any running coroutine before starting a new one
        if (healthCoroutine != null)
            StopCoroutine(healthCoroutine);

        healthCoroutine = StartCoroutine(UpdateBarRoutine(healthBar, targetFill));
    }

    private Coroutine shieldCoroutine;
    public void UpdateShieldBar(float currentShield, float maxShield)
    {
        if (shieldBar == null) return;

        float targetFill = Mathf.Clamp01(currentShield / maxShield);
        
        if (shieldAmountText != null)
        {
            shieldAmountText.text = $"{Mathf.RoundToInt(currentShield)} / {Mathf.RoundToInt(maxShield)}";
        }

        // Stop any running coroutine before starting a new one
        if (shieldCoroutine != null)
            StopCoroutine(shieldCoroutine);

        shieldCoroutine = StartCoroutine(UpdateBarRoutine(shieldBar, targetFill));
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


    private Coroutine healthDamageCoroutine;
    public void ShowHealthDamage()
    {
        if (healthDamageIndicator == null) return;

        if (healthDamageCoroutine != null)
            StopCoroutine(healthDamageCoroutine);

        healthDamageCoroutine = StartCoroutine(HealthDamageIndicatorRoutine());
    }

    private IEnumerator HealthDamageIndicatorRoutine()
    {
        healthDamageIndicator.enabled = true;

        Color c = healthDamageIndicator.color;
        c.a = 150f / 255f;
        healthDamageIndicator.color = c;

        float elapsed = 0f;
        while (elapsed < damageIndicatorFadeTime)
        {
            float t = elapsed / damageIndicatorFadeTime;
            c.a = Mathf.Lerp(150f / 255f, 0f, t);
            healthDamageIndicator.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 0f;
        healthDamageIndicator.color = c;
        healthDamageIndicator.enabled = false;

        healthDamageCoroutine = null;
    }

    private Coroutine shieldDamageCoroutine;
    public void ShowShieldDamage()
    {
        if (shieldDamageIndicator == null) return;

        if (shieldDamageCoroutine != null)
            StopCoroutine(shieldDamageCoroutine);

        shieldDamageCoroutine = StartCoroutine(ShieldDamageIndicatorRoutine());
    }

    private IEnumerator ShieldDamageIndicatorRoutine()
    {
        shieldDamageIndicator.enabled = true;

        Color c = shieldDamageIndicator.color;
        c.a = 150f / 255f;
        shieldDamageIndicator.color = c;

        float elapsed = 0f;
        while (elapsed < damageIndicatorFadeTime)
        {
            float t = elapsed / damageIndicatorFadeTime;
            c.a = Mathf.Lerp(150f / 255f, 0f, t);
            shieldDamageIndicator.color = c;

            elapsed += Time.deltaTime;
            yield return null;
        }

        c.a = 0f;
        shieldDamageIndicator.color = c;
        shieldDamageIndicator.enabled = false;

        shieldDamageCoroutine = null;
    }
}
