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
        ContentManager.HumanTypes type = (ContentManager.HumanTypes)UnityEngine.Random.Range(0, UberManager.Instance.PlayerData.Reputation);

        Contract newContract = new Contract(id, type);

        AddContract(newContract);
        return newContract;
    }
}