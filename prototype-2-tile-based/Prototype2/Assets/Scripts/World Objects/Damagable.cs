using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    public DamagableType type;
    public int x, y;

    // het rooie blokje eronder :D
    private GameObject target;
    private bool targeted = false;
    public bool Targeted
    {
        get
        {
            return targeted;
        }
        set
        {
            targeted = value;
            if (targeted)
            {
                if(target == null)
                    target = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Target"), this.transform.position,
                    this.transform.rotation);
            }
            else
            {
                if (target != null)
                {
                    GameObject.Destroy(target);
                    target = null;
                }
            }
        }
    }

    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public virtual bool Hit()
    {
        return false;
    }
}
