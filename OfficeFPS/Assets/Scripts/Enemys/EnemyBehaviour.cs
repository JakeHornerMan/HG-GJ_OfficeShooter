using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;
    public EnemyCombat enemyCombat;

    [Header("Patroling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Attacking")]
    public bool stopAndAttack = false;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    [Header("States")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Hunt")]
    public Vector3 lastKnownPosition;
    public bool lastPositionSet = false;
    public bool isHunting = false;
    public float sightRangeHunting;
    public float huntPlayerCheckTime = 5f;

    private void Awake()
    {
        sightRangeHunting = sightRange * 2;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyCombat = GetComponent<EnemyCombat>();
        Debug.Log($"[EnemyBehaviour] : Awake | Found player: {player.name}, Agent assigned: {agent != null}");
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange & isHunting)
        {
            playerInSightRange = FindPlayer();    
            Debug.Log($"[EnemyBehaviour] : isHunting | LongRangeChase playerInSightRange = {playerInSightRange}");
        }

        Debug.Log($"[EnemyBehaviour] : Update | sightRange: {playerInSightRange}, attackRange: {playerInAttackRange}");

        if (isHunting)
        {
            Hunting();
            return;
        }

        if (!playerInSightRange && !playerInAttackRange && !isHunting)
            Patroling();
        if (playerInSightRange && !playerInAttackRange && !isHunting)
            ChasePlayer();
        if (playerInAttackRange && playerInSightRange && !isHunting)
            AttackPlayer();
    }

    #region Patroling
    private void Patroling()
    {
        Debug.Log($"[EnemyBehaviour] : Patroling | walkPointSet: {walkPointSet}, currentWalkPoint: {walkPoint}");

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            Debug.Log("[EnemyBehaviour] : Patroling | Reached walkPoint");
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        Debug.Log("[EnemyBehaviour] : SearchWalkPoint | Searching new point");

        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
            Debug.Log($"[EnemyBehaviour] : SearchWalkPoint | New walkPoint set: {walkPoint}");
        }
        else
        {
            Debug.Log("[EnemyBehaviour] : SearchWalkPoint | Invalid walkPoint, retry needed");
        }
    }

    #endregion
    private void ChasePlayer()
    {
        Debug.Log($"[EnemyBehaviour] : ChasePlayer | Chasing player at {player.position}");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        Debug.Log($"[EnemyBehaviour] : AttackPlayer | stopAndAttack: {stopAndAttack}, alreadyAttacked: {alreadyAttacked}");

        if (stopAndAttack)
        {
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.SetDestination(player.position);
        }

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Vector3 targetPoint = player.position;
            targetPoint.y = 1.5f;

            enemyCombat.Attack(targetPoint);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        Debug.Log("[EnemyBehaviour] : ResetAttack | Attack cooldown reset");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Debug.Log("[EnemyBehaviour] : OnDrawGizmosSelected | Gizmos drawn");
    }

    public bool FindPlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f; // lift ray slightly so it's not at ground level
        Vector3 direction = (player.position - origin).normalized;

        Debug.DrawRay(origin, direction * sightRangeHunting, Color.cyan, 1f);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, sightRangeHunting))
        {
            Debug.Log($"[EnemyBehaviour] : FindPlayer | Ray hit {hit.collider.gameObject.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)} at distance {hit.distance}");

            if (((1 << hit.collider.gameObject.layer) & whatIsPlayer) != 0)
            {
                Debug.Log("[EnemyBehaviour] : FindPlayer | Player detected by raycast!");
                return true;
            }
            Debug.Log("[EnemyBehaviour] : FindPlayer | Can't see Player");
        }
        return false;
    }

    public void Hunting()
    {
        if (!lastPositionSet)
        {
            lastKnownPosition = player.position;
            lastPositionSet = true;
            HuntingPlayerLook();
        }
        if (lastPositionSet)
            agent.SetDestination(lastKnownPosition);

        Vector3 distanceToLastKnownPosition = transform.position - lastKnownPosition;

        //Walkpoint reached
        if (distanceToLastKnownPosition.magnitude < 1f)
        {
            Debug.Log("[EnemyBehaviour] : Patroling | Reached walkPoint");
            isHunting = false;
            bool didNotFindPlayer = FindPlayer();
            lastPositionSet = false;
            lastKnownPosition = Vector3.zero;
        }

    }

    private Coroutine huntLookCoroutine;
    public void HuntingPlayerLook()
    {
        Debug.Log($"[EnemyBehaviour] HuntingPlayerLook | isHunting: {isHunting}");

        if (!isHunting)
        {
            return;
        }
        huntLookCoroutine = StartCoroutine(LookForPlayerRoutine());
    }

    private IEnumerator LookForPlayerRoutine()
    {
        yield return new WaitForSeconds(huntPlayerCheckTime);
        if (!FindPlayer())
            HuntingPlayerLook();
    }

}
