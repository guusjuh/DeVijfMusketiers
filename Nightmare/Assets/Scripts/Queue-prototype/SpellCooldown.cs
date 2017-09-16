using UnityEngine;
using UnityEngine.UI;

public class SpellCooldown : MonoBehaviour
{
    private float timer;
    [SerializeField]
    private float maxTime = 4.0f;

    public bool active = false;
    public Button btn;

    public void Start()
    {
        btn = GetComponent<Button>();
    }

    public void Activate()
    {
        timer = maxTime;
        active = true;
    }

    public void Update()
    {
        if (active)
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                active = false;
            }
        }
        if (btn.interactable && active)
        {
            btn.interactable = false;
        }
        if (!btn.interactable && !active)
        {
            btn.interactable = true;
        }
    }
}
