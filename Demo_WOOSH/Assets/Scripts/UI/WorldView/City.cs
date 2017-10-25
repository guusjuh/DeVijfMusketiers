using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {
    private Button newContractButton;
    private List<Path> paths;
    public List<Path> Paths { get { return paths; } }
    private int cityId;
    public int CityId { get { return cityId; } }


    public void Initiliaze()
    {
        paths = new List<Path>();
        for (int i = 1; i < transform.childCount; i++)
        {
            paths.Add(new Path(transform.GetChild(i), this, i));
        }
        gameObject.GetComponent<Button>().onClick.AddListener(ActivateContractButton);

        newContractButton = transform.GetChild(0).GetComponent<Button>();
        
        //TODO: find right path, by checking the contract destination
        newContractButton.onClick.AddListener(delegate { SpawnContract(paths[0]); });
    }

    public void ActivateContractButton()
    {
        newContractButton.gameObject.SetActive(!newContractButton.gameObject.activeInHierarchy);
    }

    public void SpawnContract(Path path)
    {
        //spawning a contract, so always level 0
        bool spaceInThisLevel = UberManager.Instance.ContractManager.AmountOfContracts(path.Levels[0].LevelID) < 6;

        if (spaceInThisLevel)
        {
            GenerateRandomContract(path);
            path.Levels.HandleAction(l => l.CheckActiveForButton());

            spaceInThisLevel = UberManager.Instance.ContractManager.AmountOfContracts(path.Levels[0].LevelID) < 6;
            newContractButton.interactable = spaceInThisLevel;
        }
    }

    public Contract GenerateRandomContract(Path path)
    {
        int id = UberManager.Instance.ContractManager.AmountOfContracts();

        // get random type   
        HumanTypes type = UberManager.Instance.ContractManager.GetRandomHumanType();
        List<ContractType> matchingContractTypes = UberManager.Instance.ContractManager.ContractTypes.FindAll(c => c.HumanType == type);

        Contract newContract = new Contract(id, matchingContractTypes[UnityEngine.Random.Range(0, matchingContractTypes.Count)], path);
        UberManager.Instance.ContractManager.AddContract(newContract);
        path.Levels[0].AddHuman();
        return newContract;
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
        bool spaceInThisLevel = UberManager.Instance.ContractManager.AmountOfContracts(paths[0].Levels[0].LevelID) < 6;
        newContractButton.interactable = spaceInThisLevel;
    }
}
