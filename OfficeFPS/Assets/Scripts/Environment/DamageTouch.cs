using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class DamageTouch : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 20;       // Default damage per tick
    [SerializeField] private float damageInterval = 3f;   // How often to deal damage (in seconds)

    private Dictionary<GameObject, Coroutine> activeDamageRoutines = new Dictionary<GameObject, Coroutine>();

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[DamageTouch] Collision with {collision.gameObject.name}");
        TryStartDamage(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject obj = collision.gameObject;
        if (activeDamageRoutines.ContainsKey(obj))
        {
            Debug.Log($"[DamageTouch] {obj.name} exited {gameObject.name}, stopping damage routine.");
            StopCoroutine(activeDamageRoutines[obj]);
            activeDamageRoutines.Remove(obj);
        }
    }

    private void TryStartDamage(GameObject obj)
    {
        if (activeDamageRoutines.ContainsKey(obj))
            return; // Already damaging this one

        if (obj.CompareTag("Player"))
        {
            HealthShieldSystem health = obj.GetComponentInParent<HealthShieldSystem>();
            if (health != null)
            {
                Debug.Log($"[DamageTouch] Player {obj.name} found on {gameObject.name}, starting damage routine.");
                Coroutine routine = StartCoroutine(DamageRoutinePlayer(health, obj));
                activeDamageRoutines[obj] = routine;
            }
            else
            {
                Debug.LogWarning($"[DamageTouch] Player {obj.name} touched {gameObject.name}, but no HealthShieldSystem was found!");
            }
        }
        // else if (obj.CompareTag("Enemy"))
        // {
        //     EnemyHealthSystem enemyHealth = obj.GetComponentInParent<EnemyHealthSystem>();
        //     if (enemyHealth != null)
        //     {
        //         Debug.Log($"[DamageTouch] Enemy {obj.name} found on {gameObject.name}, starting damage routine.");
        //         Coroutine routine = StartCoroutine(DamageRoutineEnemy(enemyHealth, obj));
        //         activeDamageRoutines[obj] = routine;
        //     }
        //     else
        //     {
        //         Debug.LogWarning($"[DamageTouch] Enemy {obj.name} touched {gameObject.name}, but no EnemyHealthSystem was found!");
        //     }
        // }
        else
        {
            Debug.Log($"[DamageTouch] {obj.name} is not tagged as Player or Enemy.");
        }
    }

    private IEnumerator DamageRoutinePlayer(HealthShieldSystem health, GameObject obj)
    {
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);
            Debug.Log($"[DamageTouch] Dealing {damageAmount} damage to PLAYER ({obj.name}) at time {Time.time:F2}");
            health.TakeDamage(damageAmount, true); // ignore invincibility
        }
    }

    // private IEnumerator DamageRoutineEnemy(EnemyHealthSystem enemyHealth, GameObject obj)
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(damageInterval);
    //         Debug.Log($"[DamageTouch] Dealing {damageAmount} damage to ENEMY ({obj.name}) at time {Time.time:F2}");
    //         enemyHealth.TakeDamage(damageAmount);
    //     }
    // }
}
