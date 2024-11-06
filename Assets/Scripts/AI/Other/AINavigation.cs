using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    #region VARIABLES
    public AIManager aIManager;
    public Transform player;
    private NavMeshAgent agent;
    private Attributes attributes;

    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Default")]
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    [Header("Attack")]
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public GameObject attackObject;
    public float attackRange;
    public float damageAmount; // NOT USED YET
    public bool playerInAttackRange;
    public GameObject attackPoint;

    [Header("Sight")]
    public float sightRange;
    public bool playerInSightRange;
    public AIVision aIVision;

    [Header("Other Variables")]
    private int stateInt;
    #endregion


    public AIState aIState;

    public enum AIState
    {
        DEFAULT,
        AGGRESSIVE,
        STEALTH,
        HIDE,
        ATTACK,
        SEARCH,
        ROAM,
        CHASE
    }

    private void Awake()
    {
        aIManager = GameObject.FindGameObjectWithTag("AIManager").GetComponent<AIManager>();
        player = aIManager.player.transform;
        agent = GetComponent<NavMeshAgent>();
        attributes = GetComponent<Attributes>();
        aIVision = GetComponentInChildren<AIVision>();
    }

    // Update is called once per frame
    void Update()
    {
        Checks();
        StateHandler();
    }

    private void Checks()
    {
        // Checking for sight and attack range for player layer
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }

    private void StateHandler()
    {
        //if (!aIVision.IsInSight(aIManager.player) && !playerInSightRange && !playerInAttackRange) Default();
        //if (aIVision.IsInSight(aIManager.player) && playerInSightRange && !playerInAttackRange) Chase();
        //if (aIVision.IsInSight(aIManager.player) && playerInSightRange && playerInAttackRange) Attack();

        if (!playerInSightRange && !playerInAttackRange) Default();
        if (playerInSightRange && !playerInAttackRange) Chase();
        if (playerInSightRange && playerInAttackRange) Attack();


    }

    #region STATES
    private void Default()
    {
        aIState = AIState.DEFAULT;
        stateInt = (int)aIState;
        aIManager.stateIndicator.ChangeIndicatorMaterial(stateInt);

        if (!walkPointSet) SearchWalkPoint();

        if(walkPointSet)
        {
            agent.destination = walkPoint;
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void Aggressive()
    {

    }
    private void Stealth()
    {

    }
    private void Hide()
    {

    }
    private void Attack()
    {
        aIState = AIState.ATTACK;
        stateInt = (int)aIState;
        aIManager.stateIndicator.ChangeIndicatorMaterial(stateInt);

        // First stop AI movement
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            // ATTACK CODE HERE
            Instantiate(attackObject, attackPoint.transform.position, Quaternion.identity);


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void Search()
    {

    }
    private void Roam()
    {

    }
    private void Chase()
    {
        aIState = AIState.CHASE;
        stateInt = (int)aIState;
        aIManager.stateIndicator.ChangeIndicatorMaterial(stateInt);

        agent.SetDestination(player.position);
    }
    #endregion

    #region SUPPORTING FUNCTIONS
    private void SearchWalkPoint()
    {
        // Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Checking if walkpoint is on ground and not out of map! Also this maybe needs to be modified or groundLayer modified to be only on ground and not objects as well
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
    
    // Call this from Attributes?
    private void Die()
    {
        Debug.Log("I am dead!");
        Destroy(gameObject);
    }

    // Visualizing the sight and atk range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
    #endregion
}
