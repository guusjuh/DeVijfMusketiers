using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    private Dictionary<Particles, GameObject> particles = new Dictionary<Particles, GameObject>();

        public enum Particles
    {
        FireSpellParticle = 0,
        FreezeSpellParticle,
        LightSpellParticle,
        ShrineIsOnparticle,
        TeleportSpellParticle,
        DodinFireballParticle,
        ArnestSuperHealParticle,
        SkettaShieldParticle,
        BurnedParticle,
        FrozenParticle,
        ReputationParticle

    }

	public void Initialize()
    {
        particles.Add(Particles.FireSpellParticle, Resources.Load("Prefabs/Particles/PT_FIRESPELL") as GameObject);
        particles.Add(Particles.FreezeSpellParticle, Resources.Load("Prefabs/Particles/PT_ICESPELL_v2") as GameObject);
        particles.Add(Particles.LightSpellParticle, Resources.Load("Prefabs/Particles/PT_LIGHTSPELL_v2") as GameObject);
        particles.Add(Particles.ShrineIsOnparticle, Resources.Load("Prefabs/Particles/PT_SHRINE") as GameObject);
        particles.Add(Particles.TeleportSpellParticle, Resources.Load("Prefabs/Particles/PT_TELEPORT_v2-IN") as GameObject);
        particles.Add(Particles.DodinFireballParticle, Resources.Load("Prefabs/Particles/PT_DODIN") as GameObject);
        particles.Add(Particles.ArnestSuperHealParticle, Resources.Load("Prefabs/Particles/PT_ARNEST") as GameObject);
        particles.Add(Particles.SkettaShieldParticle, Resources.Load("Prefabs/Particles/PT_SKETTA") as GameObject);
        particles.Add(Particles.BurnedParticle, Resources.Load("Prefabs/Particles/PT_BURNED") as GameObject);
        particles.Add(Particles.FrozenParticle, Resources.Load("Prefabs/Particles/PT_FROZEN") as GameObject);
        particles.Add(Particles.ReputationParticle, Resources.Load("Prefabs/Particles/PT_REPUTATION") as GameObject);
    }

    public void PlayParticle(GameManager.SpellType effect, Vector3 position, Quaternion rotation)
    {
        switch (effect)
        {
            case GameManager.SpellType.Attack:
                PlayParticle(Particles.LightSpellParticle, position, rotation);
                break;
            case GameManager.SpellType.Fireball:
                PlayParticle(Particles.FireSpellParticle, position, rotation);
                break;
            case GameManager.SpellType.FrostBite:
                PlayParticle(Particles.FreezeSpellParticle, position, rotation);
                break;
            case GameManager.SpellType.Teleport:
                PlayParticle(Particles.TeleportSpellParticle, position, rotation);
                break;
        }
    }


    public void PlayParticle(Particles effect, Vector3 position, Quaternion rotation)
    {
        Instantiate(particles[effect], position, rotation);
    }

    public GameObject PlayParticleWithReturn(Particles effect, Vector3 position, Quaternion rotation)
    {
        return Instantiate(particles[effect], position, rotation);
    }
}
