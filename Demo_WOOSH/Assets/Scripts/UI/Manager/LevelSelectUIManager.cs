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

        levelSelectPanel = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/LevelSelect/LevelSelectPanel"), Vector3.zero, Quaternion.identity, anchorCenter.transform);
        levelSelectPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        reputationParent = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/LevelSelect/ReputationParent"), Vector3.zero, Quaternion.identity, anchorTopMid.transform).GetComponent<ReputationUIManager>();
        reputationParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20, -20);
        reputationParent.Initialize();

        levelSelectParentPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/LevelParent");

        BuildLevelParentGrid();
        int counter = 0;
        levelSelectParents.HandleAction(l =>
            {
                counter++;
                l.Initialize(counter);
            }
        );

        GameObject buttonParent = GameObject.Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, anchorBottomRight);
        buttonParent.AddComponent<RectTransform>();
        buttonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(600.0f, 100.0f);
        buttonParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300.0f, 0.0f);

        newHumanButton = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Button"), Vector3.zero, Quaternion.identity,
                buttonParent.transform);
        newHumanButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(175.0f, 0.0f);
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
        //TODO: this may not be what we want in our final version
        //for now: these positions look good and there are always 3 visible levels
        for (int i = 0; i < 3; i++)
        {
            levelSelectParents.Add(GameObject.Instantiate(levelSelectParentPrefab, Vector3.zero, Quaternion.identity, anchorCenter).GetComponent<LevelSelectParent>());
            levelSelectParents[i].GetComponent<RectTransform>().anchoredPosition = positions[i];
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
