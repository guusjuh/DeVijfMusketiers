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
            SpellComponent component = new SpellComponent(proxy.Cost(), proxy.IsDirect(), proxy.Damage(), proxy.HitChance());
            spell = AddComponent(spell, component, n > 1);
        }

        //FireVariables
        if (proxy.FireDamage() > 0 && proxy.FireTurns() > 0)
        {
            n++;
            int cost = proxy.Cost() - spell.Cost();
            BurnComponent component = new BurnComponent(cost, proxy.IsDirect(), proxy.FireDamage(), proxy.FireTurns(), proxy.FireChance());
            
            spell = AddComponent(spell, component, n > 1);
        }

        //freeze variables
        if (proxy.FreezeTurns() > 0)
        {
            n++;
            int cost = proxy.Cost() - spell.Cost();
            FreezeComponent component = new FreezeComponent(cost, proxy.IsDirect(), proxy.FreezeTurns(), proxy.FreezeChance());

            spell = AddComponent(spell, component, n > 1);
        }

        if (!proxy.IsDirect())
        {
            n++;
            int cost = proxy.Cost() - spell.Cost();
            TeleportComponent component = new TeleportComponent(cost, proxy.Range());

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
