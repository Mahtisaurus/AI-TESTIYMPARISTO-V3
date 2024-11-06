using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneManager : MonoBehaviour
{
    public GameObject player;

    public TextMeshProUGUI speedText;


    // Update is called once per frame
    void Update()
    {
        speedText.text = "Speed: " + Mathf.Round(player.GetComponent<Rigidbody>().velocity.magnitude).ToString();
    }
}
