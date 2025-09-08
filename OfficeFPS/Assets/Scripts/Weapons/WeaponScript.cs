using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class WeaponScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public GameObject playerGun;
    public HudManager hudManager;
    public Transform attackPoint;
    public Camera weaponCamera;
    public PlayerSounds playerSounds;

    [Header("Bullet Speed Settings")]
    [SerializeField] public float shootForce = 100f;
    [SerializeField] public float upwardForce = 0f;

    [Header("Gun Settings")]
    [SerializeField] public float timeBetweenShooting = 0.3f;
    [SerializeField] public float spread = 0.2f;
    [SerializeField] public float reloadTime = 1f;
    [SerializeField] public float timeBetweenBullets = 0.1f;
    [SerializeField] public int magazineSize = 8;
    [SerializeField] public int bulletsPerTap = 1;

    [Header("Reload Animation Settings")]
    [SerializeField] public float reloadRotations = 3f; // Number of full rotations during reload
    [SerializeField] public float reloadBobHeight = 0.5f; // Height of the bob during reload
    [SerializeField] public float kickBackRotation = 45f; // How much the gun kicks back on shoot

    [Header("Gun Flags")]
    public int bulletsLeft;
    public int bulletsShot;
    public bool shooting;
    public bool readyToShoot;
    public bool reloading;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        hudManager.UpdateAmmoCount(magazineSize, bulletsLeft);
    }

    private void Update()
    {
        MyInput();
    }
    private void MyInput()
    {
        if (Input.GetButtonDown("Fire1") && readyToShoot && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && readyToShoot && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (Input.GetButtonDown("Fire1") && readyToShoot && bulletsLeft <= 0 && !reloading)
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (!readyToShoot && !reloading && bulletsLeft > 0) return;

        readyToShoot = false;

        Ray ray = weaponCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100); // Just a point far away from the player
        }

        // Direction from attach point without spread
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Claculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //Instantiate bullet
        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        playerSounds.PlayGunShotSound();

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(weaponCamera.transform.up * upwardForce, ForceMode.Impulse);

        bulletsLeft -= bulletsPerTap;
        bulletsShot += bulletsPerTap;

        hudManager.UpdateAmmoCount(magazineSize, bulletsLeft);

        ShotReset(timeBetweenShooting);
    }

    private Coroutine shotRestCoroutine;

    private void ShotReset(float duration)
    {
        Debug.Log($"Shot Reset called, readyToShoot: {readyToShoot}");
        if (shotRestCoroutine == null && readyToShoot == false)
            shotRestCoroutine = StartCoroutine(ShotResetRoutine(timeBetweenShooting));
    }

    private IEnumerator ShotResetRoutine(float duration)
    {

        Quaternion startRot = playerGun.transform.localRotation;
        Quaternion kickRot = startRot * Quaternion.Euler(0f, 0f, kickBackRotation);

        float elapsed = 0f;

        // 🔄 Smoothly rotate towards 35f Z over half the duration
        while (elapsed < duration * 0.3f)
        {
            float t = elapsed / (duration * 0.5f);
            playerGun.transform.localRotation = Quaternion.Slerp(startRot, kickRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 🔄 Smoothly rotate back to original rotation over the second half
        elapsed = 0f;
        while (elapsed < duration * 0.7f)
        {
            float t = elapsed / (duration * 0.5f);
            playerGun.transform.localRotation = Quaternion.Slerp(kickRot, startRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure exact reset
        playerGun.transform.localRotation = startRot;

        readyToShoot = true;
        shotRestCoroutine = null;
        Debug.Log($"Shot Reset complete, readyToShoot: {readyToShoot}");
    }

    private Coroutine reloadCoroutine;
    public void Reload()
    {
        Debug.Log($"Relaod called, readyToShoot: {readyToShoot}, bulletsLeft: {bulletsLeft}, reloading: {reloading}");
        if (reloadCoroutine == null && readyToShoot == true && reloading == false)
            reloadCoroutine = StartCoroutine(ReloadRoutine(reloadTime));
    }

    private IEnumerator ReloadRoutine(float duration)
    {
        reloading = true;
        readyToShoot = false;
        Debug.Log($"Reload started, reloading: {reloading}, readyToShoot: {readyToShoot}");

        float elapsed = 0f;
        Quaternion startRot = playerGun.transform.localRotation;
        Vector3 startPos = playerGun.transform.localPosition;
        Vector3 upPos = startPos + new Vector3(0f, reloadBobHeight, 0f); // raise by 0.1f
        playerSounds.PlayReloadSound();

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // 🔄 Smooth spin (3 rotations on Z)
            float angle = Mathf.Lerp(0f, 360f * reloadRotations, t);
            playerGun.transform.localRotation = startRot * Quaternion.Euler(0f, 0f, angle);

            // ⬆️⬇️ Bob up and down using a sine wave (goes up at halfway, back down by end)
            float bob = Mathf.Sin(t * Mathf.PI); // goes 0 → 1 → 0
            playerGun.transform.localPosition = Vector3.Lerp(startPos, upPos, bob);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset transform values
        playerGun.transform.localRotation = startRot;
        playerGun.transform.localPosition = startPos;
        playerSounds.PlayReloadFinishedSound();

        bulletsLeft = magazineSize;
        reloading = false;
        reloadCoroutine = null;
        readyToShoot = true;
        hudManager.UpdateAmmoCount(magazineSize, bulletsLeft);
        Debug.Log($"Reload complete, readyToShoot: {readyToShoot}");
    }
}
