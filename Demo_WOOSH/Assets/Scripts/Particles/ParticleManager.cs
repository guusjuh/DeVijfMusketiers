using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    private Dictionary<Particles, GameObject> particles = new Dictionary<Particles, GameObject>();
    private Dictionary<GameManager.SpellType, ParticleBeam> beams = new Dictionary<GameManager.SpellType, ParticleBeam>();

    private GameObject staff;
    public Vector2 StaffPosition { get { return new Vector3(5.5f, -1f, 0f); } }

    public enum Particles
    {
        None = -1,
        FireSpellParticle = 0,
        FreezeSpellParticle,
        LightSpellParticle,
        TeleportSpellParticle,

        ShrineIsOnParticle,

        DodinFireballParticle,
        ArnestSuperHealParticle,
        SkettaShieldParticle,

        BloodParticle,
        BurnedParticle,
        FrozenParticle,

        ReputationParticle,

        FireBeam,
        FrostBeam,
        LightningBeam,
        TeleportBeam
    }

	public void Initialize()
    {
        particles.Add(Particles.FireSpellParticle, Resources.Load("Prefabs/Particles/FireSpellParticle") as GameObject);
        particles.Add(Particles.FreezeSpellParticle, Resources.Load("Prefabs/Particles/FreezeSpellParticle") as GameObject);
        particles.Add(Particles.LightSpellParticle, Resources.Load("Prefabs/Particles/LightSpellParticle") as GameObject);
        particles.Add(Particles.TeleportSpellParticle, Resources.Load("Prefabs/Particles/TeleportSpellParticle") as GameObject);

        particles.Add(Particles.ShrineIsOnParticle, Resources.Load("Prefabs/Particles/ShrineIsOnParticle") as GameObject);

        particles.Add(Particles.DodinFireballParticle, Resources.Load("Prefabs/Particles/DodinFireballParticle") as GameObject);
        particles.Add(Particles.ArnestSuperHealParticle, Resources.Load("Prefabs/Particles/ArnestSuperHealParticle") as GameObject);
        particles.Add(Particles.SkettaShieldParticle, Resources.Load("Prefabs/Particles/SkettaShieldParticle") as GameObject);

        particles.Add(Particles.BloodParticle, Resources.Load("Prefabs/Particles/BloodParticle") as GameObject);
        particles.Add(Particles.BurnedParticle, Resources.Load("Prefabs/Particles/BurnedParticle") as GameObject);
        particles.Add(Particles.FrozenParticle, Resources.Load("Prefabs/Particles/FrozenParticle") as GameObject);

        particles.Add(Particles.ReputationParticle, Resources.Load("Prefabs/Particles/ReputationParticle") as GameObject);

        particles.Add(Particles.FireBeam, Resources.Load("Prefabs/Particles/FireBeam") as GameObject);
        particles.Add(Particles.FrostBeam, Resources.Load("Prefabs/Particles/FrostBeam") as GameObject);
        particles.Add(Particles.LightningBeam, Resources.Load("Prefabs/Particles/LightningBeam") as GameObject);
        particles.Add(Particles.TeleportBeam, Resources.Load("Prefabs/Particles/TeleportBeam") as GameObject);
    }

    public void PlaySpellParticle(GameManager.SpellType effect, Vector3 position, Quaternion rotation)
    {
        PlayParticle(SpellToParticle(effect), position, rotation);
        CreateBeam(effect).Initialize(position);
    }

    public GameObject PlayParticle(Particles effect, Vector3 position, Quaternion rotation)
    {
        return Instantiate(particles[effect], position, rotation);
    }

    private ParticleBeam CreateBeam(GameManager.SpellType effect)
    {
        if(!beams.ContainsKey(effect))
        {
            Particles p = SpellToBeam(effect);
            if (particles.ContainsKey(p))
            {
                beams[effect] = Instantiate(particles[SpellToBeam(effect)], StaffPosition, Quaternion.identity).GetComponent<ParticleBeam>();
            }
        }
        return beams[effect];
    }

    private Particles SpellToBeam(GameManager.SpellType type)
    {
        switch (type)
        {
            case GameManager.SpellType.Fireball:
                return Particles.FireBeam;
            case GameManager.SpellType.Attack:
                return Particles.LightningBeam;
            case GameManager.SpellType.FrostBite:
                return Particles.FrostBeam;
            case GameManager.SpellType.Teleport:
                return Particles.TeleportBeam;

            default:
                return Particles.None;
        }
    }

    private Particles SpellToParticle(GameManager.SpellType type)
    {
        switch (type)
        {
            case GameManager.SpellType.Fireball:
                return Particles.FireSpellParticle;
            case GameManager.SpellType.Attack:
                return Particles.LightSpellParticle;
            case GameManager.SpellType.FrostBite:
                return Particles.FreezeSpellParticle;
            case GameManager.SpellType.Teleport:
                return Particles.TeleportSpellParticle;

            default:
                return Particles.LightSpellParticle;
        }
    }
}

