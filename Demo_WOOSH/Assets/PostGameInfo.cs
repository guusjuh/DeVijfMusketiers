using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfo : MonoBehaviour
{
    private Text text;
    private GridLayoutGroup grid;

    private GameObject contractIndicatorPrefab;
    private List<ContractIndicator> contractIndicators;

    //TODO: should inherit and be const, for now do in inspector
    [SerializeField] private string OFF_STRING;
    [SerializeField] private string ON_STRING;

    public void Initialize(List<Contract> contracts = null)
    {
        text = GetComponentInChildren<Text>();
        grid = GetComponentInChildren<GridLayoutGroup>();

        contractIndicatorPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/ContractIndicator");

        if (contracts != null && contracts.Count != 0)
        {
            BuildGrid(contracts);
            SetText(true, contracts.Count);
        }
        else
        {
            SetText(false);
        }
    }

    //TODO: this is ONLY for WIN in level 3!!! THIS IS VERY SPECIAL CASE and it should be replaced as soon as we know how finsihing a contract works!!!
    public void SpecialInitialize(List<Contract> contracts = null)
    {
        if (contracts == null && contracts.Count == 0) return;

        text = GetComponentInChildren<Text>();
        grid = GetComponentInChildren<GridLayoutGroup>();

        contractIndicatorPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/ContractIndicator");

        BuildGrid(contracts);
        text.text = contracts.Count + " humans finished their journey!";
    }

    public void Restart(List<Contract> contracts = null)
    {
        if (contracts != null && contracts.Count != 0)
        {
            BuildGrid(contracts);
            SetText(true, contracts.Count);
        }
        else
        {
            SetText(false);
        }
    }

    public void Clear()
    {
        if (contractIndicators != null && contractIndicators.Count > 0)
        {
            contractIndicators.HandleAction(c => c.Clear());
            contractIndicators.Clear();
            contractIndicators = null;
        }
    }

    private void SetText(bool on, int amount = 0)
    {
        if (on)
            text.text = amount + ON_STRING;
        else
            text.text = OFF_STRING;
    }

    private void BuildGrid(List<Contract> contracts)
    {
        contractIndicators = new List<ContractIndicator>();

        for (int i = 0; i < contracts.Count; i++)
        {
            contractIndicators.Add(GameObject.Instantiate(contractIndicatorPrefab, Vector3.zero, Quaternion.identity, grid.transform).GetComponent<ContractIndicator>());
            contractIndicators.Last().Initialize(contracts[i]);
        }
    }
}
