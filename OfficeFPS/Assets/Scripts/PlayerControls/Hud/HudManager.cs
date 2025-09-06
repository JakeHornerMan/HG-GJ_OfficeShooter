using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image hitMarker;

    [Header("Settings")]
    [SerializeField] private float hitMarkerDuration = 0.2f;

    private Coroutine hitCoroutine;

    /// <summary>
    /// Call this method to show the hit marker on screen.
    /// </summary>
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
}
