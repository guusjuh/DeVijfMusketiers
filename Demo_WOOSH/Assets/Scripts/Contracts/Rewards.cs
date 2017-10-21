using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rewards
{
    [SerializeField] private float positiveRepPerLevel;
    public float PositiveRepPerLevel { get { return positiveRepPerLevel; } }

    [SerializeField] private float negativeRepPerLevel;
    public float NegativeRepPerLevel { get { return negativeRepPerLevel; } }

    [SerializeField] private float positiveRepCompleted;
    public float PositiveRepCompleted { get { return positiveRepCompleted; } }

    [SerializeField] private float negativeRepCompleted;
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
