using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class Vase : Damagable {
    public Sprite destoryedSpr;
    public Sprite normalSpr;
    private HighlightButton highlightBttn;

    public void Start()
    {
        highlightBttn = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightButton")).GetComponent<HighlightButton>();
        highlightBttn.transform.SetParent(FindObjectOfType<Canvas>().transform);
        highlightBttn.GetComponent<RectTransform>().anchoredPosition = GameManager.Instance.WorldToCanvas(this.transform.position);
        highlightBttn.Deactive();
    }

    public bool destroyed = false;
    public bool Destroyed {
        get { return destroyed; }
        set
        {
            destroyed = value;
            GetComponent<SpriteRenderer>().sprite = destroyed ? destoryedSpr : normalSpr;

            if (destroyed) gameObject.layer = 0;
            else gameObject.layer = 8;
        }
    }

    public override bool Hit()
    {
        Destroyed = true;
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
