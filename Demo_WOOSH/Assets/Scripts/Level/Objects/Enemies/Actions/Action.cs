public class Action {
    protected int totalCooldown = 0;
    protected int currentCooldown = 0;

    public virtual void Initialize()
    {

    }

    public virtual void Reset()
    {
        currentCooldown = 0;
    }

    public virtual void Clear()
    {

    }

    public virtual void StartTurn()
    {
        if(currentCooldown > 0){
            currentCooldown--;
        }
    }

    public virtual void EndTurn()
    {

    }

    public virtual bool TryHit()
    {
        return true;
    }

    public virtual void DoAction()
    {

    }
}
