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

    protected SpellManager.SpellTarget spellTargetType = SpellManager.SpellTarget.None;
    protected List<SpellManager.SpellType> possibleSpellTypes;
    public List<SpellManager.SpellType> PossibleSpellTypes { get { return possibleSpellTypes; } }

    public virtual void Initialize(Coordinate startPos)
    {
        gridPosition = startPos;
        possibleSpellTypes = new List<SpellManager.SpellType>();
        if (spellTargetType != SpellManager.SpellTarget.None)
            possibleSpellTypes = UberManager.Instance.SpellManager.GetPossibleSpellTypes(spellTargetType);
    }

    public virtual void Click()
    { 
        if (!GameManager.Instance.LevelManager.PlayersTurn && !UberManager.Instance.Tutorial) return;

        if (!IsMonster()) UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();

        GameManager.Instance.CameraManager.LockTarget(this.transform);

        UberManager.Instance.SpellManager.SelectTarget(this);
        UberManager.Instance.SpellManager.ShowSpellButtons();

        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }

    public void Teleport(Coordinate newPos)
    {
        GameManager.Instance.TileManager.MoveObject(gridPosition, newPos, this);

        gridPosition = newPos;
        Vector3 worldPos = GameManager.Instance.TileManager.GetWorldPosition(gridPosition);

        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
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
