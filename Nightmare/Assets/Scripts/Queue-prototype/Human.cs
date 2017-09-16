using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Human : Item
{
    private float shieldTime = 3.0f;
    private float timer = 0.0f;
    private bool shielded = false;
    public override void Hit()
    {
        if (GameObject.FindGameObjectsWithTag("Human").Length <= 1)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Projectile" && !shielded)
        {
            Hit();
        }
    }

    public void Shield(bool value)
    {
        if (value)
        {
            timer = shieldTime;
            GetComponent<MeshRenderer>().material.color = new Color(0.0f, 0.0f, 1.0f);
        }
        else
        {
            timer = 0.0f;
            GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f);
        }
        shielded = value;
    }

    public void Update()
    {
        if (shielded)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                Shield(false);
            }
        }
    }
}
