using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour
{

    private bool active = false;

    public bool Active
    {
        get { return active; }
        set
        {
            active = value;
            UpdateEnable();
        }
    }

    private void UpdateEnable()
    {
        if (active)
        {
            GetComponent<Button>().interactable = true;
            //GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            GetComponent<Button>().interactable = false;
            //GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 1);
        }
    }
}
