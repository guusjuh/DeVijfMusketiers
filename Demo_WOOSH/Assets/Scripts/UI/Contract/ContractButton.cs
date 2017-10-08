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
            stars.Add(GameObject.Instantiate(starPrefab, Vector3.zero, Quaternion.identity, starGrid.transform));
        }
    }

    protected void AddHearts(int currentAmount, int totalAmount)
    {
        hearts = new List<GameObject>();

        for (int i = 0; i < totalAmount; i++)
        {
            hearts.Add(GameObject.Instantiate(heartPrefab, Vector3.zero, Quaternion.identity, heartGrid.transform));

            if (i >= currentAmount)
            {
                hearts.Last().GetComponent<Image>().color = Color.black;
            }
        }
    }

    protected void ClearHeartsAndStart()
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
        //TODO: make contract ref aware of it being selected for the current level!
        ClearHeartsAndStart();

        Destroy(this.gameObject);
    }
}
