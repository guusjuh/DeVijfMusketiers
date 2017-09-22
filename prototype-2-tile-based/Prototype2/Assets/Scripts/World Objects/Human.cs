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
    private List<HighlightButton> surroundingHighlightBttns;

    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };

    public void Start()
    {
        Shielded = false;
        normalColor = GetComponent<SpriteRenderer>().color;

        highlightBttn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightButton")).GetComponent<HighlightButton>();
        highlightBttn.transform.SetParent(FindObjectOfType<Canvas>().transform);
        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);
        highlightBttn.Deactive();

        surroundingHighlightBttns = new List<HighlightButton>();
        for (int i = 0; i < 4; i++)
        {
            surroundingHighlightBttns.Add(GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightButton")).GetComponent<HighlightButton>());
            surroundingHighlightBttns[i].transform.SetParent(FindObjectOfType<Canvas>().transform);
            surroundingHighlightBttns[i].GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position + positions[i]);
            surroundingHighlightBttns[i].Deactive();
            surroundingHighlightBttns[i].Initialize(transform.position + positions[i]);
        }
    }

    public void Shield()
    {
        GetComponent<SpriteRenderer>().color = shieldColor;
        Shielded = true;
        shieldPoints = 3;
    }

    public void BeginPlayerTurn()
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
            shieldPoints = 0;

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
            SetSurroundingHighlight(false, null);
        }
    }

    public void SetSurroundingHighlight(bool value, SpellButton bttn)
    {
        if (value)
        {
            List<HighlightButton> tempButtons = new List<HighlightButton>();

            for (int i = 0; i < surroundingHighlightBttns.Count; i++)
            {
                if (GameManager.Instance.LevelManager.TileMap.Empty(surroundingHighlightBttns[i].x, surroundingHighlightBttns[i].y))
                {
                    tempButtons.Add(surroundingHighlightBttns[i]);
                }
            }

            tempButtons.HandleAction(b => b.Activate(bttn, this.gameObject, true));
        }
        else
        {
            surroundingHighlightBttns.HandleAction(b => b.Deactive());
        }
    }

    public void Move(int x, int y)
    {
        transform.position = new Vector3(x, y, transform.position.z);

        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);

        for (int i = 0; i < surroundingHighlightBttns.Count; i++)
        {
            surroundingHighlightBttns[i].GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position + positions[i]);
            surroundingHighlightBttns[i].Initialize(transform.position + positions[i]);
        }
    }
}
