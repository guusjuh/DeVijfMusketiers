using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleBeam : MonoBehaviour {

    private float speed = 20f;
    private Vector2 collectedPos;
    private bool isOn = false;
    public GameObject canvasElement;
    public GameObject staff;
    private Vector2 staffWorldPos;

    public void Initialize(Vector3 pos)
    {
        gameObject.SetActive(true);
        FindStaffPosition();
        transform.position = staffWorldPos;
        collectedPos = pos;
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
            transform.position = Vector2.MoveTowards(transform.position, collectedPos, step);
        }

        if((collectedPos - (Vector2)transform.position).magnitude < 0.1f && isOn)
        {
            transform.position = Vector3.zero;
            isOn = false;
            Reset();
        }
	}    

    public void FindStaffPosition()
    {
        canvasElement = GameObject.Find("InGameCanvas/Anchor_BottomRight");
        staff = canvasElement.transform.GetChild(0).GetChild(1).gameObject;
        staffWorldPos = Camera.main.ScreenToWorldPoint(staff.transform.position);
        Debug.Log(staffWorldPos);
    }
}
