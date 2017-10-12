using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YourTurn : MonoBehaviour {

    public void UpdateTurn()
    {
        Text text = this.GetComponentInChildren<Text>();
        int turnCount = GameManager.Instance.LevelManager.AmountOfTurns;

        text.text = "YOUR TURN: " + turnCount;
    }
}
