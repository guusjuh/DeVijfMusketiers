using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightButton : MonoBehaviour
{
    // fucking ugly: remember to set this before you use CastSpell()
    private SpellButton bttn;
    private GameObject source;
    public GameObject Source { get { return source; } }

    public int x, y;

    private bool secondTarget = false;

    public void Initialize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Initialize(Vector2 pos)
    {
        this.x = (int)pos.x;
        this.y = (int)pos.y;
    }

    public void Activate(SpellButton bttn, GameObject source, bool secondTarget = false)
    {
        this.bttn = bttn;
        this.source = source;

        this.secondTarget = secondTarget;

        gameObject.SetActive(true);
    }

    public void Deactive()
    {
        bttn = null;

        gameObject.SetActive(false);
    }

    public void CastSpell()
    {
        bttn.CastSpell(secondTarget ? this.gameObject : source, secondTarget);
        bttn = null;
    }
}
