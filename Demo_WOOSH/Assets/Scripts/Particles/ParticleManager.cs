using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    private Dictionary<Particles, GameObject> particles = new Dictionary<Particles, GameObject>();

        public enum Particles
    {
        FireParticle = 0,
        IceParticle,
        LightParticle,
        ShrineParticle,
        TeleportInParticle,
        TeleportOutParticle,
        DodinParticle,
        ArnestParticle,
        SkettaParticle,
        BurnedParticle,
        FrozenParticle,
        ReputationParticle

    }

	public void Initialize()
    {
        particles.Add(Particles.FireParticle, Resources.Load("Prefabs/Particles/PT_FIRESPELL") as GameObject);
        particles.Add(Particles.IceParticle, Resources.Load("Prefabs/Particles/PT_ICESPELL_v2") as GameObject);
        particles.Add(Particles.LightParticle, Resources.Load("Prefabs/Particles/PT_LIGHTSPELL_v2") as GameObject);
        particles.Add(Particles.ShrineParticle, Resources.Load("Prefabs/Particles/PT_SHRINE") as GameObject);
        particles.Add(Particles.TeleportInParticle, Resources.Load("Prefabs/Particles/PT_TELEPORT_v2-IN") as GameObject);
        particles.Add(Particles.TeleportOutParticle, Resources.Load("Prefabs/Particles/PT_TELEPORT_v2-WEG") as GameObject);
        particles.Add(Particles.DodinParticle, Resources.Load("Prefabs/Particles/PT_DODIN") as GameObject);
        particles.Add(Particles.ArnestParticle, Resources.Load("Prefabs/Particles/PT_ARNEST") as GameObject);
        particles.Add(Particles.SkettaParticle, Resources.Load("Prefabs/Particles/PT_SKETTA") as GameObject);
        particles.Add(Particles.BurnedParticle, Resources.Load("Prefabs/Particles/PT_BURNED") as GameObject);
        particles.Add(Particles.FrozenParticle, Resources.Load("Prefabs/Particles/PT_FROZEN") as GameObject);
        particles.Add(Particles.ReputationParticle, Resources.Load("Prefabs/Particles/PT_REPUTATION") as GameObject);
    }

    public void PlayParticle(GameManager.SpellType effect, Vector3 position, Quaternion rotation)
    {
        switch (effect)
        {
            case GameManager.SpellType.Attack:
                PlayParticle(Particles.LightParticle, position, rotation);
                break;
            case GameManager.SpellType.Fireball:
                PlayParticle(Particles.FireParticle, position, rotation);
                break;
            case GameManager.SpellType.FrostBite:
                PlayParticle(Particles.IceParticle, position, rotation);
                break;
            case GameManager.SpellType.Teleport:
                PlayParticle(Particles.TeleportInParticle, position, rotation);
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
