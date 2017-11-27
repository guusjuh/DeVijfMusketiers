using UnityEngine;

public class Action {
    protected int totalCooldown = 0;
    protected int currentCooldown = 0;
    public int CurrentCooldown { get { return currentCooldown; } }
    protected int cost = 0;
    protected Enemy parent;
    public bool HasSpellIcon = false;
    protected Sprite spellIconSprite;
    public Sprite SpellIconSprite { get { return spellIconSprite; } }

    public virtual void Initialize(Enemy parent)
    {
        this.parent = parent;
    }

    public virtual void Reset()
    {
        currentCooldown = 0;
    }

    public virtual void StartTurn()
    {
        if(currentCooldown > 0){
            currentCooldown--;
        }
    }

    public virtual bool TryHit()
    {
        return true;
    }

    public virtual bool DoAction()
    {
        return false;
    }

    public virtual void ShowPossibleRoads()
    {

    }
}
