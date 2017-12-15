using UnityEngine;

public interface ISpell
{
    SpellComposite AddComponent(SpellComponent component);
    bool ApplyEffects(WorldObject target, float rnd);
    bool Execute(WorldObject target, float rnd, bool endTurn);
    void CastSpell(WorldObject target);

    int Cost();
    int Damage();
    float HitChance();
    int FireDamage();
    int FireTurns();
    int FreezeTurns();
    bool IsDirect();
    int Range();
}
