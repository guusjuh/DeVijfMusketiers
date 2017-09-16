using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour {
	void Update ()
	{
	    if (Input.anyKey)
            Application.LoadLevel("InGame");
	}
}
