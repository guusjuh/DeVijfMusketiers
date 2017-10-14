using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportButton : SpellButton
{
    private int secondCost = 2;

    public override void Initialize()
    {
        base.Initialize();
        cost = 0;
        type = GameManager.SpellType.Teleport;
        SpawnAP(secondCost);
    }

    public override IEnumerator CastSpell()
    {
        yield return StartCoroutine(UIManager.Instance.InGameUI.CastSpell(type,
                GameManager.Instance.TileManager.GetWorldPosition(target.GridPosition)));

        target.GetComponent<Human>().ActivateTeleportButtons();
        GameManager.Instance.LevelManager.EndPlayerMove(cost);
        UIManager.Instance.InGameUI.HideSpellButtons();

        yield return null;
    }

    public override void Activate(WorldObject target)
    {
        base.Activate(target);

        if (secondCost > GameManager.Instance.LevelManager.Player.CurrentActionPoints)
        {
            Active = false;
        }
    }
}
