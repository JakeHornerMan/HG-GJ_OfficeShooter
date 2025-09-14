using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("References")]
    public Transform attackPoint;
    public LayerMask whatIsPlayer;
    public EnemyBehaviour enemyBehaviour;

    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public float damage;
    public float shootForce = 30f;
    public float spread = 1f;
    public float upwardsSpread = 0.1f;

    public void Awake()
    { 
        enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    public void Attack(Vector3 targetPoint)
    {
        //Calculate Direction
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Claculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-upwardsSpread, upwardsSpread);

        //Direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position, Quaternion.identity);
        currentBullet.GetComponent<Bullet>().SpawnBullet(gameObject.tag, RGBSettings.BLUE, null, this, damage); // Set the owner tag for the bullet
        // playerSounds.PlayGunShotSound();

        //Add force to bullet
        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        rb.AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);

        Debug.Log($"[EnemyCombat] : Attack | Shot fired towards {targetPoint}, spread ({x}, {y})");
    }

    public void DidNotHitPlayer()
    {
        enemyBehaviour.isHunting = !enemyBehaviour.FindPlayer();
        Debug.Log($"[EnemyCombat] : DidNotHitPlayer | isHunting: {enemyBehaviour.isHunting}");
    }
}
