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
        CheckActiveForButton();
    }

    public void Restart()
    {
        BuildGrid();
        CheckActiveForButton();
    }

    public void CheckActiveForButton()
    {
        // check for being able to play this level:
        // if the next level has too many humans, they cant travel to it

        // if there is a next level
        // check for the amount of humans in the next level plus the humans traveling from this level
        // being smaller than 7
        bool nextLevelExists = levelID < ContentManager.Instance.AmountOfLevels - 1;
        bool spaceInNextLevel = ContentManager.Instance.LevelData(levelID).amountOfHumans +
                                UberManager.Instance.ContractManager.AmountOfContracts(levelID + 1) 
                                <= 6;
        bool hasEnoughHumans = UberManager.Instance.ContractManager.AmountOfContracts(levelID) <
                               ContentManager.Instance.LevelData(levelID).amountOfHumans;

        if ((nextLevelExists && !spaceInNextLevel) || hasEnoughHumans) 
            levelSelectButton.GetComponent<Button>().interactable = false;
        else
            levelSelectButton.GetComponent<Button>().interactable = true;
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

