using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour {

    ParticleSystem parts;

	// Use this for initialization
	void Start ()
    {
        parts = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!parts.IsAlive())
        {
            Destroy(transform.parent.gameObject);
        }

        if (UberManager.Instance.GameState != UberManager.GameStates.InGame)
        {
            Destroy(transform.parent.gameObject);
        }
	}
}
