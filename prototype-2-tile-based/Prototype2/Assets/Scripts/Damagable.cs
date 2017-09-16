using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    public DamagableType type;

    public virtual bool Hit()
    {
        return false;
    }
}
