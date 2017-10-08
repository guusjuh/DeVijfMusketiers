using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractButton : MonoBehaviour
{
    protected Contract contractRef;
    public Contract ContractRef { get { return contractRef; } }

    public virtual void Initialize(Contract contractRef = null)
    {
        this.contractRef = contractRef;
    }

    public virtual void OnClick() { }

    public virtual void Clear()
    {
        //TODO: make contract ref aware of it being selected for the current level!

        Destroy(this.gameObject);
    }
}
