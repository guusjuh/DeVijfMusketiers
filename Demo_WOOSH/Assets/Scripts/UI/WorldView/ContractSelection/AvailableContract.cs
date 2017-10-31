using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvailableContract : ContractButton {

    public override void Initialize(Contract contractRef = null)
    {
        base.Initialize(contractRef);

        if (contractRef == null)
        {
            Debug.LogError("A selectable contract needs a contract reference.");
            return;
        }

        transform.Find("Image").GetComponent<Image>().sprite = contractRef.Portrait;

        SetHappiness(contractRef.Happiness, contractRef.TotalHappiness);
        AddStars(contractRef.Reputation);
    }

    public override void OnClick()
    {
        GameObject acceptWindow = UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SelectionWindow.transform.Find("AcceptContract").gameObject;
        UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.DisableButtons();
        UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.MyAcceptButton = new AcceptButton(acceptWindow, SelectContract, "", UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SetInteractable);
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
    }

    public void SelectContract()
    {
        if (contractRef.MyPath.SpawnContract(contractRef))
        {
            UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.Remove(this);
            UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SetInteractable();
        }
    }
}
