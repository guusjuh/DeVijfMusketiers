using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour {

    ParticleSystem particleSystem;

	// Use this for initialization
	void Start ()
    {
        particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!transform.parent.gameObject.activeInHierarchy || 
            !particleSystem.IsAlive()     ||
            UberManager.Instance.GameState != UberManager.GameStates.InGame)
                Destroy(transform.parent.gameObject);
	}
}
