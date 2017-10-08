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

    private int currentLevel;

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

    public void SetActive(bool on, int level = 0)
    {
        activeInCurrentLevel = on;
        currentLevel = level;
    }

    public void Die()
    {
        diedLastLevel = true;
    }

    public void EndLevel()
    {
        if (diedLastLevel)
        {
            health--;
            diedLastLevel = false;
            //TODO: animation for losing heart
        }
        else
        {
            //TODO: animation for walking to next level
            currentLevel++;
        }
    }

    public void BreakContract()
    {
        //TODO: lose contract completely!
    }
}
