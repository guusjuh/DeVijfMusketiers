using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketta : Enemy {
    public override void Initialize(Coordinate startPos)
    {
        startHealth = 130;
        blockChance = 0.3f;
        canBlock = true;

        hasSpecial = false;
        viewDistance = 3;

        type = SecContentType.Sketta;

        base.Initialize(startPos);
    }

    public override void Reset()
    {
        base.Reset();
    }

    public override bool TryHit(int dmg)
    {
        if (!base.TryHit(dmg))
        {
            UberManager.Instance.ParticleManager.PlayParticle(ParticleManager.Particles.SkettaShieldParticle, transform.position, transform.rotation);
            canBlock = false;
            return false;
        }
        else
        {
            canBlock = true;
        }

        return true;
    }

    public override bool IsWalking() { return true; }
}
