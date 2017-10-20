using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    protected TileManager.ContentType type;

    protected Coordinate gridPosition;
    public Coordinate GridPosition { get { return gridPosition; } }

    protected List<GameManager.SpellType> possibleSpellTypes;
    public List<GameManager.SpellType> PossibleSpellTypes { get { return possibleSpellTypes; } }

    public virtual void Initialize(Coordinate startPos)
    {
        gridPosition = startPos;
        possibleSpellTypes = new List<GameManager.SpellType>();
    }

    public virtual void Click()
    { 
        if (!GameManager.Instance.LevelManager.PlayersTurn) return;

        bool notMonster = type != TileManager.ContentType.Boss ||
                          type != TileManager.ContentType.Minion;

        if (notMonster) UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();

        GameManager.Instance.CameraManager.LockTarget(this.transform);

        UIManager.Instance.InGameUI.ShowSpellButtons(this);
    }

    public virtual void Clear()
    {
        
    }

    public virtual bool IsFlying() { return false; }
    public virtual bool IsWalking() { return false; }

    public virtual bool IsHuman() { return false; }
    public virtual bool IsMonster() { return false; }

    public virtual bool IsBarrel() { return false; }
    public virtual bool IsShrine() { return false; }
}
