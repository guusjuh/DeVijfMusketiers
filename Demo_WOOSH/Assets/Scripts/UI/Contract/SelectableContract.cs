using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableContract : ContractButton
{
    private Image checkImage;
    private bool selected = false;

    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            checkImage.gameObject.SetActive(selected);
        }
    }

    public override void Initialize(Contract contractRef = null)
    {
        base.Initialize(contractRef);

        if (contractRef == null)
        {
            Debug.LogError("A selectable contract needs a contract reference.");
            return;
        }

        selected = false;

        transform.Find("Image").GetComponent<Image>().sprite = contractRef.Portrait;

        checkImage = transform.Find("Check").GetComponent<Image>();
        checkImage.gameObject.SetActive(false);

        SetHappiness(contractRef.Happiness, contractRef.TotalHappiness);
        AddStars(contractRef.Reputation);
    }

    public override void OnClick()
    {
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.ButtonClick);
        // select if not selected
        if (!selected)
        {
            bool succeed = UIManager.Instance.PreGameUI.PreGameInfoPanel.AddToActive(contractRef);
            if (succeed) Selected = true;
        }
        else
        {
            UIManager.Instance.PreGameUI.PreGameInfoPanel.RemoveFromActive(contractRef);
        }
    }
}
