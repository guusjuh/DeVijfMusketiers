using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ContractManager
{
    //all active contracts
    private List<Contract> contracts = new List<Contract>();
    [SerializeField] private List<ContractType> contractTypes = new List<ContractType>();
    public List<ContractType> ContractTypes { get { return contractTypes; } }

    public const float CONTRACT_REFRESH_RATE = 260.0f; //refresh rate in seconds
    private bool hasNewContracts = false;


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

    /// <summary>
    /// Returns a random human type based on the current player reputation.
    /// </summary>
    /// <returns></returns>
    public HumanTypes GetRandomHumanType()
    {
        Dictionary<HumanTypes, int> possibleTypes = new Dictionary<HumanTypes, int>();
        int maxReputation = UberManager.Instance.PlayerData.Reputation + 1;

        // obtain all possible types with the current players reputation
        // all same reputations have a probability of 100
        // one start above the player rep as a prob of 10
        for (int i = 0; i < maxReputation; i++)
        {
            possibleTypes.Add((HumanTypes) i, i < maxReputation - 1 ? 100 : 10);
        }

        return UberManager.PerformRandomRoll<HumanTypes>(possibleTypes);
    }
}