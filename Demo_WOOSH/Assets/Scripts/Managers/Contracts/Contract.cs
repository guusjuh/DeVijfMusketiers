using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Contract
{
    [SerializeField] private int id;
    public int ID { get { return id; } }

    private ContentManager.HumanTypes type;
    public ContentManager.HumanTypes Type { get { return type; } }

    [SerializeField] private int reputation;
    [SerializeField] private int health;
    public int Health { get { return health; } }

    private int currentLevel;
    public int CurrentLevel { get { return currentLevel; } }

    private bool activeInCurrentLevel = false;
    public bool ActiveInCurrentLevel { get { return activeInCurrentLevel; } }

    private bool diedLastLevel = false;
    public bool Died { get { return diedLastLevel;} }

    [SerializeField] private Sprite inWorld;
    [SerializeField] private Sprite portrait;
    public Sprite InWorld { get { return inWorld; } }
    public Sprite Portrait { get { return portrait; } }

    //TODO: buy sprites!
    public Contract(int id, ContentManager.HumanTypes type)
    {
        this.id = id;
        this.type = type;
        this.currentLevel = 0;

        switch (type)
        {
            case ContentManager.HumanTypes.Normal:
                reputation = 1;
                health = 3;
                break;
            case ContentManager.HumanTypes.Ok:
                reputation = 3;
                health = 4;
                break;
            case ContentManager.HumanTypes.Good:
                reputation = 5;
                health = 5;
                break;
        }

        inWorld = ContentManager.Instance.GetHumanSprites(type)[0];
        portrait = ContentManager.Instance.GetHumanSprites(type)[1];
    }

    public void SetActive(bool on)//, int level = 0)
    {
        activeInCurrentLevel = on;
        //currentLevel = level;
    }

    public void Die()
    {
        diedLastLevel = true;
        health--;
    }

    public void EndLevel()
    {
        if (diedLastLevel)
        {
            diedLastLevel = false;
            Debug.Log("died this level");
            //TODO: animation for losing heart

            if (health <= 0)
            {
                BreakContract();
            }
        }
        else
        {
            //TODO: animation for walking to next level
            currentLevel++;
            Debug.Log("survived this level");

            if (currentLevel >= ContentManager.Instance.LevelDataContainer.LevelData.Count)
            {
                BreakContract();
            }
        }
    }

    public void BreakContract()
    {
        UberManager.Instance.ContractManager.RemoveContract(this);
        GameManager.Instance.SelectedContracts.Remove(this);
    }
}
