using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: write to xml as being save data
public class PlayerData {
    // Reference to contracts to save
    //[SerializeField] private List<Contract> contractsRef;
    //public List<Contract> ContractsRef { get { return contractsRef; } }

    // Reputation
    [SerializeField] private float reputation = 1;
    public float Reputation { get { return Mathf.Floor(reputation / 100.0f); } }
    private float minRep = 0;
    private float maxRep = 599; // you cannot have 6-stars, but you can be a good 5-star

    public void AdjustReputation(Contract contract)
    {
        // did he lose 
    }
}
