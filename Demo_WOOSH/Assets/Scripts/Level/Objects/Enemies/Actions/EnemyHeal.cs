using System.Collections;
using UnityEngine;

public class EnemyHeal : Action
{
    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        totalCooldown = 2;
        cost = 1;

        spellIconSprite = Resources.Load<Sprite>("Sprites/UI/InGame/Spells/enemyHeal");
        HasSpellIcon = true;
    }

    public override bool DoAction()
    {
        bool enoughAP = parent.CurrentActionPoints >= cost;
        bool onCooldown = currentCooldown > 0;

        if (enoughAP && !onCooldown && parent.Health < parent.StartHealth)
        {
            parent.EndMove(cost);
            currentCooldown = totalCooldown;
            parent.Heal(20);//heal a certain amount
            UIManager.Instance.InGameUI.EnemyInfoUI.OnChange(parent);

            UberManager.Instance.ParticleManager.PlayParticle(
                ParticleManager.Particles.ArnestSuperHealParticle, 
                parent.transform.position,
                parent.transform.rotation);

            return true;
        }
        return false;
    }
}
