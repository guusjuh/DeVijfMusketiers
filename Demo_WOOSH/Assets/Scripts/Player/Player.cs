using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int totalActionPoints = 3;       // total points
    private int currentActionPoints = 0;        // points left this turn
    public int CurrentActionPoints { get { return currentActionPoints; } }

    private Dictionary<SpellManager.SpellType, int> totalCooldown;
    private Dictionary<SpellManager.SpellType, int> currentCooldown;
    public Dictionary<SpellManager.SpellType, int> CurrentCooldown { get { return currentCooldown; } }

    public void Initialize()
    {
        if (UberManager.Instance.DevelopersMode)
        {
            totalActionPoints = 99;

            UberManager.Instance.PlayerData.AdjustReputation(999);
        }
        totalActionPoints = 3;

        currentCooldown = new Dictionary<SpellManager.SpellType, int>();
        currentCooldown.Add(SpellManager.SpellType.Attack, 0);
        currentCooldown.Add(SpellManager.SpellType.Fireball, 0);
        currentCooldown.Add(SpellManager.SpellType.FrostBite, 0);
        currentCooldown.Add(SpellManager.SpellType.Teleport, 0);
    }

    public void StartPlayerTurn(int extraAP = 0)
    {
        currentActionPoints = totalActionPoints + extraAP;
    }

    public bool EndPlayerMove(int cost = 1, bool endTurn = false)
    {
        currentActionPoints = endTurn ? 0 : currentActionPoints - cost;
        currentActionPoints = (currentActionPoints <= 0) ? 0 : currentActionPoints;
        UIManager.Instance.InGameUI.PlayerAPIndicator.SetAPText();

        if (currentActionPoints <= 0)
        {
            for (int i = 0; i < Enum.GetNames(typeof(SpellManager.SpellType)).Length; i++)
            {
                if (!currentCooldown.ContainsKey((SpellManager.SpellType)i)) continue;
                if (currentCooldown[(SpellManager.SpellType)i] <= 0) continue;

                currentCooldown[(SpellManager.SpellType)i]--;
            }
            return true;
        }
        return false;
    }

    public void SetCooldown(SpellManager.SpellType type, int cooldown)
    {
        if (!currentCooldown.ContainsKey(type) || (UberManager.Instance.DevelopersMode))
            return;

        currentCooldown[type] = cooldown;
    }

    public int GetCurrentCooldown(SpellManager.SpellType type)
    {
        if (!currentCooldown.ContainsKey(type)) return 0;

        return currentCooldown.Get(type);
    }

    public void IncreaseActionPoints(int addAP = 1)
    {
        currentActionPoints += addAP;
    }
}
