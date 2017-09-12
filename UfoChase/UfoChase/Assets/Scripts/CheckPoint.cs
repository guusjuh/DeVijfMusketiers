using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class CheckPoint : MonoBehaviour {
    private bool reached = false;

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !reached)
        {
            reached = true;
            GameManager.Instance.UFO.NextPath();
        }
    }
}
