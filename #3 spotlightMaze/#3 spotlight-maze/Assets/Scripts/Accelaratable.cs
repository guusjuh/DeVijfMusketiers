using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelaratable {

    public Accelaratable()
    {
        StopMovement();
    }

    //values between -1.0f and 1.0f
    private Vector2 acceleration;
    public Vector2 Acceleration
    {
        get { return acceleration; }
        private set { acceleration = value; }
    }
	
	public void Update () {
        Acceleration = new Vector2(Input.acceleration.x, Input.acceleration.y);
    }
    public void Update2()
    {
        Acceleration = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Acceleration = Acceleration / 1.5f;
    }


    public void StopMovement()
    {
        Acceleration = Vector2.zero;
    }
}
