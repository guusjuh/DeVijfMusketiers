using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards
{
    private float positiveRepPerLevel;
    private float negativeRepPerLevel;
   
    private float positiveRepCompleted;
    private float negativeRepCompleted;

    public Rewards(float positiveRepPerLevel, float negativeRepPerLevel,
        float positiveRepCompleted, float negativeRepCompleted)
    {
        this.positiveRepPerLevel = positiveRepPerLevel;
        this.negativeRepPerLevel = negativeRepPerLevel;
        this.positiveRepCompleted = positiveRepCompleted;
        this.negativeRepCompleted = negativeRepCompleted;
    }

    //TODO: currency rewards
}
