using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    protected SecContentType type;
    public SecContentType Type { get { return type; } }

    protected bool canBlock = false;
    protected float blockChance = 0.0f;

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
        if (!GameManager.Instance.LevelManager.PlayersTurn && !UberManager.Instance.Tutorial) return;

        if (!IsMonster()) UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();

        GameManager.Instance.CameraManager.LockTarget(this.transform);

        UIManager.Instance.InGameUI.ShowSpellButtons(this);
    }

    public virtual bool TryHit(int dmg)
    {
        float roll = Random.Range(0.0f, 1.0f);
        if (canBlock && roll < blockChance)
        {
            return false;
        }
        return true;
    }

    protected virtual bool Hit(int dmg)
    {
        return false;
    }

    // Called on pausing the ingame while in devmode
    public virtual void Reset() { }

    // Called to reset every object back to inital position etc. of last saved level
    public virtual void ResetToInitDEVMODE(Coordinate startPos)
    {
        Reset();
        gameObject.SetActive(true);
        gridPosition = startPos;
    }

    public virtual void Clear() { }

    public virtual bool IsFlying() { return false; }
    public virtual bool IsWalking() { return false; }

    public virtual bool IsHuman() { return false; }
    public virtual bool IsMonster() { return false; }

    public virtual bool IsBarrel() { return false; }
    public virtual bool IsShrine() { return false; }
}
