using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightTouch : MonoBehaviour {

	void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().Spotted();
        }
    }
}
