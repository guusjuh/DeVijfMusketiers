using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ContractType
{
    [SerializeField] private HumanTypes type;
    public HumanTypes HumanType { get { return type; } }

    [SerializeField] private int reputation;
    public int Reputation { get { return reputation; } }

    [SerializeField] private int totalHealth;
    public int TotalHealth { get { return totalHealth; } }

    [SerializeField] private Sprite inWorld;
    public Sprite InWorld { get { return inWorld; } }

    [SerializeField] private Sprite portrait;
    public Sprite Portrait { get { return portrait; } }

    [SerializeField] private Rewards rewards;
    public Rewards Rewards { get { return rewards; } }
}
