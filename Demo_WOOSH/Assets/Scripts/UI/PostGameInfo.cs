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

    private static string NoString { get { return "No"; } }

    public void Initialize(string textValue, List<Contract> contracts = null)
    {
        text = GetComponentInChildren<Text>();
        grid = GetComponentInChildren<GridLayoutGroup>();

        contractIndicatorPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/ContractIndicator");

        if (contracts != null && contracts.Count != 0)
        {
            BuildGrid(contracts);
            SetText(true, textValue, contracts.Count);
        }
        else
        {
            SetText(false, textValue);
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

    public void Restart(string textValue, List<Contract> contracts = null)
    {
        if (contracts != null && contracts.Count != 0)
        {
            BuildGrid(contracts);
            SetText(true, textValue, contracts.Count);
        }
        else
        {
            SetText(false, textValue);
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

    private void SetText(bool on, string value, int amount = 0)
    {
        if (on)
            text.text = amount + value;
        else
            text.text = NoString + value;
    }

    private void BuildGrid(List<Contract> contracts)
    {
        contractIndicators = new List<ContractIndicator>();

        for (int i = 0; i < contracts.Count; i++)
        {
            contractIndicators.Add(UIManager.Instance.CreateUIElement(contractIndicatorPrefab, Vector2.zero, grid.transform).GetComponent<ContractIndicator>());
            contractIndicators.Last().Initialize(contracts[i]);
        }
    }
}
