using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arnest : Enemy
{
    public override void Initialize(Coordinate startPos)
    {
        //set boss specific health
        startHealth = 100;
        hasSpecial = false;
        viewDistance = 3;

        totalSpecialCooldown = 2;
        type = SecContentType.Arnest;
        specialCost = 1;
        hasSpecial = true;

        SpellIconSprite = Resources.Load<Sprite>("Sprites/UI/InGame/Spells/enemyHeal");

        base.Initialize(startPos);
    }

    public override void Reset()
    {
        totalActionPoints = 3;
        base.Reset();
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

            UberManager.Instance.ParticleManager.PlayParticle(ParticleManager.Particles.ArnestSuperHealParticle, transform.position, transform.rotation);

            return true;
        }
        return false;
    }

    public override bool IsWalking()
    {
        return true;
    }
}