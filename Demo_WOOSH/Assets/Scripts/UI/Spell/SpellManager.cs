using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellManager {
    [SerializeField]
    private List<SpellProxy> spellProxies;

    [SerializeField]
    private GameObject emptySpellButtonPrefab;
    private List<SpellButton> spellButtons = new List<SpellButton>();
    private List<ISpell> spells = new List<ISpell>();
    private SpellFactory factory = new SpellFactory();

    public void Initialize()
    {
        if (spellProxies.Count <= 0)
        {
            Debug.LogWarning("No spells have been found!");
            return;
        }

        for (int i = 0; i < spellProxies.Count; i++)
        {
            spells.Add(factory.CreateSpell(spellProxies[i]));

            //TODO: spellbuttons
            //-create spellbuttons
            //spellButtons[i] = GameObject.Instantiate(emptySpellButtonPrefab).GetComponent<SpellButton>();
            //-assign sprites & description
            //spellButtons[i].Initialize(spells[i], spellProxies[i].Description(), spellProxies[i].SpellSprite());
        }
    }
}
