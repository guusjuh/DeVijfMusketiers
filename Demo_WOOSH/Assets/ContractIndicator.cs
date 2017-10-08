using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractIndicator : MonoBehaviour
{
    private Image image;
    private Contract contractRef;

    public void Initialize(Contract contractRef)
    {
        image = GetComponent<Image>();

        this.contractRef = contractRef;

        image.sprite = contractRef.InWorld;
    }

    public void Clear()
    {
        //TODO: make contract ref aware of it being selected for the current level!

        Destroy(this.gameObject);
    }
}
