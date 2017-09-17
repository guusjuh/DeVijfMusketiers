using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableSpell : MonoBehaviour {

    public Button btn;

    public void Start()
    {
        btn = GetComponent<Button>();
    }

    public void SetInteractable(bool value)
    {
        btn.interactable = value;
    }
}
