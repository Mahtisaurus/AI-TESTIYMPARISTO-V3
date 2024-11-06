using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAggression : MonoBehaviour
{
    #region VARIABLES
    public float aggressionLevel = 0;

    private float aggressionMax = 100f;
    private float aggressionMin = 0f;
    private float aggressionIncreaseDefault = 10.0f;
    private float aggressionIncreaseCoefficientDefault = 1f;
    private float aggressionCoefficientDefault = 1;
    private float aggressionDepletionDefault = 10.0f;
    private float aggressionDepletionCoefficientDefault = 1f;

    private bool flip = false;
    private float controlInterval = 0.5f;
    #endregion

    private void Start()
    {
        StartCoroutine(ControlAggressionInterval(controlInterval));
    }

    private void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.P))
        {
            flip = !flip;
        }
    }

    #region FUNCTIONS
    
    public void ControlAggression()
    {
        // DO STUFF
        if (flip) ReduceAggression(aggressionDepletionDefault, aggressionDepletionCoefficientDefault);
        else if (!flip) IncreaseAggression(aggressionIncreaseDefault, aggressionIncreaseCoefficientDefault);
    }

    public void ReduceAggression(float depletionAmount, float depletionCoefficient)
    {
        aggressionLevel = aggressionLevel - (depletionAmount * depletionCoefficient);

        if (aggressionLevel < aggressionMin) aggressionLevel = aggressionMin;
    }

    public void IncreaseAggression(float increaseAmount, float increaseCoefficient)
    {
        aggressionLevel = aggressionLevel + (increaseAmount * increaseCoefficient);

        if (aggressionLevel > aggressionMax) aggressionLevel = aggressionMax;
    }

    IEnumerator ControlAggressionInterval(float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            ControlAggression();
        }
    }

    #endregion

}
