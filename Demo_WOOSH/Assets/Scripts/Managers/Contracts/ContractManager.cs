using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

[Serializable]
public class ContractManager {
    [SerializeField] private List<Contract> contracts = new List<Contract>();

    public void Initialize()
    {
        contracts.HandleAction(c => c.Initialize());
    }

    public void AddContract(Contract contract)
    {
        contracts.Add(contract);
    }

    public void RemoveContract(Contract contract)
    {
        contracts.Remove(contract);
    }

    public Contract GetContractReference(int id)
    {
        return contracts.Find(c => c.ID == id);
    }

    public int AmountOfContracts()
    {
        return contracts.Count;
    }

    public int AmountOfContracts(int level)
    {
        return contracts.FindAll(c => c.CurrentLevel == level).Count;
    }

    public List<Contract> ContractsInLevel(int level)
    {
        return contracts.FindAll(c => c.CurrentLevel == level);
    }

    public Contract GenerateRandomContract()
    {
        int id = AmountOfContracts();
     
        // get random type   
        ContentManager.HumanTypes type = GetRandomHumanType();//(ContentManager.HumanTypes)UnityEngine.Random.Range(0, UberManager.Instance.PlayerData.Reputation + 1);

        Contract newContract = new Contract(id, type);

        AddContract(newContract);
        return newContract;
    }

    /// <summary>
    /// Returns a random human type based on the current player reputation.
    /// </summary>
    /// <returns></returns>
    private ContentManager.HumanTypes GetRandomHumanType()
    {
        Dictionary<ContentManager.HumanTypes, int> possibleTypes = new Dictionary<ContentManager.HumanTypes, int>();
        int maxReputation = UberManager.Instance.PlayerData.Reputation + 1;
        
        // obtain all possible types with the current players reputation
        // all same reputations have a probability of 100
        // one start above the player rep as a prob of 10
        for (int i = 0; i < maxReputation; i++)
        {
            possibleTypes.Add((ContentManager.HumanTypes)i, i < maxReputation - 1 ? 100 : 10);
        }

        // sum all probabilities 
        int summedProbability = 0;
        foreach (KeyValuePair<ContentManager.HumanTypes, int> entry in possibleTypes) summedProbability += entry.Value;

        // perform a random roll to find a random humantype
        int counter = 0;
        int randomRoll = (int) UnityEngine.Random.Range(0, summedProbability);

        // find the human type matching the random roll
        foreach (KeyValuePair<ContentManager.HumanTypes, int> entry in possibleTypes)
        {
            counter += entry.Value;
            if (randomRoll < counter)
            {
                return entry.Key;
            }
        }

        // shouldn't happen!
        return 0;
    }
}