using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{

    private DateTime startSession;

    public TimeSpan GetTotalPlaytime { get { return TimeSpan.Parse(totalPlayTimeSting); } }
    public string totalPlayTimeSting = "";
    public bool tutorialFinsihed = false;
    public float reputation;
    public bool[] activeCities;
    public DateTime contractRefreshTime;
    public List<UniqueContractData>[] availableContractsPerCity;
    public List<UniqueContractData>[] activeContractsPerLevel;
    public float musicVolume;
    public float fxVolume;
    public int gameSpeed = 1;

    public void Initialize()
    {
        startSession = DateTime.Now;

        availableContractsPerCity = new List<UniqueContractData>[UberManager.AMOUNT_OF_CITIES];
        activeCities = new bool[UberManager.AMOUNT_OF_CITIES];
        activeContractsPerLevel = new List<UniqueContractData>[UberManager.AMOUNT_OF_LEVELS];
    }

    public void SetValues(SaveData data)
    {
        startSession = DateTime.Now;
        totalPlayTimeSting = data.GetTotalPlaytime.ToString();
        tutorialFinsihed = data.tutorialFinsihed;
        reputation = data.reputation;

        // make sure right length
        // always the smallest to avoid nullpointers
        // if AMOUNT OF CITIES is greater, it will be updated in UpdateData()
        int leastCities = UberManager.AMOUNT_OF_CITIES <= data.activeCities.Length ? UberManager.AMOUNT_OF_CITIES : data.activeCities.Length;
        int leastLevels = UberManager.AMOUNT_OF_LEVELS <= data.activeContractsPerLevel.Length ? UberManager.AMOUNT_OF_LEVELS : data.activeContractsPerLevel.Length;

        activeCities = new bool[leastCities];
        availableContractsPerCity = new List<UniqueContractData>[leastCities];
        for (int i = 0; i < leastCities; i++)
        {
            activeCities[i] = data.activeCities[i];
            availableContractsPerCity[i] = data.availableContractsPerCity[i];
        }

        activeContractsPerLevel = new List<UniqueContractData>[leastLevels];
        for (int i = 0; i < leastLevels; i++)
        {
            activeContractsPerLevel[i] = data.activeContractsPerLevel[i];
        }

        contractRefreshTime = data.contractRefreshTime;
        musicVolume = data.musicVolume;
        fxVolume = data.fxVolume;
        gameSpeed = data.gameSpeed;
    }

    public void UpdateIngame()
    {
        if (tutorialFinsihed) UberManager.Instance.EndTutorial();

        if (UberManager.Instance.DoingSetup) return;

        UberManager.Instance.PlayerData.SetReputation(reputation);

        // other variables cannot be set when in the tutorial
        if (!tutorialFinsihed) return;

        UIManager.Instance.LevelSelectUI.UpdateLastRep();
        UIManager.Instance.LevelSelectUI.ReputationParent.SetStars();

        UIManager.Instance.LevelSelectUI.SettingsMenu.ChangeMusicVolume(musicVolume);
        UIManager.Instance.LevelSelectUI.SettingsMenu.ChangeFxVolume(fxVolume);
        UberManager.Instance.GameSpeed = gameSpeed;

        for (int i = 0; i < activeCities.Length; i++)
        {
            if (activeCities[i]) UIManager.Instance.LevelSelectUI.Cities[i].Reached();
        }

        UberManager.Instance.ContractManager.SetContractTimer(contractRefreshTime);

        List<Contract> tempAvailableContracts = new List<Contract>();
        for (int i = 0; i < availableContractsPerCity.Length; i++)
        {
            if (!activeCities[i] || availableContractsPerCity[i].Count == 0) continue;

            List<Contract> cityContracts = UIManager.Instance.LevelSelectUI.Cities[i].AvailableContracts[UIManager.Instance.LevelSelectUI.Cities[i].Destination];

            for (int j = 0; j < availableContractsPerCity[i].Count; j++)
            {
                Contract temp = new Contract(availableContractsPerCity[i][j], i * 3);
                if (!UIManager.Instance.LevelSelectUI.Cities[i].AvailableContracts[UIManager.Instance.LevelSelectUI.Cities[i].Destination].Contains(temp))
                    tempAvailableContracts.Add(temp);
            }
            Debug.Log(availableContractsPerCity[i].Count + " contracts for city " + i + ", " + tempAvailableContracts.Count + " c. generated");

            UIManager.Instance.LevelSelectUI.Cities[i].RefreshAvailableContracts(tempAvailableContracts, UIManager.Instance.LevelSelectUI.Cities[i].Destination);
            tempAvailableContracts.Clear();
        }

        for (int i = 0; i < activeContractsPerLevel.Length; i++)
        {
            for (int j = 0; j < activeContractsPerLevel[i].Count; j++)
            {
                Contract temp = new Contract(activeContractsPerLevel[i][j], i);

                if (UberManager.Instance.ContractManager.GetContractReference(temp.ID) == null)
                    temp.MyPath.SpawnContract(temp, (temp.CurrentLevel - 1) % 3);
            }
        }
    }

    public void UpdateSaved()
    {
        totalPlayTimeSting = (GetTotalPlaytime + (DateTime.Now - startSession)).ToString();

        // if the tutorial is finished, you cannot go back atm
        if (!tutorialFinsihed) tutorialFinsihed = !UberManager.Instance.Tutorial;

        reputation = UberManager.Instance.PlayerData.Reputation;
        musicVolume = SoundManager.MusicVolume;
        fxVolume = SoundManager.FXVolume;
        gameSpeed = UberManager.Instance.GameSpeed;

        // if the tutorial isn't finished, 
        // trying to obtain all other variables 
        // will cause errors!
        if (!tutorialFinsihed) return;

        bool[] tempActiveCities = new bool[UberManager.AMOUNT_OF_CITIES];
        for (int i = 0; i < UberManager.AMOUNT_OF_CITIES; i++)
            tempActiveCities[i] = UIManager.Instance.LevelSelectUI.Cities[i].CityReached;
        activeCities = tempActiveCities;

        contractRefreshTime = UberManager.Instance.ContractManager.ContractRefreshDate;

        List<UniqueContractData>[] tempAvailableContractsPerCity = new List<UniqueContractData>[UberManager.AMOUNT_OF_CITIES];
        for (int i = 0; i < UberManager.AMOUNT_OF_CITIES; i++)
        {
            tempAvailableContractsPerCity[i] = new List<UniqueContractData>();

            foreach (Contract c in UIManager.Instance.LevelSelectUI.Cities[i].AvailableContracts[UIManager.Instance.LevelSelectUI.Cities[i].Destination])
            {
                tempAvailableContractsPerCity[i].Add(new UniqueContractData(c));
            }
        }
        availableContractsPerCity = tempAvailableContractsPerCity;

        List<UniqueContractData>[] tempActiveContractsPerLevel = new List<UniqueContractData>[UberManager.AMOUNT_OF_LEVELS];
        for (int i = 0; i < UberManager.AMOUNT_OF_LEVELS; i++)
        {
            tempActiveContractsPerLevel[i] = new List<UniqueContractData>();

            foreach (Contract c in UberManager.Instance.ContractManager.ContractsInLevel(i + 1))
            {
                tempActiveContractsPerLevel[i].Add(new UniqueContractData(c));
            }
        }
        activeContractsPerLevel = tempActiveContractsPerLevel;
    }

    public static SaveData InitialData()
    {
        SaveData sgd = new SaveData();

        sgd.totalPlayTimeSting = UberManager.Instance.GooglePlayServices.TotalPlayTime.ToString();
        sgd.tutorialFinsihed = true;
        sgd.reputation = 112.0f;
        sgd.activeCities = new bool[UberManager.AMOUNT_OF_CITIES] { true, false, false };
        sgd.contractRefreshTime = System.DateTime.Now.AddSeconds(ContractManager.CONTRACT_REFRESH_RATE);
        sgd.availableContractsPerCity = new List<UniqueContractData>[UberManager.AMOUNT_OF_CITIES];
        sgd.activeContractsPerLevel = new List<UniqueContractData>[UberManager.AMOUNT_OF_LEVELS];
        sgd.musicVolume = 1;
        sgd.fxVolume = 1;
        sgd.gameSpeed = 1;

        return sgd;
    }
}
