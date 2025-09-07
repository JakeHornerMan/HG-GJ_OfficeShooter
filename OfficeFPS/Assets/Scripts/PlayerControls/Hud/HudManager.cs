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

    [Header("Settings")]
    [SerializeField] private float hitMarkerDuration = 0.2f;


    private Coroutine hitCoroutine;

    public void Start()
    {
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

}
