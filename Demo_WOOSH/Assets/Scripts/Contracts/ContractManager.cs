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

    public const double CONTRACT_REFRESH_RATE = 261.0d; //refresh rate in seconds
    private const int CONTRACTS_PER_DESTINATION = 6;
    private DateTime contractRefreshDate;
    public DateTime ContractRefreshDate { get {return contractRefreshDate;} }

    public void Initialize()
    {
        contracts.HandleAction(c => c.Initialize());

        //TODO: ensure that refreshdate is saved, so that it does not reset on restart
        LoadContractTimer();
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

    private void SaveContractTimer()
    {
        //TODO: save contract timer in file
    }

    private void LoadContractTimer()
    {
        //TODO: load contract timer from file
        contractRefreshDate = System.DateTime.Now.AddSeconds(2.0d);
    }

    private void SetContractTimer()
    {
        contractRefreshDate = System.DateTime.Now.AddSeconds(CONTRACT_REFRESH_RATE);
        SaveContractTimer();
    }

    public void UpdateContractTimer()
    {
        UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SetTimer(contractRefreshDate.Subtract(System.DateTime.Now));
        if (contractRefreshDate <= System.DateTime.Now)
        {
            RefreshContracts();
            SetContractTimer();
        }
    }

    private void RefreshContracts()
    {
        Debug.Log("new contracts");
        
        //find all cities and paths
        List<City> cities = UberManager.Instance.UiManager.LevelSelectUI.Cities;

        for (int i = 0; i < cities.Count; i++)
        {
            for (int j = 0; j < cities[i].Paths.Count; j++)
            {
                List<Contract> newContracts = new List<Contract>();
                
                //generate x new contracts, for each destination
                for (int k = 0; k < CONTRACTS_PER_DESTINATION; k++)
                {
                    newContracts.Add(GenerateRandomContract(cities[i].Paths[j]));
                }
                //pass contracts to right cities including destination
                cities[i].RefreshAvailableContracts(newContracts, cities[i].Paths[j].Destination);
            }
        }

        UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SetInteractable();
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

    public Contract GenerateRandomContract(Path path)
    {
        int id = AmountOfContracts();

        // get random type   
        HumanTypes type = GetRandomHumanType();
        List<ContractType> matchingContractTypes = ContractTypes.FindAll(c => c.HumanType == type);

        Contract newContract = new Contract(id, matchingContractTypes[UnityEngine.Random.Range(0, matchingContractTypes.Count)], path);
        //AddContract(newContract);
        
        return newContract;
    }
}