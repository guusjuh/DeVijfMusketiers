using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectContractWindow
{
    private GameObject selectionWindow;
    public GameObject SelectionWindow { get { return selectionWindow; } }

    private Text timerfield;
    private Button close;
    private Button add4Add;
    private GridLayoutGroup contractGrid;
    private GameObject contractPrefab;

    private List<AvailableContract> availableContractIndicators;
    public List<AvailableContract> AvailableContractIndicators { get { return availableContractIndicators;} }

    private AcceptButton acceptButton;
    public AcceptButton MyAcceptButton
    {
        get {return acceptButton;}
        set { acceptButton = value; }
    }

    private bool active;
    private City city;
    private Destination destination;

    private bool interactable = true;

    private bool Interactable
    {
        get { return interactable;}
        set
        {
            availableContractIndicators.HandleAction(c => c.GetComponent<Button>().interactable = value);
            interactable = value;
        }
    }

    public void SetInteractable()
    {
        Interactable = PathHasSpace();
    }

    public void DisableButtons()
    {
        Interactable = false;
    }

    public SelectContractWindow(GameObject selectionWindow)
    {
        this.selectionWindow = selectionWindow;
        Initialize();
    }

    /// <summary>
    /// returns whether the first level of the path has space, if the window is active
    /// </summary>
    private bool PathHasSpace()
    {
        if (!active) return false;
        Path thisPath = city.Paths.Find(p => p.Destination == destination);
        return UberManager.Instance.ContractManager.AmountOfContracts(thisPath.Levels[0].LevelID) < GameManager.AMOUNT_HUMANS_PER_LEVEL;
    }

    public bool isCitySelected(City city)
    {
        return (this.city == city);
    }

    public void Activate(bool value, City city, Destination destination)
    {
        active = value;
        if (value)
        {
            this.city = city;
            this.destination = destination;
            ShowContracts();
            if (city.MyContractState != City.ContractState.Nothing) city.MyContractState = City.ContractState.Seen;
            SetInteractable();
        }
        else
        {
            this.city = null;
        }
        selectionWindow.gameObject.SetActive(value);
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }

    private void Initialize()
    {
        availableContractIndicators = new List<AvailableContract>();
        timerfield = selectionWindow.transform.Find("Timer").GetComponent<Text>();

        contractGrid = selectionWindow.transform.Find("ContractGrid").GetComponent<GridLayoutGroup>();
        contractPrefab = Resources.Load<GameObject>("Prefabs/UI/LevelSelect/SelectContract/AvailableContractButton").gameObject;

        add4Add = selectionWindow.transform.Find("Add4Add").GetComponent<Button>();

        close = selectionWindow.transform.Find("Close").GetComponent<Button>();
        close.onClick.AddListener(DisableAcceptButton);
        close.onClick.AddListener(Deactivate);
    }

    private void DisableAcceptButton()
    {
        if (acceptButton != null)
        {
            acceptButton.DisableWindow();
            SetInteractable();
        }
    }

    public void Refresh(City city, Destination destination)
    {
        if (isCitySelected(city))
        {
            this.destination = destination;
            ClearContracts();
            selectionWindow.transform.Find("AcceptContract").gameObject.SetActive(false);
            if (active) city.MyContractState = City.ContractState.Seen; 
            ShowContracts();
        }
        else
        {
            Debug.LogWarning("Tried to refresh contracts with wrong city");
        }
    }

    public void Remove(AvailableContract toRemove)
    {
        if (availableContractIndicators.Contains(toRemove))
        {
            city.RemoveAvailableContract(toRemove.ContractRef, destination);
            GameObject.Destroy(availableContractIndicators.Find(c => c == toRemove).gameObject);
            availableContractIndicators.Remove(toRemove);
        }
    }

    public void Deactivate()
    {
        ClearContracts();
        active = false;
        city = null;
        selectionWindow.gameObject.SetActive(false);
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }

    public void SetTimer(TimeSpan remainingTime)
    {
        timerfield.text = " Refresh: " + remainingTime.Hours.ToString("00") + ":" + remainingTime.Minutes.ToString("00") + ":" + remainingTime.Seconds.ToString("00");
    }

    private void ClearContracts()
    {
        if (availableContractIndicators.Count > 0)
        {
            for (int i = 0; i < availableContractIndicators.Count; i++)
            {
                GameObject.Destroy(availableContractIndicators[i].gameObject);
                availableContractIndicators[i] = null;
            }
            availableContractIndicators.Clear();
            availableContractIndicators = new List<AvailableContract>();
        }
        else
        {
            Debug.LogWarning("nothing to delete");
        }
    }

    private void ShowContracts()
    {
        if (!active)
        {
            Debug.LogError("SelectContractWindow inactive: can't show available contracts...");
            return;
        }

        for (int i = 0; i < city.AvailableContracts[destination].Count; i++)
        {
            availableContractIndicators.Add(UIManager.Instance.CreateUIElement(contractPrefab, Vector2.zero, contractGrid.transform).GetComponent<AvailableContract>());
            availableContractIndicators.Last().Initialize(city.AvailableContracts[destination][i]);
        }
    }


}
