using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectParent : MonoBehaviour
{
    private LevelSelectButton levelSelectButton;
    private GridLayoutGroup gridParent;
    private GameObject contractIndicatorPrefab;
    private List<ContractIndicator> contractIndicators;

    [SerializeField]private int levelID;
    public int LevelID { get { return levelID; } }
    private Path path;
    private int levelInPathId;

    public void Initialize(Path path, int levelInPathId)
    {
        this.path = path;
        this.levelInPathId = levelInPathId;

        levelSelectButton = transform.Find("Button").GetComponent<LevelSelectButton>();
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
        bool nextLevelExists = path.hasNextLevel(levelInPathId);
        bool spaceInNextLevel = ContentManager.Instance.LevelData(levelID).amountOfHumans +
                                UberManager.Instance.ContractManager.AmountOfContracts(path.GetNextLevelID(levelInPathId)) 
                                <= 6;
        bool hasEnoughHumans = UberManager.Instance.ContractManager.AmountOfContracts(levelID) >=
                               ContentManager.Instance.LevelData(levelID).amountOfHumans;

        if ((nextLevelExists && !spaceInNextLevel) || !hasEnoughHumans)
        {
            levelSelectButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            levelSelectButton.GetComponent<Button>().interactable = true;
        }
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

