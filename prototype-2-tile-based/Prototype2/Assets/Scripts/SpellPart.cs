using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPart : MonoBehaviour
{
    public bool completed = false;

    private static Color stndrtColor = new Color(255, 255, 255, 255);
    private static Color selectedColor = new Color(0, 0, 255, 255);

    public void Initialize()
    {
        gameObject.GetComponent<Renderer>().material.color = stndrtColor;
    }

    public void Reset()
    {
        completed = false;
    }

    void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            gameObject.GetComponent<Renderer>().material.color = selectedColor;
            completed = true;
        }
    }
}
