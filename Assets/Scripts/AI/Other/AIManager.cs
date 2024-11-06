using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    #region VARIABLES
    public static AIManager Instance { get; private set; }

    [Header("Dynamic objects")]
    public GameObject player;
    public GameObject enemy;
    public Transform lastSeenPlayerLocation;
    public float distanceToPlayer;
    public float hidingInterval = 60f;
    public float hidingTimer = 0f;
    public float hidingDuration = 20f;
    public bool enemyIsHiding = false;
    

    //[Header("Player Statistics")]
    //public int playerRoomNumber;
    //public Transform playerRoomTransform;

    //[Header("Enemy Statistics")]
    //public int enemyRoomNumber;
    //public Transform enemyRoomTransform;

    [Header("Other Managers")]
    public AIRoomInfoManager aIRoomInfoManager;

    [Header("Enemy AI")]
    //public AINavigation aINavigation;
    public StateIndicatorMaterialChanger stateIndicator;
    public AIAggression aIAggression;
    public AIVision aIVision;

    [Header("AI Info Texts")]
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //aINavigation = enemy.GetComponent<AINavigation>();
        aIAggression = gameObject.GetComponent<AIAggression>();
        aIVision = enemy.GetComponentInChildren<AIVision>();
    }

    private void Update()
    {
        //text1.text = aINavigation.aIState.ToString();
        text2.text = enemy.GetComponent<NavMeshAgent>().speed.ToString();
        text3.text = enemy.GetComponent<NavMeshAgent>().destination.ToString();
        text4.text = aIAggression.aggressionLevel.ToString();

        LastSeenPlayerLocation(aIVision.canSeePlayer);
        distanceToPlayer = Vector3.Distance(player.transform.position, enemy.transform.position);
        HidingTimer();
    }


    #region SUPPORTING FUNCTIONS

    // While AI can see the player this updates the last known location of the player. This is used in AI_BEHAVE to Search the last known player location when lost LoS of player.
    public void LastSeenPlayerLocation(bool aiCanSeePlayer)
    {
        if(aiCanSeePlayer)
        {
            lastSeenPlayerLocation.position = player.transform.position;
        }
    }

    // Every hidingInterval after coming out of hiding (def: 60s) sets the AI behaviour strategy bool CanHide to true, allowing the AI to hide.
    public void HidingTimer()
    {
        hidingTimer += Time.deltaTime;

        if (hidingTimer >= hidingInterval && !enemyIsHiding)
        {
            enemy.GetComponent<AI_BEHAVE>().CanHide(true);
            enemyIsHiding = true;

            Invoke(nameof(StopHiding), hidingDuration);
        }
    }

    // Stops the AI hiding
    public void StopHiding()
    {
        enemy.GetComponent<AI_BEHAVE>().CanHide(false);
        hidingTimer = 0;
        enemyIsHiding = false;
    }

    #endregion

    #region GETTERS
    // CHARACTER EXACT LOCATIONS
    public Transform GetPlayerTransform()
    {
        return player.transform;
    }

    public Transform GetEnemyTransform()
    {
        return enemy.transform;
    }

    // CHARACTER CURRENT ROOM
    public int GetPlayerRoomNumber()
    {
        return aIRoomInfoManager.playerRoomNumber;
    }

    public int GetEnemyRoomNumber()
    {
        return aIRoomInfoManager.enemyRoomNumber;
    }

    // CHARACTER CURRENT ROOM LOCATIONS
    public Transform GetPlayerRoomTransform()
    {
        return aIRoomInfoManager.playerRoomTransform;
    }

    public Transform GetEnemyRoomTransform()
    {
        return aIRoomInfoManager.enemyRoomTransform;
    }
    #endregion

    #region SETTERS
    // SET NEW LOCATION AND ROOM FOR CHARACTER
    public void SetPlayerTransformPosition(Transform t)
    {
        player.transform.position = t.position;
    }
    public void SetEnemyTransformPosition(Transform t)
    {
        enemy.transform.position = t.position;
    }
    public void SetPlayerRoomNumber(int i)
    {
        aIRoomInfoManager.playerRoomNumber = i;
    }
    public void SetEnemyRoomNumber(int i)
    {
        aIRoomInfoManager.playerRoomNumber = i;
    }

    #endregion

}