using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : Item
{
    public bool destroyed = false;
    [SerializeField]
    private Material spriteDestroyed;
    [SerializeField]
    private Material spriteWhole;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile")
        {
            Hit();
        }
    }

    public override void Hit()
    {
        base.Hit();
        destroyed = true;
        GetComponent<MeshRenderer>().material = spriteDestroyed;
    }

    public void Repair()
    {
        destroyed = false;
        GetComponent<MeshRenderer>().material = spriteWhole;
    }
}
