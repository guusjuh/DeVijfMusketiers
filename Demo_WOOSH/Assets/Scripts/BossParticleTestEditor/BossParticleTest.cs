using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticleTest : MonoBehaviour {

    public GameObject arnest;
    public GameObject sketta;

    public GameObject skettaParticle;
    public GameObject arnestParticle;

    public Bosses chooseBoss;

    public enum Bosses
    {
        Arnest = 0,
        Sketta
    }

    public enum Particles
    {
        arnestParticle = 0,
        skettaParticle
    }

    public void SpawnBoss(Bosses boss, Vector2 spawnPoint, Quaternion rotation)
    {
        switch (boss)
        {
            case Bosses.Arnest:
                Instantiate(arnest, spawnPoint, rotation);
                break;
            case Bosses.Sketta:
                Instantiate(sketta, spawnPoint, rotation);
                break;
        }
    }

    public void SpawnParticle(Particles particle, Vector2 spawnPoint, Quaternion rotation)
    {
        switch (particle)
        {
            case Particles.arnestParticle:
                Instantiate(arnestParticle, spawnPoint, rotation);
                break;
            case Particles.skettaParticle:
                Instantiate(skettaParticle, spawnPoint, rotation);
                break;
        }
    }
}
