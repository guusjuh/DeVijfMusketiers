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

    private SelectContractWindow selectContractWindow;
    public SelectContractWindow SelectContractWindow { get { return selectContractWindow;} }

    private List<City> cities;
    public List<City> Cities { get { return cities; } }

    protected override void Initialize()
    {
        canvas = GameObject.FindGameObjectWithTag("LevelSelectCanvas").GetComponent<Canvas>();
        anchorCenter = canvas.gameObject.transform.Find("Anchor_Center").GetComponent<RectTransform>();
        anchorBottomRight = canvas.gameObject.transform.Find("Anchor_BottomRight").GetComponent<RectTransform>();
        anchorTopMid = canvas.gameObject.transform.Find("Anchor_TopMid").GetComponent<RectTransform>();

        levelSelectPanel = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/LevelSelectPanel", Vector2.zero, anchorCenter.transform);
        cities = new List<City>(levelSelectPanel.GetComponentsInChildren<City>());
        cities.HandleAction(c => c.Initiliaze());

        reputationParent = UIManager.Instance.CreateUIElement("Prefabs/UI/LevelSelect/ReputationParent", new Vector2(-20, -20), anchorTopMid.transform).GetComponent<ReputationUIManager>();
        reputationParent.Initialize();

        selectContractWindow = new SelectContractWindow(levelSelectPanel.transform.Find("SelectContractMenu").gameObject);
    }

    protected override void Restart()
    {
        cities.HandleAction(c => c.Restart());
        reputationParent.SetStars();
    }

    public override void Clear()
    {
        cities.HandleAction(c => c.Clear());

        base.Clear();
    }
}
