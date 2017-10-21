using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

[Serializable]
public class ContractManager
{
    private List<Contract> contracts = new List<Contract>();
    [SerializeField] private List<ContractType> contractTypes = new List<ContractType>();

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
        HumanTypes type = GetRandomHumanType();
        List<ContractType> matchingContractTypes = contractTypes.FindAll(c => c.HumanType == type);

        Contract newContract = new Contract(id,
                                            matchingContractTypes[UnityEngine.Random.Range(0, matchingContractTypes.Count)]);
        AddContract(newContract);
        return newContract;
    }

    /// <summary>
    /// Returns a random human type based on the current player reputation.
    /// </summary>
    /// <returns></returns>
    private HumanTypes GetRandomHumanType()
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