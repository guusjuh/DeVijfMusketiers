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

    public void Initialize(Sprite sprite)
    {
        image = GetComponent<Image>();

        image.sprite = sprite;
    }

    public void Clear()
    {
        Destroy(this.gameObject);
    }
}
