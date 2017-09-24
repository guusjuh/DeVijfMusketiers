using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class Barrel : Damagable {
    public Sprite destoryedSpr;
    public Sprite normalSpr;
    private HighlightButton highlightBttn;
    private List<HighlightButton> surroundingHighlightBttns;

    static Vector3[] positions = { new Vector3(0, 1), new Vector3(1, 0), new Vector3(-1, 0), new Vector3(0, -1) };

    public void Start()
    {
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

    public bool destroyed = false;
    public bool Destroyed {
        get { return destroyed; }
        set
        {
            destroyed = value;
            cannotBeTarget = destroyed;
            GetComponent<SpriteRenderer>().sprite = destroyed ? destoryedSpr : normalSpr;
            GameManager.Instance.LevelManager.TileMap.SwitchVaseStatus(x, y, destroyed);
            if (destroyed) gameObject.layer = 0;
            else gameObject.layer = 8;
        }
    }

    public bool CheckForCreatureStandingOnMe()
    {
        List<Minion> enemies = GameManager.Instance.Creatures;

        if (GameManager.Instance.Creature != null)
        {
            if (GameManager.Instance.Creature.x == x && GameManager.Instance.Creature.y == y)
            {
                return true;
            }
        }

        foreach (BaseEnemy e in enemies)
        {
            if (e.x == x && e.y == y)
            {
                return true;
            }
        }

        return false;
    }

    public override bool Hit()
    {
        if (target != null)
        {
            GameObject.Destroy(target);
            target = null;
        }
        Destroyed = true;
        return true;
    }

    public void Remove()
    {
        cannotBeTarget = true;
        Targeted = false;
        GameManager.Instance.LevelManager.TileMap.RemoveObject(x, y);
        GameObject.Destroy(gameObject);
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
                if (GameManager.Instance.LevelManager.TileMap.Empty(surroundingHighlightBttns[i].x, surroundingHighlightBttns[i].y, false))
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
        GameManager.Instance.LevelManager.TileMap.MoveObject(this.x, this.y, x, y, TileMap.Types.Barrel);

        transform.position = new Vector3(x, y, transform.position.z);
        this.x = (int)transform.position.x;
        this.y = (int)transform.position.y;

        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);

        for (int i = 0; i < surroundingHighlightBttns.Count; i++)
        {
            surroundingHighlightBttns[i].GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position + positions[i]);
            surroundingHighlightBttns[i].Initialize(transform.position + positions[i]);
        }
    }
}
