using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleBeam : MonoBehaviour {

    private const float speed = 20f;
    private Vector2 targetPos;
    private bool isOn = false;

    public void Initialize(Vector3 pos)
    {
        gameObject.SetActive(true);

        Vector2 staffPos = Camera.main.ScreenToWorldPoint(UberManager.Instance.UiManager.InGameUI.AnchorBottomRight.transform.GetChild(0).GetChild(0).GetChild(0).transform.position);
        transform.position = staffPos;

        targetPos = pos;

        isOn = true;
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
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
        }

        if((targetPos - (Vector2)transform.position).magnitude < 0.1f && isOn)
        {
            transform.position = Vector3.zero;
            isOn = false;
            Reset();
        }
	}    
}
