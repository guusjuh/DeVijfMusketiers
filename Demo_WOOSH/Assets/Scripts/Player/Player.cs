public class Player {
    private int totalActionPoints = 3;       // total points
    private int currentActionPoints;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    private int invisibleCooldown = 0;
    //public int InvisibleCooldown { get { return invisibleCooldown; } }
    private const int invisibleCooldownTotal = 2;
    
    private int repairCooldown = 0;
    //public int RepairCooldown { get { return repairCooldown; } }
    private const int repairCooldownTotal = 1;

    private int damage = 10;

    public int Damage{ get { return damage; } }

    public void Initialize()
    {
        repairCooldown = 0;
        invisibleCooldown = 0;
    }

    public void StartPlayerTurn(int extraAP = 0)
    {
        currentActionPoints = totalActionPoints + extraAP;
        if (repairCooldown > 0) repairCooldown--;
        if (invisibleCooldown > 0) invisibleCooldown--;
    }

    public bool EndPlayerMove(int cost = 1, bool endTurn = false)
    {
        currentActionPoints = endTurn ? 0 : currentActionPoints - cost;
        UIManager.Instance.InGameUI.PlayerActionPoints.SetAPText();

        if (currentActionPoints <= 0)
        {
            return true;
        }
        return false;
    }

    public void SetInvisibleCooldown()
    {
        invisibleCooldown = invisibleCooldownTotal;
    }

    public void SetRepairCooldown()
    {
        repairCooldown = repairCooldownTotal;
    }

    public int GetCurrentCooldown(GameManager.SpellType type)
    {
        switch (type)
        {
            case GameManager.SpellType.Repair:
                return repairCooldown;
            case GameManager.SpellType.Attack:
                return 0;
            case GameManager.SpellType.Invisible:
                return invisibleCooldown;
            case GameManager.SpellType.Push:
                return 0;
            default:
                return -1;
        }
    }
}
