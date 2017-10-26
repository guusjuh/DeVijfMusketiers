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

        AddHearts(contractRef.Health, contractRef.TotalHealth);
        AddStars(contractRef.Reputation);
    }

    public override void OnClick()
    {
        GameObject acceptWindow = UberManager.Instance.UiManager.LevelSelectUI.SelectContractWindow.SelectionWindow.transform.Find("AcceptContract").gameObject;
        AcceptButton ab = new AcceptButton(acceptWindow, SelectContract);
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
