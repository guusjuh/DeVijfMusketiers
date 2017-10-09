using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfoPanel : MonoBehaviour
{
    private Text statusText;

    private const string WIN_STRING = "Level completed!";
    private const string LOSE_STRING = "Defeated...";

    private enum HumanPostGameStatus
    {
        MoveOn = 0,
        Stay,
        BrokeContract
    }

    private Dictionary<HumanPostGameStatus, PostGameInfo> postGameInfo;

    public void Initialize()
    {
        statusText = transform.Find("StatusText").GetComponent<Text>();

        SetText();

        postGameInfo = new Dictionary<HumanPostGameStatus, PostGameInfo>();
        postGameInfo.Add(HumanPostGameStatus.MoveOn, transform.Find("Won").GetComponent<PostGameInfo>());
        postGameInfo.Add(HumanPostGameStatus.Stay, transform.Find("Stay").GetComponent<PostGameInfo>());
        postGameInfo.Add(HumanPostGameStatus.BrokeContract, transform.Find("BrokeContract").GetComponent<PostGameInfo>());

        //TODO: this is ONLY for WIN in level 3!!! THIS IS VERY SPECIAL CASE and it should be replaced as soon as we know how finsihing a contract works!!!
        if (GameManager.Instance.CurrentLevel == 2) postGameInfo.Get(HumanPostGameStatus.MoveOn).SpecialInitialize(GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));
        else postGameInfo.Get(HumanPostGameStatus.MoveOn).Initialize(GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));

        postGameInfo.Get(HumanPostGameStatus.Stay).Initialize(GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Health > 0));
        postGameInfo.Get(HumanPostGameStatus.BrokeContract).Initialize(GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Health <= 0));
    }

    public void Restart()
    {
        SetText();

        //TODO: this is ONLY for WIN in level 3!!! THIS IS VERY SPECIAL CASE and it should be replaced as soon as we know how finsihing a contract works!!!
        if (GameManager.Instance.CurrentLevel == 2) postGameInfo.Get(HumanPostGameStatus.MoveOn).SpecialInitialize(GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));
        else postGameInfo.Get(HumanPostGameStatus.MoveOn).Restart(GameManager.Instance.SelectedContracts.FindAll(c => !c.Died));

        postGameInfo.Get(HumanPostGameStatus.Stay).Restart(GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Health > 0));
        postGameInfo.Get(HumanPostGameStatus.BrokeContract).Restart(GameManager.Instance.SelectedContracts.FindAll(c => c.Died && c.Health <= 0));
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

        for (int i = 0; i < GameManager.Instance.SelectedContracts.Count; i++)
        {
            if (!GameManager.Instance.SelectedContracts[i].EndLevel())
            {
                i--;
            }
        }
    }
}
