using System;
using System.Collections.Generic;

public class Player
{
    private int totalActionPoints = 10;       // total points
    private int currentActionPoints = 0;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    private Dictionary<GameManager.SpellType, int> totalCooldown;
    private Dictionary<GameManager.SpellType, int> currentCooldown;
    public Dictionary<GameManager.SpellType, int> CurrentCooldown { get { return currentCooldown; } }

    public void Initialize()
    {
        totalCooldown = new Dictionary<GameManager.SpellType, int>();
        totalCooldown.Add(GameManager.SpellType.Attack, 0);
        totalCooldown.Add(GameManager.SpellType.Fireball, 1);
        totalCooldown.Add(GameManager.SpellType.FrostBite, 3);
        totalCooldown.Add(GameManager.SpellType.Teleport, 2);

        currentCooldown = new Dictionary<GameManager.SpellType, int>();
        currentCooldown.Add(GameManager.SpellType.Attack, 0);
        currentCooldown.Add(GameManager.SpellType.Fireball, 0);
        currentCooldown.Add(GameManager.SpellType.FrostBite, 0);
        currentCooldown.Add(GameManager.SpellType.Teleport, 0);
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
            for (int i = 0; i < Enum.GetNames(typeof(GameManager.SpellType)).Length; i++)
            {
                if (!currentCooldown.ContainsKey((GameManager.SpellType)i)) continue;
                if (currentCooldown[(GameManager.SpellType)i] <= 0) continue;

                currentCooldown[(GameManager.SpellType)i]--;
            }

            return true;
        }

        return false;
    }

    public void SetCooldown(GameManager.SpellType type)
    {
        if (!currentCooldown.ContainsKey(type)) return;

        currentCooldown[type] = totalCooldown.Get(type);
    }

    public int GetCurrentCooldown(GameManager.SpellType type)
    {
        if (!currentCooldown.ContainsKey(type)) return 0;

        return currentCooldown.Get(type);
    }

    public void IncreaseActionPoints(int addAP = 1)
    {
        currentActionPoints += addAP;
    }
}
