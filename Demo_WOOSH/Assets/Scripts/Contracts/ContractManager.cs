﻿using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ContractManager
{
    private static int contentCounter = 0;

    //all active contracts
    private List<Contract> contracts = new List<Contract>();
    [SerializeField] private List<ContractType> contractTypes = new List<ContractType>();
    public List<ContractType> ContractTypes { get { return contractTypes; } }

    public const double CONTRACT_REFRESH_RATE = 3600.0d;//261.0d; //refresh rate in seconds
    private const int CONTRACTS_PER_DESTINATION = 6;
    private DateTime contractRefreshDate;
    public DateTime ContractRefreshDate { get {return contractRefreshDate;} }

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

    public void SetContractTimer(DateTime value)
    {
        contractRefreshDate = value;
    }

    private void SetContractTimer()
    {
        contractRefreshDate = System.DateTime.Now.AddSeconds(CONTRACT_REFRESH_RATE);
        //TODO: SavedPlayerData.Instance.UpdateSaved();
    }

    public void UpdateContractTimer()
    {
        UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SetTimer(contractRefreshDate.Subtract(System.DateTime.Now));
        //CLEANUP: remove minute per second line
        contractRefreshDate = contractRefreshDate.Subtract(new TimeSpan(0,0,0,10));
        if (contractRefreshDate <= System.DateTime.Now)
        {
            RefreshContracts();
            SetContractTimer();

            GooglePlayScript.Instance.SaveData();
        }
    }

    public void RefreshCityContracts(City city)
    {
        if (city.Paths != null)
        {
            for (int j = 0; j < city.Paths.Count; j++)
            {
                List<Contract> newContracts = new List<Contract>();

                //generate x new contracts, for each destination
                for (int k = 0; k < CONTRACTS_PER_DESTINATION; k++)
                {
                    newContracts.Add(GenerateRandomContract(city.Paths[j]));
                }
                //pass contracts to right cities including destination
                city.RefreshAvailableContracts(newContracts, city.Paths[j].Destination);
            }
        }
    }

    private void RefreshContracts()
    {        
        //find all cities and paths
        List<City> cities = UberManager.Instance.UiManager.LevelSelectUI.Cities;

        for (int i = 0; i < cities.Count; i++)
        {
            if (!cities[i].CityReached) continue;
            if (cities[i].Paths != null)
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
        int maxReputation = UberManager.Instance.PlayerData.ReputationLevel + 1;

        if (maxReputation > 5) maxReputation = 5;

        // obtain all possible types with the current players reputation
        // all same reputations have a probability of 100
        // one start above the player rep as a prob of 10
        for (int i = 0; i < maxReputation; i++)
        {
            possibleTypes.Add((HumanTypes) i, i < maxReputation - 1 ? 50 * (i+1) : 10);
        }
        
        return UberManager.PerformRandomRoll<HumanTypes>(possibleTypes);
    }

    public Contract GenerateRandomContract(Path path)
    {
        contentCounter++;
        int id = contentCounter;

        // get random type   
        HumanTypes type = GetRandomHumanType();
        List<ContractType> matchingContractTypes = ContractTypes.FindAll(c => c.HumanType == type);

        Contract newContract = new Contract(id, matchingContractTypes[UnityEngine.Random.Range(0, matchingContractTypes.Count)], path);
        
        return newContract;
    }

    public Contract GenerateContract(Path path, int rep, int index = -1)
    {
        contentCounter++;
        int id = contentCounter;

        // get random type   
        HumanTypes type = (HumanTypes)(rep-1);
        ContractType matchingContractType = ContractTypes.Find(c => c.HumanType == type);

        Contract newContract = new Contract(id, matchingContractType, path, index);

        return newContract;
    }
}