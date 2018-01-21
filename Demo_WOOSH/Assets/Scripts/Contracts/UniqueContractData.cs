using System;
using UnityEngine;

public class UniqueContractData
{
    public int id;
    public int happiness;
    public HumanTypes type;
    public int index;

    public UniqueContractData() { }

    public UniqueContractData(Contract reference)
    {
        id = reference.ID;
        happiness = reference.Happiness;
        type = reference.HumanType;
        index = reference.HumanIndex;
    }

    public UniqueContractData(int id, int happiness, HumanTypes type, int index)
    {
        this.id = id;
        this.happiness = happiness;
        this.type = type;
        this.index = index;
    }
}
