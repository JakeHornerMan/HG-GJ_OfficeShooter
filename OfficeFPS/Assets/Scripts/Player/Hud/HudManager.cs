using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class HudManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image hitMarker;
    [SerializeField] private Image speedLines;
    [SerializeField] private Image shieldIcon;
    [SerializeField] private TextMeshProUGUI ammoCountText1;

    [SerializeField] private Image leftAmmoIndicator;
    [SerializeField] private TextMeshProUGUI ammoCountText2;

    [SerializeField] private Image rightAmmoIndicator;
    [SerializeField] private Image BoostRadial;
    [SerializeField] private Image BoostIcon;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private Image healthDamageIndicator;
    [SerializeField] private Image shieldDamageIndicator;
    [SerializeField] private TextMeshProUGUI healthAmountText;
    [SerializeField] private TextMeshProUGUI shieldAmountText;

    [SerializeField] public GameObject ammoIconPrefab; 

    [SerializeField] public Transform magazineUIParent;
    private Queue<RGBSettings> magazineQueue = new Queue<RGBSettings>();
    private List<Image> activeIcons = new List<Image>();

    // [SerializeField] public Transform magazineUIParent2;
    // private Queue<RGBSettings> magazineQueue2 = new Queue<RGBSettings>();
    // private List<Image> activeIcons2 = new List<Image>();


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
            Color hmc = hitMarker.color;
            hmc.a = 0f;
            hitMarker.color = hmc;
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
        Color hmc = hitMarker.color;
        hmc.a = 1f;
        hitMarker.color = hmc;

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

    public void UpdateAmmoCount1(int maxAmmo, int currentAmmo, RGBSettings bulletType)
    {
        if (ammoCountText1 != null)
        {
            ammoCountText1.text = $"{maxAmmo} / {currentAmmo}";
            SetColorAmmoCount(bulletType, ammoCountText1, leftAmmoIndicator);
        }
    }

    public void UpdateAmmoCount2(int maxAmmo, int currentAmmo, RGBSettings bulletType)
    {
        if (ammoCountText2 != null)
        {
            ammoCountText2.text = $"{maxAmmo} / {currentAmmo}";
            SetColorAmmoCount(bulletType, ammoCountText2, rightAmmoIndicator);
        }
    }

    public void SetColorAmmoCount(RGBSettings bulletType, TextMeshProUGUI ammoCountText, Image ammoIndicator)
    {
        if (ammoCountText != null)
        {
            ammoIndicator.enabled = true;
            Color targetColor;

            switch (bulletType)
            {
                case RGBSettings.RED:
                    targetColor = Color.red;
                    break;
                case RGBSettings.GREEN:
                    targetColor = Color.green;
                    break;
                case RGBSettings.BLUE:
                    targetColor = Color.blue;
                    break;
                default:
                    targetColor = Color.white;
                    ammoIndicator.enabled = false;
                    break;
            }

            // Apply to text (full alpha)
            ammoCountText.color = targetColor;

            // Apply to indicator (alpha 150 / 255)
            if (ammoIndicator.enabled)
            {
                Color indicatorColor = targetColor;
                indicatorColor.a = 150f / 255f; // ~0.59
                ammoIndicator.color = indicatorColor;
            }
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

    public void LoadMagazine(IEnumerable<RGBSettings> bullets)
    {
        // Clear old
        magazineQueue.Clear();
        foreach (var icon in activeIcons)
            Destroy(icon.gameObject);
        activeIcons.Clear();

        // Load new
        foreach (var bullet in bullets)
        {
            magazineQueue.Enqueue(bullet);
            SpawnIcon(bullet);
        }
    }

    private void SpawnIcon(RGBSettings bulletType)
    {
        GameObject iconGO = Instantiate(ammoIconPrefab, magazineUIParent);
        Image img = iconGO.GetComponent<Image>();

        // Color based on bullet type
        switch (bulletType)
        {
            case RGBSettings.RED: img.color = new Color(1f, 0f, 0f, 0.6f); break;
            case RGBSettings.GREEN: img.color = new Color(0f, 1f, 0f, 0.6f); break;
            case RGBSettings.BLUE: img.color = new Color(0f, 0f, 1f, 0.6f); break;
        }

        activeIcons.Add(img);
    }

    public void UseBullet()
    {
        if (magazineQueue.Count == 0) return;

        // Remove from queue
        magazineQueue.Dequeue();

        // Remove first icon in UI
        if (activeIcons.Count > 0)
        {
            Image firstIcon = activeIcons[0];
            activeIcons.RemoveAt(0);
            Destroy(firstIcon.gameObject);
        }
    }
}
