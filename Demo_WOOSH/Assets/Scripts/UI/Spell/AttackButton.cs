using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackButton : SpellButton
{
    public override void Initialize()
    {
        base.Initialize();
        cost = 1;
        type = GameManager.SpellType.Attack;

    }

    public override void CastSpell()
    {
        if (!target.GetComponent<Enemy>().Hit(GameManager.Instance.LevelManager.Player.Damage))
        {
            base.CastSpell();
        }
    }
}
