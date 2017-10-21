using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveContract : ContractButton
{
    private Image iconImage;
    private Color nonActiveColor = new Color(1.0f, 1.0f, 1.0f, 0.25f);
    private Color activeColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private bool active = false;

    public bool Active
    {
        get { return active; }
        private set
        {
            active = value;
            iconImage.color = active ? activeColor : nonActiveColor;
            if (!active)
            {
                //TODO: reset sprite to default sprite (to be made)
                contractRef = null;
                ClearHeartsAndStars();
            }
            else
            {
                iconImage.sprite = contractRef.Portrait;
                AddHearts(contractRef.Health, contractRef.TotalHealth);
                AddStars(contractRef.Reputation);
            }
        }
    }

    public override void Initialize(Contract contractRef = null)
    {
        iconImage = transform.Find("Image").GetComponent<Image>();
        iconImage.color = nonActiveColor;

        base.Initialize();
    }

    public override void OnClick()
    {
        // deselect on click
        if (active)
        {
            UIManager.Instance.PreGameUI.PreGameInfoPanel.RemoveFromActive(contractRef);
        }

        // dont do anything when not active
    }

    public void SetActive(bool on, Contract contractRef = null)
    {
        if (contractRef == null && on)
        {
            Debug.LogError("Cannot activate without contract reference");
        }

        this.contractRef = contractRef;
        Active = on;
    }
}
