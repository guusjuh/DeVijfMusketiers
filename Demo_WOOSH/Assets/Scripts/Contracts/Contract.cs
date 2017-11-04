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

    private Path path;
    public Path MyPath { get { return path; } }

    private int levelInPath;

    private ContractType type;
    private int humanIndex = 0;
    public HumanTypes HumanType { get { return type.HumanType; } }
    public int Reputation { get { return type.Reputation; } }
    public int TotalHappiness { get { return type.TotalHappiness; } }
    public GameObject InWorld { get { return type.HumanAssets[humanIndex].InWorld; } }
    public Sprite InWorldSprite { get { return type.HumanAssets[humanIndex].InWorldSprite; } }
    public Sprite Portrait { get { return type.HumanAssets[humanIndex].Portrait; } }
    public Rewards Rewards { get { return type.Rewards; } }

    private int currentLevel;
    public int CurrentLevel { get { return currentLevel; } }

    private bool activeInCurrentLevel = false;
    public bool ActiveInCurrentLevel { get { return activeInCurrentLevel; } }

    private bool diedLastLevel = false;
    public bool Died { get { return diedLastLevel;} }

    public Contract(int id, ContractType type, Path path, int index = -1)
    {
        this.id = id;
        this.type = type;
        levelInPath = 0;
        if(path != null) currentLevel = path.Levels[0].LevelID;
        this.path = path;

        humanIndex = index == -1 ? UnityEngine.Random.Range(0, type.HumanAssets.Count) : index;

        happiness = Mathf.CeilToInt((type.TotalHappiness / 50.0f) * 40.0f);
    }

    public void MakeHappy()
    {
        if (happiness < TotalHappiness)
        {
            happiness += 5;
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
        happiness -= 10;
    }

    public bool EndLevel()
    {
        if (diedLastLevel)
        {
            diedLastLevel = false;

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
            UberManager.Instance.PlayerData.AdjustReputation(Rewards.PositiveRepPerLevel);

            //TODO:fix this
            if (levelInPath + 1 >= path.Levels.Count)
            {
                UberManager.Instance.PlayerData.AdjustReputation(Rewards.PositiveRepCompleted);

                if(!UberManager.Instance.Tutorial) UIManager.Instance.LevelSelectUI.Cities.Find(c => path.Destination == c.ThisCity).Reached();

                BreakContract();
                return false;
            }
            else
            {
                currentLevel = path.GetNextLevelID(levelInPath);
                levelInPath++;
            }
        }

        return true;
    }

    private void BreakContract()
    {
        UberManager.Instance.ContractManager.RemoveContract(this);
        GameManager.Instance.SelectedContracts.Remove(this);
    }

    public bool HasNextLevel()
    {
        return path.hasNextLevel(levelInPath);
    }
}
