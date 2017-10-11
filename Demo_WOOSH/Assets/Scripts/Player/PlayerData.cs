﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: write to xml as being save data
public class PlayerData {
    // Reference to contracts to save
    //[SerializeField] private List<Contract> contractsRef;
    //public List<Contract> ContractsRef { get { return contractsRef; } }

    // Reputation
    [SerializeField] private float reputation = 1;

    public int Reputation
    {
        get
        {
            int rep = (int)Mathf.Floor(reputation / 100.0f);
            return rep < 1 ? 1 : rep;
        }
    }

    private float minRep = 0;
    private float maxRep = 599; // you cannot have 6-stars, but you can be a good 5-star

    public void AdjustReputation(float adjustment)
    {
        reputation += adjustment;

        Debug.Log("current reputation: " + reputation);

        // check for updating the visuals.
    }
}
