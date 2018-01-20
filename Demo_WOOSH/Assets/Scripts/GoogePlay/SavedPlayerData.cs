using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedPlayerData
{
    public static SavedPlayerData Instance { get; set; }

    [SerializeField] public System.DateTime timeStamp;
    [SerializeField] public bool tutorialFinsihed = false;
    [SerializeField] public float reputation;
    [SerializeField] public bool[] activeCities;

    [SerializeField] public DateTime contractRefreshTime;
    [SerializeField] public List<Contract>[] availableContractsPerCity;

    [SerializeField] public List<Contract>[] activeContractsPerLevel;

    [SerializeField] public float musicVolume;
    [SerializeField] public float fxVolume;

    [SerializeField] public int gameSpeed = 1; 

    public void Initialize()
    {
        Instance = this;

        availableContractsPerCity = new List<Contract>[3];

        activeCities = new bool[3];
        activeContractsPerLevel = new List<Contract>[9];
    }

    public void InitializeInGame()
    {
        if (tutorialFinsihed)
        {
            UIManager.Instance.LevelSelectUI.SettingsMenu.ChangeMusicVolume(musicVolume);
            UIManager.Instance.LevelSelectUI.SettingsMenu.ChangeFxVolume(fxVolume);
        }

        UpdateIngame();

        if (tutorialFinsihed)
        {
            UIManager.Instance.LevelSelectUI.UpdateLastRep();
            UIManager.Instance.LevelSelectUI.ReputationParent.SetStars();
        }
    }

    public void UpdateIngame()
    {
        // tutorial
        if (tutorialFinsihed) UberManager.Instance.EndTutorial();

        // rep 
        UberManager.Instance.PlayerData.SetReputation(reputation);

        if (!tutorialFinsihed) return;

        // active cities
        // posssibly errors since the UI isn't instantiated yet
        for (int i = 0; i < activeCities.Length; i++)
        {
            if(activeCities[i]) UIManager.Instance.LevelSelectUI.Cities[i].Reached();
        }

        // refresh contracts timer
        UberManager.Instance.ContractManager.SetContractTimer(contractRefreshTime);

        // available contracts
        List<Contract> tempGeneratedContracts = new List<Contract>();
        for (int i = 0; i < availableContractsPerCity.Length; i++)
        {
            if (!activeCities[i] || availableContractsPerCity[i].Count == 0) continue;

            for (int j = 0; j < availableContractsPerCity[i].Count; j++)
            {
                Contract temp = availableContractsPerCity[i][j];

                if (!UIManager.Instance.LevelSelectUI.Cities[i].AvailableContracts[UIManager.Instance.LevelSelectUI.Cities[i].Destination].Contains(temp))
                {
                    tempGeneratedContracts.Add(new Contract(temp.id_att, temp.happiness_att, temp.type_att, temp.human_index_att, i * 3));
                }
            }

            UIManager.Instance.LevelSelectUI.Cities[i].RefreshAvailableContracts(tempGeneratedContracts, UIManager.Instance.LevelSelectUI.Cities[i].Destination);
            tempGeneratedContracts.Clear();
        }

        // active contracts
        for (int i = 0; i < activeContractsPerLevel.Length; i++)
        {
            for (int j = 0; j < activeContractsPerLevel[i].Count; j++)
            {
                Contract temp = activeContractsPerLevel[i][j];

                if (UberManager.Instance.ContractManager.GetContractReference(temp.id_att) == null)
                {
                    Contract newContract = new Contract(temp.id_att, temp.happiness_att, temp.type_att, temp.human_index_att, i);
                    newContract.MyPath.SpawnContract(newContract, (newContract.CurrentLevel - 1) % 3);
                }
            }
        }

        // music & sounds
        UberManager.Instance.SoundManager.MusicVolume = musicVolume;
        UberManager.Instance.SoundManager.FXVolume = fxVolume;

        // in-game speed
        UberManager.Instance.GameSpeed = gameSpeed;
    }

    public void UpdateSaved()
    {
        // time stamp
        timeStamp = System.DateTime.Now;

        // tutorial
        if(!tutorialFinsihed) tutorialFinsihed = !UberManager.Instance.Tutorial;

        // rep
        reputation = UberManager.Instance.PlayerData.Reputation;

        if (!tutorialFinsihed) return;

        // active cities
        bool[] tempActiveCities = new bool[3];
        for (int i = 0; i < 3; i++) tempActiveCities[i] = UIManager.Instance.LevelSelectUI.Cities[i].CityReached;
        activeCities = tempActiveCities;

        // refresh contracts timer
        contractRefreshTime = UberManager.Instance.ContractManager.ContractRefreshDate;

        // available contracts per city
        List<Contract>[] tempAvailableContractsPerCity = new List<Contract>[3];
        for (int i = 0; i < 3; i++)
        {
            tempAvailableContractsPerCity[i] = UIManager.Instance.LevelSelectUI.Cities[i].AvailableContracts
                                                [UIManager.Instance.LevelSelectUI.Cities[i].Destination];
        }
        availableContractsPerCity = tempAvailableContractsPerCity;

        // active contracts
        List<Contract>[] tempActiveContractsPerLevel = new List<Contract>[9];
        for (int i = 0; i < 9; i++)
        {
            tempActiveContractsPerLevel[i] = UberManager.Instance.ContractManager.ContractsInLevel(i + 1);
        }
        activeContractsPerLevel = tempActiveContractsPerLevel;

        // music & sounds
        musicVolume = UberManager.Instance.SoundManager.MusicVolume;
        fxVolume = UberManager.Instance.SoundManager.FXVolume;

        // game speed
        gameSpeed = UberManager.Instance.GameSpeed;
    }

    public static SavedPlayerData InitialData()
    {
        SavedPlayerData spd = new SavedPlayerData();

        spd.timeStamp = System.DateTime.Now;
        spd.tutorialFinsihed = false;
        spd.reputation = 112.0f;
        spd.activeCities = new bool[3] {true, false, false};
        spd.contractRefreshTime = System.DateTime.Now.AddSeconds(ContractManager.CONTRACT_REFRESH_RATE);
        spd.availableContractsPerCity = new List<Contract>[3];
        spd.activeContractsPerLevel = new List<Contract>[9];
        spd.musicVolume = 1.0f;
        spd.fxVolume = 1.0f;
        spd.gameSpeed = 1;

        return spd;
    }
}
