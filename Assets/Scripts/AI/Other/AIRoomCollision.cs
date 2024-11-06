using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRoomCollision : MonoBehaviour
{
    public AIRoomInfoManager roomInfoManager;

    [Header("Room Info")]
    [SerializeField] private int roomNumber;

    private void Start()
    {
        roomInfoManager = gameObject.GetComponentInParent<AIRoomInfoManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            roomInfoManager.UpdatePlayerRoomInformation(roomNumber);
            roomInfoManager.UpdatePlayerRoomTransform(gameObject.transform);
            //Debug.Log("PLAYER " + roomNumber);
        }
        else if(other.gameObject.tag == "Enemy")
        {
            roomInfoManager.UpdateEnemyRoomInformation(roomNumber);
            roomInfoManager.UpdateEnemyRoomTransform(gameObject.transform);
            //Debug.Log("ENEMY " + roomNumber);
        }
        else
        {
            //Debug.Log("SOMETHING COLLIDED " + roomNumber);
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        roomInfoManager.UpdatePlayerRoomInformation(roomNumber);
    //        roomInfoManager.UpdatePlayerRoomTransform(gameObject.transform);
    //    }
    //    else if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        roomInfoManager.UpdateEnemyRoomInformation(roomNumber);
    //        roomInfoManager.UpdateEnemyRoomTransform(gameObject.transform);
    //    }
    //}
}
