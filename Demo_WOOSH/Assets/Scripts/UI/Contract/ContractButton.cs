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
        starPrefab = Resources.Load<GameObject>("Prefabs/UI/PreGame/ContractInfo/Star");
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

    protected void SetHappiness(int happiness, int totalHappiness)
    {
        //what respective happiness would this hooman have if his total was 5
        float percentage = (float)happiness / (float)totalHappiness;
        int normalizedHealth = (happiness > 10) ? Mathf.RoundToInt(percentage * 5.0f) : 1;
        GameObject happinessPrefab = UberManager.Instance.ContentManager.HappinessPrefabs[normalizedHealth - 1];

        happinessIndicator = UIManager.Instance.CreateUIElement(
            happinessPrefab, 
            new Vector2(45.0f, 45.0f), 
            this.transform
            );
    }

    protected void ClearHappiness()
    {
        Destroy(happinessIndicator);
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
