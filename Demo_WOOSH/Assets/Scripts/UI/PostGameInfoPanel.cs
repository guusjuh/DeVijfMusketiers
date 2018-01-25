using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfoPanel : MonoBehaviour
{
    private static bool nearDeadWarningGiven = false;
    private enum HumanPostGameStatus {
        MoveOn = 0,
        Stay,
        Dead
    }

    private Text statusText;
    private const string WIN_STRING = "Level completed!";
    private const string LOSE_STRING = "Defeated...";

    private const string WIN = " humans moved onto the next level";
    private const string STAY = " humans stayed in this level";
    private const string DEAD = " humans broke contract";

    private Dictionary<HumanPostGameStatus, PostGameInfo> postGameInfo;

    private PostGameInfoReputation reputation;

    public void Initialize()
    {
        statusText = transform.Find("StatusText").GetComponent<Text>();
        reputation = transform.Find("Reputation").GetComponent<PostGameInfoReputation>();

        List<Contract> deadContracts = new List<Contract>();
        deadContracts = GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Happiness <= 0);
        bool nearDead = GameManager.Instance.SelectedContracts.FindAll(c => c.Happiness <= 10 && c.Happiness > 0).Count > 0;

        if (deadContracts.Count > 0)
        {
            //dialog

            UIManager.Instance.LevelSelectUI.SetUpDeadContractDialog(true);
        } else if (nearDead && !nearDeadWarningGiven)
        {
            UIManager.Instance.LevelSelectUI.SetUpDeadContractDialog(false);
            nearDeadWarningGiven = true;
        }

        SetText();

        postGameInfo = new Dictionary<HumanPostGameStatus, PostGameInfo>();
        postGameInfo.Add(HumanPostGameStatus.MoveOn, transform.Find("Won").GetComponent<PostGameInfo>());
        postGameInfo.Add(HumanPostGameStatus.Stay, transform.Find("Stay").GetComponent<PostGameInfo>());
        postGameInfo.Add(HumanPostGameStatus.Dead, transform.Find("BrokeContract").GetComponent<PostGameInfo>());

        //TODO: this is ONLY for WIN in level 3!!! THIS IS VERY SPECIAL CASE and it should be replaced as soon as we know how finsihing a contract works!!!
        if (!GameManager.Instance.SelectedContracts[0].HasNextLevel()) postGameInfo.Get(HumanPostGameStatus.MoveOn).SpecialInitialize(GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));
        else postGameInfo.Get(HumanPostGameStatus.MoveOn).Initialize(WIN, GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));

        postGameInfo.Get(HumanPostGameStatus.Stay).Initialize(STAY, GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Happiness > 0));
        
        postGameInfo.Get(HumanPostGameStatus.Dead).Initialize(DEAD, deadContracts);

        float startRep = UberManager.Instance.PlayerData.Reputation;
        calculateRep();
        float endRep = UberManager.Instance.PlayerData.Reputation;
        reputation.Initialize(startRep, endRep);

        GooglePlayScript.Instance.SaveData();
    }

    public void Restart()
    {
        List<Contract> deadContracts = new List<Contract>();
        deadContracts = GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Happiness <= 0);
        bool nearDead = GameManager.Instance.SelectedContracts.FindAll(c => c.Happiness <= 10 && c.Happiness > 0).Count > 0;

        if (deadContracts.Count > 0)
        {
            //dialog

            UIManager.Instance.LevelSelectUI.SetUpDeadContractDialog(true);
        }
        else if (nearDead && !nearDeadWarningGiven)
        {
            UIManager.Instance.LevelSelectUI.SetUpDeadContractDialog(false);
            nearDeadWarningGiven = true;
        }

        SetText();

        //TODO: this is ONLY for WIN in level 3!!! THIS IS VERY SPECIAL CASE and it should be replaced as soon as we know how finsihing a contract works!!!
        if (!GameManager.Instance.SelectedContracts[0].HasNextLevel()) postGameInfo.Get(HumanPostGameStatus.MoveOn).SpecialInitialize(GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));
        else postGameInfo.Get(HumanPostGameStatus.MoveOn).Restart(WIN, GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));

        postGameInfo.Get(HumanPostGameStatus.Stay).Restart(STAY, GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Happiness > 0));
        postGameInfo.Get(HumanPostGameStatus.Dead).Restart(DEAD, GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Happiness <= 0));

        float startRep = UberManager.Instance.PlayerData.Reputation;
        calculateRep();
        float endRep = UberManager.Instance.PlayerData.Reputation;
        reputation.Restart(startRep, endRep);

        GooglePlayScript.Instance.SaveData();
    }

    private void SetText()
    {
        statusText.text = GameManager.Instance.Won ? WIN_STRING : LOSE_STRING;
    }

    public void Clear()
    {
        foreach (var p in postGameInfo)
        {
            p.Value.Clear();
        }
    }

    private void calculateRep()
    {
        for (int i = 0; i < GameManager.Instance.SelectedContracts.Count; i++)
        {
            if (!GameManager.Instance.SelectedContracts[i].EndLevel())
            {
                i--;
            }
        }
    }
}
