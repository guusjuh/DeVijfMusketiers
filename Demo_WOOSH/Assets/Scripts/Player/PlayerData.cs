using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: write to xml as being save data
public class PlayerData {
    // Reference to contracts to save
    //[SerializeField] private List<Contract> contractsRef;
    //public List<Contract> ContractsRef { get { return contractsRef; } }

    // Reputation
    private float reputation = 112;
    public float Reputation { get { return reputation; } }

    private float toNext = 0;

    private int reputationLevel = 1;
    public int ReputationLevel { get { return reputationLevel < 1 ? 1 : reputationLevel; } }    

    private float minRep = 0.0f;
    private float maxRep = 3000.0f;

    public void Initialize()
    {
        reputationLevel = LevelForRep(reputation);
    }

    public void AdjustReputation(float adjustment)
    {
        // check for reputationlevel matching curr rep
        reputationLevel = LevelForRep(reputation);

        Debug.Log("current replvl: " +reputationLevel);
        Debug.Log("current rep: " +reputation);

        // adjust
        reputation += adjustment;
        reputation = Mathf.Clamp(reputation, minRep, maxRep);

        // check up or down
        if (reputation > ReqRep(reputationLevel + 1) ||
            reputation < ReqRep(reputationLevel))
        {
            reputationLevel = LevelForRep(reputation);
        }


        Debug.Log("-------------------------------------------");
        Debug.Log("adjusted with: " + adjustment);
        Debug.Log("new replvl: " + reputationLevel);
        Debug.Log("new rep: " + reputation);
        Debug.Log("-------------------------------------------");

    }

    public float ReqRep(int level)
    {
        return (3.0f * Mathf.Pow(3.75f, level)) + 100.0f;
    }

    public int LevelForRep(float rep)
    {
        int level = Mathf.FloorToInt(Mathf.Log(((rep - 100.0f) / 3.0f), 3.75f));
        return level < 1 ? 1 : level;
    }
}
