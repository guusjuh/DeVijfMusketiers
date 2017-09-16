using UnityEngine;
using UnityEngine.SceneManagement;

public class Beast : MonoBehaviour
{

    [SerializeField]
    private float chargeTime = 0.5f;
    [SerializeField]
    private float cooldownTime = 1.0f;
    private float[] timers = new float[2];
    private bool[] timerIsActive = new bool[2] { false, false };
    [SerializeField]
    private float vaseChance = 50.0f;
    private int hp = 100;

    private GameObject target;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject targetGraphic;
    private GameObject activeTarget;

    public Random rnd;

    private bool aimed;

    private void Fire()
    {
        Debug.Log("Pieuw!");
        Instantiate(projectile, transform.position + new Vector3(0.0f, 0.0f, 0.1f), transform.rotation);
        SpellManager.Instance.EnemyQueueRemove();

        Destroy(activeTarget);
        activeTarget = null;
        timers[0] = cooldownTime;
        timerIsActive[0] = true;
        //shoot projectile
    }

    public void Attack(GameObject newTarget)
    {
        target = newTarget;
        transform.LookAt(target.transform);
        activeTarget = Instantiate(targetGraphic, target.transform.position, Quaternion.identity);

        //start Timer
        timers[1] = chargeTime;
        timerIsActive[1] = true;
    }

    private GameObject findNewTarget()
    {
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
        GameObject[] vases = GameObject.FindGameObjectsWithTag("Vase");

        int chance = Random.Range(0, 100);
        int rnd = -1;
        int nVases = 0;
        for (int i = 0; i < vases.Length; i++)
        {
            if (!vases[i].GetComponent<Vase>().destroyed)
                nVases++;
        }

        if (nVases <= 0)
        {
            chance = 71;
        }
        if (chance < 70.0f)
        {
            SpellManager.Instance.AddVaseSpell();
            rnd = Random.Range(0, vases.Length);
            
            while (true)
            {
                Vase vase = vases[rnd].GetComponent<Vase>();
                if (vase.destroyed)
                {
                    rnd = Random.Range(0, vases.Length);
                }
                else
                {
                    return vases[rnd];
                }
            }
            
        }
        else
        {
            SpellManager.Instance.AddHumanSpell();
            rnd = Random.Range(0, humans.Length);
            return humans[rnd];
        }
    }

    public void TakeDamage(int amount)
    {
        if (timerIsActive[1])
        {
            timerIsActive[1] = false;
            timers[1] = 0.0f;

            SpellManager.Instance.EnemyQueueRemove();
            if (activeTarget != null)
            {
                Destroy(activeTarget);
                activeTarget = null;
            }
            
            timers[0] = cooldownTime;
            timerIsActive[0] = true;
        }
        hp -= amount;
        BeastHealthBar.Instance.TakeDamage(amount);
        if (hp <= 0)
        {
            SceneManager.LoadScene("Win");
        }
    }

    public void Start()
    {
        timers[0] = cooldownTime;
        timerIsActive[0] = true;

        BeastHealthBar.Instance.maxHealth = hp;
        BeastHealthBar.Instance.health = hp;
    }

    public void SetHighlight(bool value)
    {
        //use child for highlightobj
        transform.GetChild(0).gameObject.SetActive(value);
    }

    // Update is called once per frame
    public void Update()
    {
        //update timers
        for (int i = 0; i < timers.Length; i++)
        {
            if (timerIsActive[i])
            {
                timers[i] -= Time.deltaTime;
                if (timers[i] <= 0.0f)
                {
                    switch (i)
                    {
                        case 0: // cooldown complete
                            //new target
                            //draai naar target
                            //charge twee seconden
                            Attack(findNewTarget());
                            timerIsActive[0] = false;
                            break;
                        case 1: // cooldown complete
                            //attack
                            //charge twee seconden
                            Fire();
                            timerIsActive[1] = false;
                            break;
                    }
                }
            }
        }
    }
}
