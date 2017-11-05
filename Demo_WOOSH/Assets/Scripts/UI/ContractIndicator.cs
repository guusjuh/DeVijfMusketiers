using UnityEngine;
using UnityEngine.UI;

public class ContractIndicator : MonoBehaviour
{
    private Image image;

    public void Initialize(Contract contractRef)
    {
        image = GetComponent<Image>();

        image.sprite = contractRef.InWorldSprite;
    }

    public void Clear()
    {
        Destroy(this.gameObject);
    }
}
