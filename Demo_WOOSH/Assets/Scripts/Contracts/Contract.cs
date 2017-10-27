using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contract
{
    private int id;
    public int ID { get { return id; } }

    private int happiness;
    public int Happiness { get { return happiness; } }

    private ContractType type;
    public HumanTypes HumanType { get { return type.HumanType; } }
    public int Reputation { get { return type.Reputation; } }
    public int TotalHappiness { get { return type.TotalHappiness; } }
    public Sprite InWorld { get { return type.InWorld; } }
    public Sprite Portrait { get { return type.Portrait; } }
    public Rewards Rewards { get { return type.Rewards; } }

    private int currentLevel;
    public int CurrentLevel { get { return currentLevel; } }

    private bool activeInCurrentLevel = false;
    public bool ActiveInCurrentLevel { get { return activeInCurrentLevel; } }

    private bool diedLastLevel = false;
    public bool Died { get { return diedLastLevel;} }

    public Contract(int id, ContractType type)
    {
        this.id = id;
        this.type = type;
        this.currentLevel = 0;

        happiness = type.TotalHappiness;
    }

    public void MakeHappy()
    {
        if (happiness < TotalHappiness)
        {
            happiness++;
        }
    }

    public void Initialize()
    {
        happiness = type.TotalHappiness;
    }

    public void SetActive(bool on)
    {
        activeInCurrentLevel = on;
    }

    public void Die()
    {
        diedLastLevel = true;
        happiness--;
    }

    public bool EndLevel()
    {
        if (diedLastLevel)
        {
            diedLastLevel = false;

            //TODO: animation for losing happiness

            UberManager.Instance.PlayerData.AdjustReputation(Rewards.NegativeRepPerLevel);

            if (happiness <= 0)
            {
                UberManager.Instance.PlayerData.AdjustReputation(Rewards.NegativeRepCompleted);
                BreakContract();
                return false;
            }
        }
        else
        {
            //TODO: animation for walking to next level
            currentLevel++;

            UberManager.Instance.PlayerData.AdjustReputation(Rewards.PositiveRepPerLevel);

            if (currentLevel >= ContentManager.Instance.AmountOfLevels)
            {
                UberManager.Instance.PlayerData.AdjustReputation(Rewards.PositiveRepCompleted);

                BreakContract();
                return false;
            }
        }

        return true;
    }

    private void BreakContract()
    {
        UberManager.Instance.ContractManager.RemoveContract(this);
        GameManager.Instance.SelectedContracts.Remove(this);
    }
}
