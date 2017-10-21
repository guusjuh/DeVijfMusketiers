using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ContractButton : MonoBehaviour
{
    protected Contract contractRef;
    public Contract ContractRef { get { return contractRef; } }

    protected GridLayoutGroup heartGrid;
    protected GridLayoutGroup starGrid;

    protected GameObject heartPrefab;
    protected GameObject starPrefab;

    protected List<GameObject> hearts = new List<GameObject>();
    protected List<GameObject> stars = new List<GameObject>();

    public virtual void Initialize(Contract contractRef = null)
    {
        this.contractRef = contractRef;

        heartGrid = transform.Find("HeartParent").GetComponent<GridLayoutGroup>();
        starGrid = transform.Find("StarParent").GetComponent<GridLayoutGroup>();

        heartPrefab = Resources.Load<GameObject>("Prefabs/UI/PreGame/ContractInfo/HeartImg");
        starPrefab = Resources.Load<GameObject>("Prefabs/UI/PreGame/ContractInfo/StarImg");
    }

    public virtual void OnClick() { }

    protected void AddStars(int amount)
    {
        stars = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            stars.Add(UIManager.Instance.CreateUIElement(starPrefab, Vector2.zero, starGrid.transform));
        }
    }

    protected void AddHearts(int currentAmount, int totalAmount)
    {
        hearts = new List<GameObject>();

        for (int i = 0; i < totalAmount; i++)
        {
            hearts.Add(UIManager.Instance.CreateUIElement(heartPrefab, Vector2.zero, heartGrid.transform));

            if (i >= currentAmount)
            {
                hearts.Last().GetComponent<Image>().color = Color.black;
            }
        }
    }

    protected void ClearHeartsAndStars()
    {
        while (hearts.Count > 0)
        {
            Destroy(hearts[0]);
            hearts.RemoveAt(0);
        }

        while (stars.Count > 0)
        {
            Destroy(stars[0]);
            stars.RemoveAt(0);
        }
    }

    public virtual void Clear()
    {
        ClearHeartsAndStars();
        Destroy(this.gameObject);
    }
}
