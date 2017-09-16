using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Damagable
{
    private Color shieldColor = new Color(0.0f, 0.0f, 1, 1);
    private Color normalColor;
    public bool Shielded { get; private set; }
    private int shieldPoints = 2;

    public bool dead = false;

    private HighlightButton highlightBttn;

    public void Start()
    {
        Shielded = false;
        normalColor = GetComponent<SpriteRenderer>().color;

        highlightBttn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightButton")).GetComponent<HighlightButton>();
        highlightBttn.transform.SetParent(FindObjectOfType<Canvas>().transform);
        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);
        highlightBttn.Deactive();
    }

    public void Shield()
    {
        GetComponent<SpriteRenderer>().color = shieldColor;
        Shielded = true;
        shieldPoints = 3;
    }

    public void EndPlayerTurn()
    {
        if (Shielded)
        {
            shieldPoints--;

            GetComponent<SpriteRenderer>().color += new Color(0.25f, 0.25f, 0, 0);

            if (shieldPoints <= 0)
            {
                GetComponent<SpriteRenderer>().color = normalColor;
                Shielded = false;
            }
        }
    }

    public override bool Hit()
    {
        if (Shielded)
        {
            shieldPoints--;

            GetComponent<SpriteRenderer>().color += new Color(0.25f, 0.25f, 0, 0);

            if (shieldPoints <= 0)
            {
                GetComponent<SpriteRenderer>().color = normalColor;
                Shielded = false;
            }
        }
        else
        {
            dead = true;
            GameManager.Instance.LevelManager.RemoveHuman(this);
        }

        return true;
    }

    public void SetHighlight(bool value, SpellButton bttn)
    {
        if (value)
        {
            highlightBttn.Activate(bttn, this.gameObject);
        }
        else
        {
            highlightBttn.Deactive();
        }
    }
}
