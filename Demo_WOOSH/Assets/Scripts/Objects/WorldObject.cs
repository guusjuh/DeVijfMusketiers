﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    protected TileManager.ContentType type;
    public TileManager.ContentType Type { get { return type; } }

    protected Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition; } }

    protected List<GameManager.SpellType> possibleSpellTypes;

    public virtual void Initialize(Coordinate startPos)
    {
        gridPosition = startPos;
        possibleSpellTypes = new List<GameManager.SpellType>();
    }

    public virtual void Click()
    { 
        if (!GameManager.Instance.LevelManager.PlayersTurn) return;

        //TODO: shouldn't only be walking monster but hey
        if (type != TileManager.ContentType.WalkingMonster) UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();

        UIManager.Instance.InGameUI.ShowSpellButtons(transform.position, possibleSpellTypes, this);
    }

    //TODO: should be moved to an objectpool
    public virtual void Clear()
    {
        
    }
}
