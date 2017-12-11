using UnityEngine;

public interface ISpell
{
    SpellComposite AddComponent(SpellComponent component);
    bool ApplyEffects(WorldObject target);
    void Execute(WorldObject target);
    void CastSpell(WorldObject target);


    int Damage();
    float HitChance();
    int FireDamage();
    int FireTurns();
    int FreezeTurns();
    bool IsDirect();
    int Range();
}
