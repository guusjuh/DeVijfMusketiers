using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpellPlacement : MonoBehaviour {

    public bool spellAreaChoosing = false;
    public Sprite circleRed;
    public Sprite circleBlue;
    public Sprite circleGreen;
    Vector2 mousePos;
    public bool attackOn = false;
    public bool protectOn = false;
    public bool repairOn = false;
    public bool attackOn2 = false;
    public bool protectOn2 = false;
    public bool repairOn2 = false;
    Bed selectedBed;
    public bool enemyHasBeenAttacked = false;
    public float timeLeft;

    private int cdt;
    private string nme;


    // Use this for initialization
    void Start () {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (spellAreaChoosing)
        {
            Debug.Log("spellAreaChoosing");
            if (Input.GetMouseButton(0))
            {
                Debug.Log("keydown");
                mousePos = Input.mousePosition;
                gameObject.GetComponent<Renderer>().enabled = true;
                gameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(mousePos).x, 0, (Camera.main.ScreenToWorldPoint(mousePos).z));
            }
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("keyup");

                if (attackOn)
                {
                    attackOn = false;
                    attackOn2 = true;
                    enemyHasBeenAttacked = true;
                    Bed[] bedObjects = FindObjectsOfType(typeof(Bed)) as Bed[];
                    for(int i = 0; i < bedObjects.Length; i++)
                    {
                        bedObjects[i].GetComponent<Bed>().canBeAttacked = true;
                    }                    
                }
                if (protectOn)
                {
                    protectOn = false;
                    protectOn2 = true;                    
                }
                if (repairOn)
                {
                    repairOn = false;
                    repairOn2 = true;                    
                }
                gameObject.GetComponent<Renderer>().enabled = false;
                spellAreaChoosing = false;
            }
            
        }
	}

    void OnTriggerStay(Collider other)
    {
        List<GameObject> currentCollisions = new List<GameObject>();
        currentCollisions.Add(other.gameObject);

        foreach (GameObject gObject in currentCollisions)
        {
            if (gObject.tag == "enemy" && attackOn2)
            {                
                if(enemyHasBeenAttacked)
                {
                    Shadow enemy = FindObjectOfType(typeof(Shadow)) as Shadow;
                    if (gObject.GetComponent<Shadow>().ShieldTimer <= 0)
                    {
                        enemy.health -= 10;
                    }
                    else
                    {
                        gObject.GetComponent<Shadow>().ShieldTimer -= 5;
                    }
                    enemyHasBeenAttacked = false;
                }
                
                
            }
            else if (gObject.tag == "human" && attackOn2)
            {
                if (gObject.GetComponent<Bed>().ShieldTimer >= 0)
                {
                    if(gObject != null && gObject.GetComponent<Bed>().canBeAttacked)
                    {
                        gObject.GetComponent<Bed>().canBeAttacked = false;
                        gObject.GetComponent<Bed>().ShieldTimer -= 5;
                    }
                    
                }
                else
                {
                    if(gObject != null && gObject.GetComponent<Bed>().canBeAttacked)
                    {
                        gObject.GetComponent<Bed>().canBeAttacked = false;
                        Destroy(gObject);
                        Bed[] bedObjects = FindObjectsOfType(typeof(Bed)) as Bed[];
                        if (bedObjects.Length <= 0)
                        {
                            SceneManager.LoadScene("GameOver");
                        }
                    }
                    
                }

            }

            else if (gObject.tag == "repairobject" && attackOn2)
            {
                gObject.GetComponent<Shake>().destroyed = true;
            }

            else if (gObject.tag == "human" && protectOn2)
            {
                gObject.GetComponent<Bed>().ShieldTimer = 15;
            }

            else if (gObject.tag == "enemy" && protectOn2)
            {
                Shadow enemy = FindObjectOfType(typeof(Shadow)) as Shadow;
                gObject.GetComponent<Shadow>().ShieldTimer = 15;
            }
            else if (gObject.tag == "repairobject" && repairOn2)
            {
                gObject.GetComponent<Shake>().destroyed = false;
            }

        }
        currentCollisions.Clear();
    }


    public void ChooseSpellPlace(string name, float timer)
    {
        timeLeft = timer;
        nme = name;
        attackOn2 = false;
        protectOn2 = false;
        repairOn2 = false;
       
        spellAreaChoosing = true;
        if (name == "CubeAttack")
        {
            attackOn = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = circleRed;

        }
        if (name == "CubeProtect")
        {
            protectOn = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = circleBlue;
        }
        if (name == "CubeRepair")
        {
            repairOn = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = circleGreen;
        }
        
        
        
    }
}
