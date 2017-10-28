using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGameInfoPanel : MonoBehaviour
{
    private int levelID;
    private Text levelText;
    private const string LEVEL_TEXT = "Level ";

    private GridLayoutGroup gridContracts;
    private GridLayoutGroup gridActiveContracts;

    private GameObject activeContractPrefab;
    private GameObject selectableContractPrefab;

    private List<SelectableContract> selectableContracts = new List<SelectableContract>();
    private List<ActiveContract> activeContracts = new List<ActiveContract>();
    public Image FirstSelectableContract { get { return selectableContracts[0].transform.Find("Image").GetComponent<Image>();  } }

    public void Initialize()
    {
        levelID = UberManager.Instance.PreGameManager.SelectedLevel;

        levelText = transform.Find("StatusText").GetComponent<Text>();
        levelText.text = LEVEL_TEXT + (levelID + 1);

        gridContracts = transform.Find("GridContracts").GetComponent<GridLayoutGroup>();
        gridActiveContracts = transform.Find("GridActiveContracts").GetComponent<GridLayoutGroup>();

        activeContractPrefab = Resources.Load<GameObject>("Prefabs/UI/PreGame/ActiveContractButton").gameObject;
        selectableContractPrefab = Resources.Load<GameObject>("Prefabs/UI/PreGame/HumanContractButton").gameObject;

        BuildGrids();
    }

    public void Restart()
    {
        levelID = UberManager.Instance.PreGameManager.SelectedLevel;
        BuildGrids();
        levelText.text = LEVEL_TEXT + (levelID + 1);
    }

    public void Clear()
    {
        selectableContracts.HandleAction(c => c.Clear());
        selectableContracts.Clear();
        selectableContracts = null;

        activeContracts.HandleAction(c => c.Clear());
        activeContracts.Clear();
        activeContracts = null;
    }

    private void BuildGrids()
    {
        selectableContracts = new List<SelectableContract>();
        activeContracts = new List<ActiveContract>();

        for (int i = 0; i < UberManager.Instance.ContractManager.AmountOfContracts(levelID); i++)
        {
            selectableContracts.Add(UIManager.Instance.CreateUIElement(selectableContractPrefab, Vector2.zero, gridContracts.transform).GetComponent<SelectableContract>());
            selectableContracts.Last().Initialize(UberManager.Instance.ContractManager.ContractsInLevel(levelID)[i]);
        }

        for (int i = 0; i < ContentManager.Instance.LevelData(levelID).amountOfHumans; i++)
        {
            activeContracts.Add(UIManager.Instance.CreateUIElement(activeContractPrefab, Vector2.zero, gridActiveContracts.transform).GetComponent<ActiveContract>());
            activeContracts.Last().Initialize();
        }
    }

    public bool AddToActive(Contract contractRef)
    {
        ActiveContract reference = null;

        // find first empty active slot
        for (int i = 0; i < activeContracts.Count; i++)
        {
            if (!activeContracts[i].Active)
            {
                reference = activeContracts[i];
                break;
            }
        }

        // if none available, return false
        if (reference == null) return false;

        reference.SetActive(true, contractRef);

        bool hasEnoughHoomans = GetSelectedContracts().Count >= ContentManager.Instance.LevelData(levelID).amountOfHumans;
        if (hasEnoughHoomans)
        {
            UIManager.Instance.PreGameUI.CanStart(true);
        }

        return true;
    }

    public void RemoveFromActive(Contract contractRef)
    {
        activeContracts.Find(a => a.ContractRef == contractRef).SetActive(false);
        selectableContracts.Find(s => s.ContractRef == contractRef).Selected = false;

        UIManager.Instance.PreGameUI.CanStart(false);
    }

    public List<Contract> GetSelectedContracts()
    {
        List<Contract> contracts = new List<Contract>();

        for (int i = 0; i < activeContracts.Count; i++)
        {
            if (activeContracts[i].Active)
            {
                contracts.Add(activeContracts[i].ContractRef);
            }
        }

        return contracts;
    }
}
