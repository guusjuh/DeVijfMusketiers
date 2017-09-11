using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverManager : MonoBehaviour {
	void Update ()
    {
        if (Input.anyKey)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("testscene");
        }
    }
}
