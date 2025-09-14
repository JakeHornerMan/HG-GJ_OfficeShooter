using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private GameObject enemyContainer;
    [SerializeField] private float respawnDelay = 15f;
    [SerializeField] private Transform spawnPoint;

    void Awake()
    {
        enemyContainer = GameObject.Find("EnemyContainer");
        if (enemyContainer == null)
        {
            Debug.LogError("[Spawner] Could not find EnemyContainer in scene!");
        }
        else
        {
            Debug.Log($"[Spawner] Found EnemyContainer with {enemyContainer.transform.childCount} children");
        }
    }

    void Start()
    {
        StartRespawn();
    }

    private void StartRespawn()
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (enemyContainer == null)
        {
            Debug.LogWarning("[Spawner] No EnemyContainer assigned, cannot respawn.");
            yield break;
        }

        List<GameObject> inactiveEnemies = new List<GameObject>();

        foreach (Transform child in enemyContainer.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                inactiveEnemies.Add(child.gameObject);
            }
        }

        Debug.Log($"[Spawner] Found {inactiveEnemies.Count} inactive enemies after {respawnDelay} seconds.");

        if (inactiveEnemies.Count > 0)
        {
            GameObject enemyToRespawn = inactiveEnemies[Random.Range(0, inactiveEnemies.Count)];

            // move to spawnPoint if assigned
            if (spawnPoint != null)
                enemyToRespawn.transform.position = spawnPoint.position;

            enemyToRespawn.SetActive(true);

            EnemyHealth health = enemyToRespawn.GetComponent<EnemyHealth>();
            if (health != null)
            {
                RGBSettings randomType = GetRandomRGB();
                health.AwakeEnemy(randomType);
                Debug.Log($"[Spawner] Respawned {enemyToRespawn.name} as {randomType} at {enemyToRespawn.transform.position}");
            }
            else
            {
                Debug.LogWarning($"[Spawner] Respawned {enemyToRespawn.name} but no EnemyHealth component found!");
            }
        }

        // keep looping
        StartRespawn();
    }

    private RGBSettings GetRandomRGB()
    {
        int random = Random.Range(0, 3); // RED, GREEN, BLUE only
        return (RGBSettings)random;
    }
}
