using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIndicatorMaterialChanger : MonoBehaviour
{
    public Material currentMaterial;

    [Header("State Indicator Materials")]
    public Material[] indicatorMaterials = new Material[8];

    private void Start()
    {
        currentMaterial = gameObject.GetComponent<Renderer>().material;

        foreach(Material mat in indicatorMaterials)
        {
            Debug.Log(mat);
        }
    }

    public void ChangeIndicatorMaterial(int i)
    {
        switch (i)
        {
            case 0:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[0];
                break;
            case 1:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[1];
                break;
            case 2:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[2];
                break;
            case 3:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[3];
                break;
            case 4:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[4];
                break;
            case 5:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[5];
                break;
            case 6:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[6];
                break;
            case 7:
                gameObject.GetComponent<Renderer>().material = indicatorMaterials[7];
                break;
        }
    }
}