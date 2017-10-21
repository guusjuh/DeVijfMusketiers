using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIManager : SubUIManager
{
    private RectTransform anchorCenter;
    private RectTransform anchorBottomRight;
    private RectTransform anchorTopMid;

    private GameObject levelSelectPanel;
    private ReputationUIManager reputationParent;

    private GameObject levelSelectParentPrefab;
    private List<LevelSelectParent> levelSelectParents;

    private GameObject newHumanButton;

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("LevelSelectCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();

        levelSelectPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/LevelSelectPanel", Vector2.zero, anchorCenter.transform);

        reputationParent = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/ReputationParent", new Vector2(-20, -20), anchorTopMid.transform).GetComponent<ReputationUIManager>();
        reputationParent.Initialize();

        levelSelectParentPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/LevelParent");

        BuildLevelParentGrid();
        int counter = 0;
        levelSelectParents.HandleAction(l =>
            {
                l.Initialize(counter);
                counter++;
            }
        );

        GameObject buttonParent = UIManager.Instance.CreateUIElement(new Vector2(-300.0f, 0.0f), new Vector2(600.0f, 100.0f), anchorBottomRight);
        newHumanButton = UIManager.Instance.CreateUIElement("Prefabs/UI/Button", new Vector2(175.0f, 0.0f), buttonParent.transform);
        newHumanButton.GetComponentInChildren<Text>().text = "Get new contract";
        newHumanButton.GetComponent<Button>().onClick.AddListener(GenerateNewContract);
    }

    protected override void Restart()
    {
        levelSelectParents.HandleAction(l => l.Restart());
        reputationParent.SetStars();
    }

    public override void Clear()
    {
        levelSelectParents.HandleAction(l => l.Clear());

        base.Clear();
    }

    private void BuildLevelParentGrid()
    {
        List<Vector2> positions = new List<Vector2>();
        positions.Add(new Vector2(0, 650));
        positions.Add(new Vector2(0, -35));
        positions.Add(new Vector2(0, -600));

        levelSelectParents = new List<LevelSelectParent>();
        //for now: these positions look good and there are always 3 visible levels
        for (int i = 0; i < 3; i++)
        {
            levelSelectParents.Add(UIManager.Instance.CreateUIElement(levelSelectParentPrefab, positions[i], anchorCenter).GetComponent<LevelSelectParent>());
        }
    }

    public void GenerateNewContract()
    {
        //all contracts in level one
        if (UberManager.Instance.ContractManager.ContractsInLevel(0).Count >= 6) return;

        UberManager.Instance.ContractManager.GenerateRandomContract();

        // update grid level 1
        levelSelectParents[0].AddHuman();

        levelSelectParents.HandleAction(l => l.CheckActiveForButton());
    }
}
