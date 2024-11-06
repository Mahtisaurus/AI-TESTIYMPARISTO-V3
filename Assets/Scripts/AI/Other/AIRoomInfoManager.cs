using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRoomInfoManager : MonoBehaviour
{
    [Header("Character room information")]
    public int playerRoomNumber;
    public int enemyRoomNumber;
    public Transform playerRoomTransform;
    public Transform enemyRoomTransform;


    public void UpdatePlayerRoomInformation(int roomNumber)
    {
        playerRoomNumber = roomNumber;
    }

    public void UpdateEnemyRoomInformation(int roomNumber)
    {
        enemyRoomNumber = roomNumber;
    }

    public void UpdatePlayerRoomTransform(Transform t)
    {
        playerRoomTransform = t;
    }

    public void UpdateEnemyRoomTransform(Transform t)
    {
        enemyRoomTransform = t;
    }
}