using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            Hit();
        }
    }

    public virtual void Hit()
    {
    }

    public void SetHighlight(bool value)
    {
        //use child for highlightobj
        transform.GetChild(0).gameObject.SetActive(value);
    }
}
