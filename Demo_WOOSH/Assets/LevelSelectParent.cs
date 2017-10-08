using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectParent : MonoBehaviour
{
    private LevelSelectButton levelSelectButton;
    private GridLayoutGroup gridParent;
    private GameObject contractIndicatorPrefab;
    private List<ContractIndicator> contractIndicators;

    private int levelID;

    public void Initialize(int id)
    {
        levelSelectButton = transform.Find("Button").GetComponent<LevelSelectButton>();
        levelID = id;
        levelSelectButton.Initialize(levelID);

        contractIndicatorPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/ContractIndicator");
        gridParent = transform.Find("GridParent").GetComponent<GridLayoutGroup>();

        BuildGrid();
    }

    public void Restart()
    {
        BuildGrid();
    }

    public void Clear()
    {
        contractIndicators.HandleAction(c => c.Clear());
        contractIndicators.Clear();
        contractIndicators = null;
    }

    private void BuildGrid()
    {
        contractIndicators = new List<ContractIndicator>();

        for (int i = 0; i < UberManager.Instance.ContractManager.AmountOfContracts(levelID - 1); i++)
        {
            contractIndicators.Add(GameObject.Instantiate(contractIndicatorPrefab, Vector3.zero, Quaternion.identity, gridParent.transform).GetComponent<ContractIndicator>());
            contractIndicators.Last().Initialize(UberManager.Instance.ContractManager.ContractsInLevel(levelID - 1)[i]);
        }
    }

    public void AddHuman()
    {
        contractIndicators.Add(GameObject.Instantiate(contractIndicatorPrefab, Vector3.zero, Quaternion.identity, gridParent.transform).GetComponent<ContractIndicator>());
        contractIndicators.Last().Initialize(UberManager.Instance.ContractManager.ContractsInLevel(levelID - 1).Last());
    }
}

