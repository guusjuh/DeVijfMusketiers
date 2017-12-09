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
        ShrineIsOnParticle,
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
        particles.Add(Particles.FireSpellParticle, Resources.Load("Prefabs/Particles/FireSpellParticle") as GameObject);
        particles.Add(Particles.FreezeSpellParticle, Resources.Load("Prefabs/Particles/FreezeSpellParticle") as GameObject);
        particles.Add(Particles.LightSpellParticle, Resources.Load("Prefabs/Particles/LightSpellParticle") as GameObject);
        particles.Add(Particles.ShrineIsOnParticle, Resources.Load("Prefabs/Particles/ShrineIsOnParticle") as GameObject);
        particles.Add(Particles.TeleportSpellParticle, Resources.Load("Prefabs/Particles/TeleportSpellParticle") as GameObject);
        particles.Add(Particles.DodinFireballParticle, Resources.Load("Prefabs/Particles/DodinFireballParticle") as GameObject);
        particles.Add(Particles.ArnestSuperHealParticle, Resources.Load("Prefabs/Particles/ArnestSuperHealParticle") as GameObject);
        particles.Add(Particles.SkettaShieldParticle, Resources.Load("Prefabs/Particles/SkettaShieldParticle") as GameObject);
        particles.Add(Particles.BurnedParticle, Resources.Load("Prefabs/Particles/BurnedParticle") as GameObject);
        particles.Add(Particles.FrozenParticle, Resources.Load("Prefabs/Particles/FrozenParticle") as GameObject);
        particles.Add(Particles.ReputationParticle, Resources.Load("Prefabs/Particles/ReputationParticle") as GameObject);
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
