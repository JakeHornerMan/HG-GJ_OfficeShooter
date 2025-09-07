using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image hitMarker;
    [SerializeField] private Image speedLines;
    [SerializeField] private Image shieldIcon;

    [Header("Settings")]
    [SerializeField] private float hitMarkerDuration = 0.2f;
    [SerializeField] private float speedLinesDuration = 0.25f;
    

    private Coroutine hitCoroutine;

    public void Start()
    {
        if (hitMarker != null)
            hitMarker.enabled = false;

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
    public void ShowDodgeHud()
    {
        if (dodgeHudCoroutine != null)
            StopCoroutine(dodgeHudCoroutine);

        dodgeHudCoroutine = StartCoroutine(DodgeHudRoutine());
    }

    private IEnumerator DodgeHudRoutine()
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

        while (elapsed < speedLinesDuration)
        {
            float t = elapsed / speedLinesDuration;
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
}
