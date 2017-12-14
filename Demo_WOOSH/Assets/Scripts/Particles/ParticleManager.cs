using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    private Dictionary<Particles, GameObject> particles = new Dictionary<Particles, GameObject>();

    private Dictionary<GameManager.SpellType, ParticleBeam> beams = new Dictionary<GameManager.SpellType, ParticleBeam>();
    public GameObject staff;

    //VECTOR KAN NIET CONSTANT ZIJN
    public Vector2 STAFF_POSITION = new Vector3(5.5f, -1f, 0f);

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
        ReputationParticle,
        FireBeam,
        FrostBeam,
        LightningBeam

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
        particles.Add(Particles.FireBeam, Resources.Load("Prefabs/Particles/FireBeam") as GameObject);
        particles.Add(Particles.FrostBeam, Resources.Load("Prefabs/Particles/FrostBeam") as GameObject);
        particles.Add(Particles.LightningBeam, Resources.Load("Prefabs/Particles/LightningBeam") as GameObject);
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

    public ParticleBeam CreateBeam(GameManager.SpellType effect)
    {
        if(!beams.ContainsKey(effect))
        {
            beams[effect] = Instantiate(particles[SpellToBeams(effect)], STAFF_POSITION, Quaternion.identity).GetComponent<ParticleBeam>();
        }
        return beams[effect];
    }

    public Particles SpellToBeams(GameManager.SpellType type)
    {
        switch (type)
        {
            case GameManager.SpellType.Fireball:
                return Particles.FireBeam;
            case GameManager.SpellType.Attack:
                return Particles.LightningBeam;
            case GameManager.SpellType.FrostBite:
                return Particles.FrostBeam;
            default:
                return Particles.FireBeam;
        }
    }
}

