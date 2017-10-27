using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    private float lifeTime = 0.5f;

    public void Update()
    {
        lifeTime -= Time.deltaTime;

        if(lifeTime < 0) Destroy(gameObject);
    }
}
