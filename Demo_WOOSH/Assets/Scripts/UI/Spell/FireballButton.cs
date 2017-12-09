using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballButton : SpellButton
{

    public override void Initialize()
    {
        base.Initialize();
        cost = 2;
        duration = 2;
        type = GameManager.SpellType.Fireball;
        spellDamage = 20;
        fireDamage = 5;
        SpawnAP();
    }

    public override void ApplyEffect()
    {
        target.GetComponent<Enemy>().Burn(duration, fireDamage);
        target.GetComponent<Enemy>().TryHit(spellDamage);
        UberManager.Instance.ParticleManager.PlayParticle(ParticleManager.Particles.BurnedParticle, target.transform.position, target.transform.rotation);

        GooglePlayScript.UnlockAchievement(GooglePlayIds.achievement_burn_baby_burn);
    }
}
