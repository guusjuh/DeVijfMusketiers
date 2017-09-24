using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Damagable
{
    private Color shieldColor = new Color(0.0f, 0.0f, 0.5f, 0.5f);
    private Color normalColor;
    public bool Invisible { get; private set; }
    private int shieldPoints = 2;

    public bool dead = false;

    private HighlightButton highlightBttn;
    private List<HighlightButton> surroundingHighlightBttns;

    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };

    public void Start()
    {
        Invisible = false;
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
        Invisible = true;
        shieldPoints = 2;

        GameManager.Instance.LevelManager.TileMap.SwitchHoomanStatus(x, y, true);
        gameObject.layer = 0;

        Targeted = false;
    }

    public void BeginPlayerTurn()
    {
        if (Invisible)
        {
            shieldPoints--;

            GetComponent<SpriteRenderer>().color += new Color(0.25f, 0.25f, 0.25f, 0.25f);

            if (shieldPoints <= 0)
            {
                GetComponent<SpriteRenderer>().color = normalColor;
                Invisible = false;
                GameManager.Instance.LevelManager.TileMap.SwitchHoomanStatus(x, y, false);
                gameObject.layer = 8;
            }
        }
    }

    public override bool Hit()
    {
        /*if (Invisible)
        {
            shieldPoints = 0;

            if (shieldPoints <= 0)
            {
                GetComponent<SpriteRenderer>().color = normalColor;
                Invisible = false;
            }
        }
        else
        {*/
            dead = true;
            cannotBeTarget = true;
            Targeted = false;
            GameManager.Instance.LevelManager.RemoveHuman(this);
        //}

        return true;
    }

    public void SetHighlight(bool value, SpellButton bttn)
    {
        if (value && !Invisible)
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
        GameManager.Instance.LevelManager.TileMap.MoveObject(this.x, this.y, x, y, TileMap.Types.Human);

        transform.position = new Vector3(x, y, transform.position.z);
        this.x = (int) transform.position.x;
        this.y = (int) transform.position.y;

        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);

        for (int i = 0; i < surroundingHighlightBttns.Count; i++)
        {
            surroundingHighlightBttns[i].GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position + positions[i]);
            surroundingHighlightBttns[i].Initialize(transform.position + positions[i]);
        }

        List<Shrine> all = new List<Shrine>();
        all.AddMultiple(FindObjectsOfType<Shrine>() as Shrine[]);

        all.HandleAction(s => s.CheckForActive());

        if (target != null)
        {
            target.transform.position = this.transform.position;

            bool stillTarget = false;
            for (int i = 0; i < GameManager.Instance.Creatures.Count; i++)
            {
                for (int j = 0; j < positions.Length; j++)
                {
                    if (Mathf.Abs(GameManager.Instance.Creatures[i].x - (x + positions[j].x)) < 0.1f &&
                        Mathf.Abs(GameManager.Instance.Creatures[i].y - (y + positions[j].y)) < 0.1f)
                    {
                        stillTarget = true;
                    }
                }
            }

            if (!stillTarget)
            {
                Targeted = false;
            }
        }
    }
}
