using UnityEngine;
using UnityEngine.UI;

public class SpellButton : MonoBehaviour {
    private Sprite SpellSprite { set { gameObject.GetComponent<Image>().sprite = value; } }
    private string description;
    private ISpell spell;

    public void Initialize(ISpell spell, string description, Sprite sprite)
    {
        this.spell = spell;
        this.description = description;
        SpellSprite = sprite;
    }

    public void OnClick()
    {
        //spell.
    }
}
