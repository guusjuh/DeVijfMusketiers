using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ContractButton : MonoBehaviour
{
    protected Contract contractRef;
    public Contract ContractRef { get { return contractRef; } }

    protected GameObject happinessIndicator;
    protected GridLayoutGroup starGrid;
    protected GameObject starPrefab;

    protected List<GameObject> stars = new List<GameObject>();

    public virtual void Initialize(Contract contractRef = null)
    {
        this.contractRef = contractRef;

        starGrid = transform.Find("StarParent").GetComponent<GridLayoutGroup>();
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

    protected void SetHappiness(int health)
    {
        happinessIndicator =
            UIManager.Instance.CreateUIElement(UberManager.Instance.ContentManager.HappinessPrefabs[health - 1],
                                               new Vector2(45.0f, 45.0f), this.transform);
    }

    protected void ClearStars()
    {
        while (stars.Count > 0)
        {
            Destroy(stars[0]);
            stars.RemoveAt(0);
        }
    }

    public virtual void Clear()
    {
        Destroy(happinessIndicator);
        ClearStars();
        Destroy(this.gameObject);
    }
}
