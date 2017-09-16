using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightButton : MonoBehaviour
{
    // fucking ugly: remember to set this before you use CastSpell()
    private SpellButton bttn;
    private GameObject source;

    public void Activate(SpellButton bttn, GameObject source)
    {
        this.bttn = bttn;
        this.source = source;
        gameObject.SetActive(true);
    }

    public void Deactive()
    {
        bttn = null;

        gameObject.SetActive(false);
    }

    public void CastSpell()
    {
        bttn.CastSpell(source);
        bttn = null;
    }
}
