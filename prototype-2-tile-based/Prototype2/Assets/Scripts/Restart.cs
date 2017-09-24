using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Restart : MonoBehaviour {
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Win")
        {
            FindObjectOfType<Text>().text = "You succefully defeated the enemy" +
                                            (FindObjectOfType<AliveHumans>().deadHumans <= 0
                ? "!"
                : ", but you lost " + FindObjectOfType<AliveHumans>().deadHumans + " humans trying to survive...");
        }
    }

    void Update ()
	{
	    if (Input.anyKey)
            Application.LoadLevel("InGame");
	}
}
