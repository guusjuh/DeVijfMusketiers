using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyQueue : Queue {

    public override void Start()
    {
        amountOfSpells = 3;
        queueLength = 1;
        base.Start();
    }
    
}
