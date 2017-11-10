using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Destination
{
    NoDestination = -1,
    Red = 0,
    Green,
    Blue,
    Purple,
    Tutorial
}

public class City : MonoBehaviour {
    private Button bttn;

    private List<Path> paths;
    
    private ContractState contractState = ContractState.Nothing;

    public ContractState MyContractState
    {
        get { return contractState;}
        set
        {
            contractState = value;
            nCI.SetState(value);
        }
    }

    private NewContractIndicator nCI;

    //TODO: implement different destinations
    [SerializeField] private Destination thisCity;
    [SerializeField] private Destination destination;
    public Destination ThisCity { get { return thisCity; } }
    private Dictionary<Destination, List<Contract>> availableContracts;
    public Dictionary<Destination, List<Contract>> AvailableContracts { get { return availableContracts;} }

    public List<Path> Paths { get { return paths; } }

    // true when the player has reached the city for the first time
    private bool cityReached = false;
    public bool CityReached { get { return cityReached; } }
    public void Reached()
    {
        if (cityReached) return;
        if (bttn == null) bttn = gameObject.GetComponentInChildren<Button>();

        if (destination != Destination.NoDestination)
        {
            bttn.interactable = true;
            bttn.onClick.AddListener(delegate
            {
                UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.Activate(true, this, destination);
            });
        }
        else
        {
            bttn.interactable = true;
            bttn.enabled = true;
        }

        cityReached = true;
    } 

    public void Initiliaze()
    {
        bttn = gameObject.GetComponentInChildren<Button>();

        // any city with a path to a different destination
        if (destination != Destination.NoDestination)
        {
            paths = new List<Path>();
            availableContracts = new Dictionary<Destination, List<Contract>>();

            for (int i = 2; i < transform.childCount; i++)
            {
                paths.Add(new Path(transform.GetChild(i), this, destination));
                availableContracts.Add(destination, new List<Contract>());
            }

            bttn.interactable = false;

            nCI = new NewContractIndicator(transform.GetChild(1).gameObject);
        }
        // the last city in the game
        else
        {
            // nothing for now
            bttn.interactable = false;
        }

        //TODO: find right path, by checking the contract destination
    }

    public void RefreshAvailableContracts(List<Contract> newContracts, Destination destination)
    {
        ClearAvailableContracts(destination);
        AddAvailableContracts(newContracts, destination);

        MyContractState = ContractState.New;

        if (UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.isCitySelected(this))
        {
            UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.Refresh(this, destination);
        }
    }

    public void RefreshAvailableContracts(Contract newContract, Destination destination)
    {
        List<Contract> contracts = new List<Contract> {newContract};
        RefreshAvailableContracts(contracts, destination);
    }

    private void AddAvailableContracts(List<Contract> newContracts, Destination destination)
    {
        availableContracts[destination].AddRange(newContracts);
    }

    public void SawNewContracts()
    {
        MyContractState = ContractState.Seen;
    }

    public void RemoveAvailableContract(Contract toRemove, Destination destination)
    {
        if (availableContracts[destination].Contains(toRemove))
        {
            availableContracts[destination].Remove(toRemove);
            if (availableContracts[destination].Count <= 0) MyContractState = ContractState.Nothing;
        }
    }

    private void ClearAvailableContracts(Destination destination)
    {
        //moet dit?
        availableContracts[destination].HandleAction(c => UberManager.Instance.ContractManager.RemoveContract(c));
        availableContracts[destination].Clear();
        availableContracts[destination] = new List<Contract>();
    }

    public void Clear()
    {
        //TODO: implement clear
        paths.HandleAction(p => p.Clear());
    }

    public void Restart()
    {
        //TODO: implement Restart
        paths.HandleAction(p => p.Restart());
        //TODO: multiple paths
    }

    public enum ContractState
    {
        Nothing = 0,
        Seen = 1,
        New = 2
    }
}
