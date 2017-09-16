using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellButton : MonoBehaviour
{
    [SerializeField] private float totalCooldown;
    private float currentCooldown;

    [SerializeField] private GameObject spellFigure;

	// Use this for initialization
	void Start ()
	{
	    currentCooldown = 0;
	}
	
	// Update is called once per frame
	void Update () {
	    if (currentCooldown > 0)
	    {
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.25f, 0.25f, 0.25f, 1);
            currentCooldown -= Time.deltaTime;
        }
	    else
	    {
            GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void CastSpell()
    {
        if (currentCooldown <= 0)
        {
            currentCooldown = totalCooldown;
            spellFigure.SetActive(true);
            spellFigure.GetComponent<Spell>().StartSpell();
        }
    }
}
