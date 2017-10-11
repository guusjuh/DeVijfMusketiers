using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards
{
    private float positiveRepPerLevel;
    public float PositiveRepPerLevel { get { return positiveRepPerLevel; } }

    private float negativeRepPerLevel;
    public float NegativeRepPerLevel { get { return negativeRepPerLevel; } }
   
    private float positiveRepCompleted;
    public float PositiveRepCompleted { get { return positiveRepCompleted; } }

    private float negativeRepCompleted;
    public float NegativeRepCompleted { get { return negativeRepCompleted; } }

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
