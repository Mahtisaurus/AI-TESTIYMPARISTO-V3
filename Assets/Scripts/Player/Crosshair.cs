using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Crosshair : MonoBehaviour
{
    public TextMeshProUGUI hoverText;


    // Start is called before the first frame update
    void Start()
    {
        hoverText.text = "";
    }

    public void HoverText(string s)
    {
        hoverText.text = s;
    }

    public void ResetText()
    {
        hoverText.text = "";
    }
}