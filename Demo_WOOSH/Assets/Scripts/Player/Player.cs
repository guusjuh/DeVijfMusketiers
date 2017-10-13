public class Player {
    private int totalActionPoints = 6;       // total points
    private int currentActionPoints;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    public void Initialize()
    {
    }

    public void StartPlayerTurn(int extraAP = 0)
    {
        currentActionPoints = totalActionPoints + extraAP;
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

    public int GetCurrentCooldown(GameManager.SpellType type)
    {
        switch (type)
        {
            case GameManager.SpellType.Attack:
                return 0;
            case GameManager.SpellType.FrostBite:
                return 3;
        }

        return 0;
    }

    public void IncreaseActionPoints(int addAP)
    {
        currentActionPoints += addAP;
    }
}
