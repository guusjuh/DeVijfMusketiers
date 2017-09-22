using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPart : MonoBehaviour
{
    public bool completed = false;

    private static Color stndrtColor = new Color(255, 255, 255, 255);
    private Color selectedColor;

    public void Initialize(Color selectedColor)
    {
        gameObject.GetComponent<Renderer>().material.color = stndrtColor;
        this.selectedColor = selectedColor;
        int i = 0;
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
