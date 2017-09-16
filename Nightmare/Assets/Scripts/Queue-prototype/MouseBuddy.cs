using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBuddy : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        pos = Camera.main.ScreenToWorldPoint(pos);
	    transform.position = new Vector3(pos.x, 0, pos.z);
	}
}
