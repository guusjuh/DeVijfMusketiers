using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairSpell : MonoBehaviour
{
    float cooldownTime = 1.5f;
    float cooldown;
    public GameObject circle;
    public Shake target;
    Manager manager;

    // Use this for initialization
    void Start()
    {
        manager = FindObjectOfType(typeof(Manager)) as Manager;
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldown > 0)
        {
            GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f, 1);
            cooldown -= Time.deltaTime;
            circle.SetActive(true);
            circle.transform.localScale = new Vector3(2f + cooldown, 0.2f, 2f + cooldown);

            if (cooldown < 0.5f)
            {
                circle.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
            }
            else
            {
                circle.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
            }
        }
        else if (manager.RepairCooldown > 0)
        {
            circle.SetActive(false);
            GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f, 1);
        }
        else
        {
            circle.SetActive(false);
            GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }

    void OnMouseDown()
    {
        if (manager.RepairCooldown <= 0)
        {
            //add global cooldown
            if (cooldown <= 0)
            {
                cooldown = cooldownTime;
                manager.RepairCooldown = 4;
            }
        }
        else if (cooldown <= 0.5f && cooldown > 0)
        {
            target.destroyed = false;
            cooldown = -1;
        }
        else
        {
            cooldown = -1;
        }
    }
}
