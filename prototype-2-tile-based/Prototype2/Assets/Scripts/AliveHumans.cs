using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliveHumans : MonoBehaviour
{
    public int deadHumans = 0;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
