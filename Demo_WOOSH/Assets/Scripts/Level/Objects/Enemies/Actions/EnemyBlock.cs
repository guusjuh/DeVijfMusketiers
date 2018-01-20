using System.Collections;
using UnityEngine;

public class EnemyBlock : Action
{
    private ParticleSystem shieldParticle;
    private float blockChance;
    private bool canBlock;

    public override void Initialize(Enemy parent)
    {
        base.Initialize(parent);

        blockChance = 0.3f;
        canBlock = true;
    }

    public override bool TryHit()
    {
        float roll = Random.Range(0.0f, 1.0f);
        if (canBlock && roll < blockChance)
        {
            UberManager.Instance.ParticleManager.PlayParticle(
                ParticleManager.Particles.SkettaShieldParticle, 
                parent.transform.position,
                parent.transform.rotation);

            canBlock = false;
            return false;
        }
        else
        {
            canBlock = true;
        }

        return true;
    }
}
