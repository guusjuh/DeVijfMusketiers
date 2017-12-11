using System;
using UnityEngine;

public class SpellFactory {
    public ISpell CreateSpell(SpellProxy proxy)
    {
        int n = 0;
        ISpell spell = new SpellComponent();
        
        //damage variables
        if (proxy.Damage() > 0)
        {
            n++;
            SpellComponent component = new SpellComponent(proxy.Damage(), proxy.HitChance());
            ISpell temp = AddComponent(spell, component, n > 1);
            spell = temp;
        }

        //FireVariables
        if (proxy.FireDamage() > 0 && proxy.FireTurns() > 0)
        {
            n++;
            BurnComponent component = new BurnComponent(proxy.FireDamage(), proxy.FireTurns(), proxy.FireChance());

            spell = AddComponent(spell, component, n > 1);
        }

        //freeze variables
        if (proxy.FreezeTurns() > 0)
        {
            n++;
            FreezeComponent component = new FreezeComponent(proxy.FreezeTurns(), proxy.FreezeChance());

            spell = AddComponent(spell, component, n > 1);
        }

        if (!proxy.IsDirect())
        {
            n++;
            TeleportComponent component = new TeleportComponent(proxy.Range());

            spell = AddComponent(spell, component, n > 1);
        }

        if (n <= 0)
        {
            Debug.LogError("Spell has no components: " + proxy.Name());
        }

        return spell;
    }

    ISpell AddComponent(ISpell spell, SpellComponent component, bool SpellExists)
    {
        if (SpellExists)
        {
            return spell.AddComponent(component);
        }
        else
        {
            return component;
        }
    }
}
