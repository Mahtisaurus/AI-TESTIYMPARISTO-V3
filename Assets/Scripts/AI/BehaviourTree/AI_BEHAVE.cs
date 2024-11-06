using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTrees;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class AI_BEHAVE : MonoBehaviour
{
    [SerializeField] List<Transform> searchPoints = new();
    [SerializeField] List<Transform> waypoints = new();
    [SerializeField] GameObject pickable;
    [SerializeField] GameObject pickable2;

    [SerializeField] GameObject soundSource;
    [SerializeField] GameObject hidingSpot;

    [SerializeField] Transform tempTransform;

    [Header("Checks for Strategies")]
    [SerializeField] bool heardNoise;
    [SerializeField] bool canSearchPlayer;
    [SerializeField] bool canChase;
    [SerializeField] bool canAttack;
    [SerializeField] bool canHide;

    [Header("Attack")]
    public float timeBetweenAttacks;
    private bool alreadyAttacked;
    public GameObject attackObject;
    public float attackRange;
    public float damageAmount; // NOT USED YET
    public GameObject attackPoint;

    [Header("Other")]
    public LayerMask whatIsGround;

    NavMeshAgent agent;
    BehaviourTree tree;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree("ALIEN");



        PrioritySelector actions = new PrioritySelector("AI_LOGIC");


        // DEFAULT BEHAVIOR, Priority = 0
        Leaf wander = new Leaf("PATROL", new PatrolStrategy(transform, agent, waypoints));

        actions.AddChild(wander);


        // SEARCH for Sound, Priority = 50
        Sequence searchSound = new Sequence("SEARCH_SOUND", 50);

        bool HeardSound()
        {
            if (!heardNoise)
            {
                searchSound.Reset();
                return false;
            }

            return true;
        }

        searchSound.AddChild(new Leaf("Sound Heard?", new Condition(HeardSound)));
        searchSound.AddChild(new Leaf("Go To Sound", new MoveToTarget(transform, agent, soundSource.transform, 4f)));
        searchSound.AddChild(new Leaf("Search Area", new SearchStrategy(transform, agent, whatIsGround)));

        actions.AddChild(searchSound);


        // SEARCH for Player when seen recently, Priority = 60
        Sequence searchPlayer = new Sequence("SEARCH_PLAYER", 60);

        bool SearchPlayer()
        {
            if (!canSearchPlayer)
            {
                searchPlayer.Reset();
                return false;
            }

            return true;
        }

        searchPlayer.AddChild(new Leaf("Can Search?", new Condition(SearchPlayer)));
        searchPlayer.AddChild(new Leaf("Go To Recent Player Location", new MoveToTarget(transform, agent, AIManager.Instance.lastSeenPlayerLocation, 4f)));
        searchPlayer.AddChild(new Leaf("Search Area", new SearchStrategy(transform, agent, whatIsGround)));

        actions.AddChild(searchPlayer);


        // CHASE Player when see player and alerted, Priority = 90
        Sequence chasePlayer = new Sequence("CHASE_PLAYER", 90);

        bool CanChase()
        {
            if (!canChase)
            {
                chasePlayer.Reset();
                return false;
            }

            return true;
        }

        chasePlayer.AddChild(new Leaf("Can Chase?", new Condition(CanChase)));
        chasePlayer.AddChild(new Leaf("Start Chasing Player", new ChaseStrategy(transform, agent, AIManager.Instance.player.transform)));

        actions.AddChild(chasePlayer);


        // ATTACK Player when in range and allowed to, Priority = 100
        Sequence attackPlayer = new Sequence("ATTACK_PLAYER", 100);

        bool CanAttack()
        {
            if (!canAttack)
            {
                attackPlayer.Reset();
                return false;
            }

            return true;
        }

        attackPlayer.AddChild(new Leaf("Can Attack?", new Condition(CanAttack)));
        attackPlayer.AddChild(new Leaf("Perform Attack", new AttackStrategy(() => Attack(), transform, AIManager.Instance.player.transform)));

        actions.AddChild(attackPlayer);


        // HIDE for an ambush when can hide, Priority = 10
        Sequence hide = new Sequence("HIDE", 10);

        bool CanHide()
        {
            if (!canHide)
            {
                hide.Reset();
                return false;
            }

            return true;
        }

        hide.AddChild(new Leaf("Can Hide?", new Condition(CanHide)));
        hide.AddChild(new Leaf("Go To Hiding Spot", new MoveToTarget(transform, agent, hidingSpot.transform, 3f)));

        actions.AddChild(hide);



        tree.AddChild(actions);

        tree.PrintTree();
    }

    void Update()
    {
        tree.Process();
        CheckIfAbleToAttack();
    }

    #region ACTIONS
    private void Attack()
    {
        if (!alreadyAttacked)
        {
            // ATTACK CODE HERE
            Instantiate(attackObject, attackPoint.transform.position, Quaternion.identity);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    #endregion

    #region CHECKS

    public void CanSearchPlayer(bool input)
    {
        canSearchPlayer = input;
    }
    public void CanChase(bool input)
    {
        canChase = input;
    }
    public void CanAttack(bool input)
    {
        canAttack = input;
    }
    public void CanHide(bool input)
    {
        canHide = input;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void CheckIfAbleToAttack()
    {
        float distance = AIManager.Instance.distanceToPlayer;

        if(canChase && distance <= attackRange && !alreadyAttacked)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }

    #endregion

}