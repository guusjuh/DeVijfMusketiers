using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectParent : MonoBehaviour
{
    private LevelSelectButton levelSelectButton;
    public Image levelSelectButtonImage { get { return levelSelectButton.GetComponentInChildren<Image>(); } }

    private GridLayoutGroup gridParent;
    private GameObject contractIndicatorPrefab;
    private List<ContractIndicator> contractIndicators;
    public Image firstContractIndicator { get { return contractIndicators[0].GetComponent<Image>(); } }

    [SerializeField]private int levelID;
    public int LevelID { get { return levelID; } }

    public void Initialize(Path path, int levelInPathId)
    {
        levelSelectButton = transform.Find("Button").GetComponent<LevelSelectButton>();
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

        for (int i = 0; i < UberManager.Instance.ContractManager.AmountOfContracts(levelID); i++)
        {
            contractIndicators.Add(UIManager.Instance.CreateUIElement(contractIndicatorPrefab, Vector2.zero, gridParent.transform).GetComponent<ContractIndicator>());
            contractIndicators.Last().Initialize(UberManager.Instance.ContractManager.ContractsInLevel(levelID)[i]);
        }
    }

    public void AddHuman()
    {
        contractIndicators.Add(UIManager.Instance.CreateUIElement(contractIndicatorPrefab, Vector2.zero, gridParent.transform).GetComponent<ContractIndicator>());
        contractIndicators.Last().Initialize(UberManager.Instance.ContractManager.ContractsInLevel(levelID).Last());
    }
}

