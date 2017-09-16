using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Damagable
{
    private Color shieldColor = new Color(0.25f, 0.25f, 1, 1);
    private Color normalColor;
    public bool Shielded { get; private set; }
    private float shieldTimer = 15.0f;

    public bool dead = false;

    public void Start()
    {
        Shielded = false;
        normalColor = GetComponent<SpriteRenderer>().color;
    }

    public IEnumerator Shield()
    {
        GetComponent<SpriteRenderer>().color = shieldColor;
        Shielded = true;

        yield return new WaitForSeconds(shieldTimer);

        GetComponent<SpriteRenderer>().color = normalColor;
        Shielded = false;
    }

    public override bool Hit()
    {
        if (Shielded)
        {
            shieldTimer -= 5.0f;
        }
        else
        {
            dead = true;
            GameManager.Instance.LevelManager.RemoveHuman(this);
        }

        return true;
    }
}
