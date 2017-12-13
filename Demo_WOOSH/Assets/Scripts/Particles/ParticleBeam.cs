using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBeam : MonoBehaviour {

    private float speed = 20f;
    private Vector2 collectedPos;
    private bool isOn = false;

    public void Initialize(Vector2 pos)
    {
        gameObject.SetActive(true);
        transform.position = UberManager.Instance.ParticleManager.STAFF_POSITION;
        collectedPos = pos;
        isOn = true;
        Debug.Log(isOn);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }

    public void Update ()
    {
        if (isOn)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, collectedPos, step);
        }

        if((collectedPos - (Vector2)transform.position).magnitude < 0.1f && isOn)
        {
            isOn = false;
            Reset();
        }
        Debug.Log(collectedPos);
	}    
}
