using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Contract
{
    [SerializeField] private int id;
    public int ID { get { return id; } }

    [SerializeField] private ContentManager.HumanTypes type;
    public ContentManager.HumanTypes Type { get { return type; } }

    [SerializeField] private int reputation;
    public int Reputation { get { return reputation; } }

    [SerializeField] private int totalHealth;
    private int health;
    public int TotalHealth { get { return totalHealth; } }
    public int Health { get { return health; } }

    private int currentLevel;
    public int CurrentLevel { get { return currentLevel; } }

    private bool activeInCurrentLevel = false;
    public bool ActiveInCurrentLevel { get { return activeInCurrentLevel; } }

    private bool diedLastLevel = false;
    public bool Died { get { return diedLastLevel;} }

    private Rewards rewards;
    public Rewards Rewards { get { return rewards; } }

    [SerializeField] private Sprite inWorld;
    [SerializeField] private Sprite portrait;
    public Sprite InWorld { get { return inWorld; } }
    public Sprite Portrait { get { return portrait; } }

    public Contract(int id, ContentManager.HumanTypes type)
    {
        this.id = id;
        this.type = type;
        this.currentLevel = 0;

        switch (type)
        {
            case ContentManager.HumanTypes.Bad:
                reputation = 1;
                totalHealth = 5;
                break;
            case ContentManager.HumanTypes.Ok:
                reputation = 2;
                totalHealth = 4;
                break;
            case ContentManager.HumanTypes.Normal:
                reputation = 3;
                totalHealth = 4;
                break;
            case ContentManager.HumanTypes.Good:
                reputation = 4;
                totalHealth = 3;
                break;
            case ContentManager.HumanTypes.Fabulous:
                reputation = 5;
                totalHealth = 2;
                break;
        }

        health = totalHealth;

        inWorld = ContentManager.Instance.GetHumanSprites(type)[0];
        portrait = ContentManager.Instance.GetHumanSprites(type)[1];

        rewards = ContentManager.Instance.GetHumanRewards(type);
    }

    public void Initialize()
    {
        health = totalHealth;

        rewards = ContentManager.Instance.GetHumanRewards(type);
    }

    public void SetActive(bool on)
    {
        activeInCurrentLevel = on;
    }

    public void Die()
    {
        diedLastLevel = true;
        health--;
    }

    public bool EndLevel()
    {
        if (diedLastLevel)
        {
            diedLastLevel = false;

            //TODO: animation for losing heart

            UberManager.Instance.PlayerData.AdjustReputation(rewards.NegativeRepPerLevel);

            if (health <= 0)
            {
                UberManager.Instance.PlayerData.AdjustReputation(rewards.NegativeRepCompleted);
                BreakContract();
                return false;
            }
        }
        else
        {
            //TODO: animation for walking to next level
            currentLevel++;

            UberManager.Instance.PlayerData.AdjustReputation(rewards.PositiveRepPerLevel);

            if (currentLevel >= ContentManager.Instance.LevelDataContainer.LevelData.Count)
            {
                UberManager.Instance.PlayerData.AdjustReputation(rewards.PositiveRepCompleted);

                BreakContract();
                return false;
            }
        }

        return true;
    }

    public void BreakContract()
    {
        UberManager.Instance.ContractManager.RemoveContract(this);
        GameManager.Instance.SelectedContracts.Remove(this);
    }
}
