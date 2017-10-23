using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arnest : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        this.startHealth = 100;
        this.hasSpecial = false;
        viewDistance = 3;

        this.totalSpecialCooldown = 2;
        this.specialCost = 1;
        this.hasSpecial = true;
        this.type = SecContentType.Arnest;

        this.SpellIconSprite = Resources.Load<Sprite>("Sprites/UI/Spells/enemyHeal");

        base.Initialize(startPos);
    }

    protected override void Attack(EnemyTarget other)
    {
        base.Attack(other);
    }

    public override bool CheckForSpell()
    {
        // target reached
        bool enoughAP = currentActionPoints >= specialCost;
        bool onCooldown = specialCooldown > 0;

        if (enoughAP && !onCooldown && Health < startHealth)
        {
            currentActionPoints -= specialCost;
            specialCooldown = totalSpecialCooldown;
            Heal(20);//heal a certain amount
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(this);


            return true;
        }
        return false;
    }

    public override bool IsWalking()
    {
        return true;
    }
}