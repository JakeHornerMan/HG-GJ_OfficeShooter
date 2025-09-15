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
    private System.Random rng = new System.Random();
    public ComboController comboController;

    [Header("Bullet Speed Settings")]
    [SerializeField] public float shootForce = 100f;
    [SerializeField] public float upwardForce = 0f;

    [Header("Gun Settings")]
    [SerializeField] public float bulletDamage = 30f;
    [SerializeField] public bool isLeftGun = false;
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
    private Queue<RGBSettings> magazineQueue;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        shooting = false;
        readyToShoot = true;
        reloading = false;
        ManageMagazine();
        RGBSettings nextBullet = magazineQueue.Peek();
        if (isLeftGun)
        {
            hudManager.UpdateAmmoCount1(magazineSize, bulletsLeft, nextBullet);
        }
        else
        {
            hudManager.UpdateAmmoCount2(magazineSize, bulletsLeft, nextBullet);
        }
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (isLeftGun)
        {
            if (Input.GetButtonDown("Fire1") && readyToShoot && !reloading && bulletsLeft > 0)
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire2") && readyToShoot && !reloading && bulletsLeft > 0)
            {
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && readyToShoot && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (isLeftGun)
        {
            if (Input.GetButtonDown("Fire1") && readyToShoot && bulletsLeft <= 0 && !reloading)
            {
                Reload();
            }
        }
        else
        { 
            if (Input.GetButtonDown("Fire2") && readyToShoot && bulletsLeft <= 0 && !reloading)
            {
                Reload();
            }
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
        RGBSettings firedBullet = magazineQueue.Dequeue();
        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        currentBullet.GetComponent<Bullet>()
            .SpawnBullet(
                "Player", firedBullet, this, null, Mathf.Round(bulletDamage * comboController.currentBonus), comboController.comboCount
            )
        ;
        playerSounds.PlayGunShotSound();

        //Add force to bullet
        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        rb.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        rb.AddForce(weaponCamera.transform.up * upwardForce, ForceMode.Impulse);

        bulletsLeft -= bulletsPerTap;
        bulletsShot += bulletsPerTap;


        RGBSettings nextBullet = RGBSettings.NONE;
        if (bulletsLeft > 0 && !reloading)
        {
           nextBullet = magazineQueue.Peek();
        }

        if (isLeftGun)
        {
            hudManager.UpdateAmmoCount1(magazineSize, bulletsLeft, nextBullet);
            hudManager.UseBullet();
        }
        else
        { 
            hudManager.UpdateAmmoCount2(magazineSize, bulletsLeft, nextBullet);
        }

        ShotReset(timeBetweenShooting);
    }

    private Coroutine shotRestCoroutine;

    private void ShotReset(float duration)
    {
        // Debug.Log($"Shot Reset called, readyToShoot: {readyToShoot}");
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
        // Debug.Log($"Shot Reset complete, readyToShoot: {readyToShoot}");
    }

    public void SuccessfulHitEnemy()
    {
        hudManager.ShowHitMarker();
        comboController.PlusUpdateCombo();
    }

    public void UnsuccessfulHitEnemy()
    {
        hudManager.ShowHitMarker();
        comboController.MinusUpdateCombo();
    }

    private Coroutine reloadCoroutine;
    public void Reload()
    {
        // Debug.Log($"Relaod called, readyToShoot: {readyToShoot}, bulletsLeft: {bulletsLeft}, reloading: {reloading}");
        if (reloadCoroutine == null && readyToShoot == true && reloading == false)
        {
            if (isLeftGun)
            {
                hudManager.SetColorAndAmmoCountForReloading1();
            }
            else
            { 
                hudManager.SetColorAndAmmoCountForReloading2();
            }
            reloadCoroutine = StartCoroutine(ReloadRoutine(reloadTime));
            ManageMagazine();
        }
    }

    private IEnumerator ReloadRoutine(float duration)
    {
        reloading = true;
        readyToShoot = false;
        // Debug.Log($"Reload started, reloading: {reloading}, readyToShoot: {readyToShoot}");

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
        RGBSettings nextBullet = magazineQueue.Peek();

        if (isLeftGun)
        {
            hudManager.UpdateAmmoCount1(magazineSize, bulletsLeft, nextBullet);
        }
        else
        { 
            hudManager.UpdateAmmoCount2(magazineSize, bulletsLeft, nextBullet);
        }

        // Debug.Log($"Reload complete, readyToShoot: {readyToShoot}");
    }

    private void ManageMagazine()
    {
        magazineQueue = new Queue<RGBSettings>();

        int baseCount = magazineSize / 3;     // How many of each color
        int remainder = magazineSize % 3;     // Extra bullets to fill with randoms

        List<RGBSettings> tempList = new List<RGBSettings>();

        // Add base bullets
        for (int i = 0; i < baseCount; i++)
        {
            tempList.Add(RGBSettings.RED);
            tempList.Add(RGBSettings.GREEN);
            tempList.Add(RGBSettings.BLUE);
        }

        // Add remainder bullets randomly
        for (int i = 0; i < remainder; i++)
        {
            RGBSettings randomColor = (RGBSettings)rng.Next(0, 3); // 0=RED, 1=GREEN, 2=BLUE
            tempList.Add(randomColor);
        }

        // Shuffle the list before turning it into a queue
        ShuffleList(tempList);

        // Fill the queue
        foreach (var bullet in tempList)
        {
            magazineQueue.Enqueue(bullet);
        }
        
        hudManager.LoadMagazine(tempList);

        Debug.Log($"[ManageMagazine] New magazine created with {magazineSize} bullets: {string.Join(", ", magazineQueue)}");
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
