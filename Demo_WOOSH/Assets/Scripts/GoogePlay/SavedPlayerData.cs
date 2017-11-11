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
    [SerializeField] public List<Contract>[] activeContractsPerLevel;

    public void Initialize()
    {
        Instance = this;

        activeCities = new bool[3];
        activeContractsPerLevel = new List<Contract>[9];
    }

    public void UpdateIngame()
    {
        // tutorial
        if (tutorialFinsihed) UberManager.Instance.EndTutorial();

        // rep 
        UberManager.Instance.PlayerData.Reputation = reputation;

        // active cities
        // posssibly errors since the UI isn't instantiated yet
        for (int i = 0; i < activeCities.Length; i++)
        {
            if(activeCities[i]) UIManager.Instance.LevelSelectUI.Cities[i].Reached();
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

                //TODO: do we need to update happiness? guess not
            }
        }
    }

    public void UpdateSaved()
    {
        // time stamp
        timeStamp = System.DateTime.Now;

        // tutorial
        if(!tutorialFinsihed) tutorialFinsihed = !UberManager.Instance.Tutorial;

        // rep
        reputation = UberManager.Instance.PlayerData.Reputation;

        // active cities
        bool[] tempActiveCities = new bool[3];
        for (int i = 0; i < 3; i++) tempActiveCities[i] = UIManager.Instance.LevelSelectUI.Cities[i].CityReached;
        activeCities = tempActiveCities;

        // active contracts
        List<Contract>[] tempActiveContractsPerLevel = new List<Contract>[9];
        for (int i = 0; i < 9; i++)
        {
            tempActiveContractsPerLevel[i] = UberManager.Instance.ContractManager.ContractsInLevel(i + 1);
        }
        activeContractsPerLevel = tempActiveContractsPerLevel;
    }

    public static SavedPlayerData InitialData()
    {
        SavedPlayerData spd = new SavedPlayerData();

        spd.timeStamp = System.DateTime.Now;
        spd.tutorialFinsihed = false;
        spd.reputation = 112.0f;
        spd.activeCities = new bool[3] {true, false, false};
        spd.activeContractsPerLevel = new List<Contract>[9];

        return spd;
    }
}
