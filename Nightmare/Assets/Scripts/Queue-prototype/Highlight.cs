using UnityEngine;

public class Highlight : MonoBehaviour
{
    public highlightType type;

    void OnTriggerStay(Collider other)
    {
        
        if (other.tag == "Mouse")
        {
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Clicked " + transform.parent.gameObject.name);
                SpellManager.Instance.PlayerQueueRemove(0);
                SpellManager.Instance.RemoveHighlights();

                switch (type)
                {
                    case highlightType.Vase:
                        transform.parent.GetComponent<Vase>().Repair();
                        break;
                    case highlightType.Human:
                        transform.parent.GetComponent<Human>().Shield(true);
                        break;
                    case highlightType.Beast:
                        transform.parent.GetComponent<Beast>().TakeDamage(SpellManager.Instance.attackDamage);
                        break;
                    default:
                        break;
                }
                
                
                
            }
        }
    }
}

public enum highlightType
{
    Vase,
    Human,
    Beast
}
