using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    public DamagableType type;
    public int x, y;

    // either dead or broken, set from child classes
    protected bool cannotBeTarget;

    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };

    // het rooie blokje eronder :D
    protected GameObject target;
    protected bool targeted = false;
    public bool Targeted
    {
        get
        {
            return targeted;
        }
        set
        {
            targeted = value;

            if (cannotBeTarget)
            {
                if (target != null)
                {
                    GameObject.Destroy(target);
                    target = null;
                }
            }
            else
            {

                if (targeted)
                {
                    if (target == null)
                        target = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Target"),
                            this.transform.position,
                            this.transform.rotation);

                    else
                    {
                        target.transform.position = this.transform.position;
                    }

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

    public bool CanBePushed()
    {
        for (int i = 0; i < positions.Length; i++)
        {
            if (GameManager.Instance.LevelManager.TileMap.Empty((int)positions[i].x + x, (int)positions[i].y + y))
            {
                return true;
            }
        }

        return false;
    }
}
