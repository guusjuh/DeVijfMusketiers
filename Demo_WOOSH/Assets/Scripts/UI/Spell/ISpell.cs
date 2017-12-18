using UnityEngine;

public interface ISpell
{
    SpellComposite AddComponent(SpellComponent component);
    bool DoesHit(WorldObject target, float rnd);
    bool Execute(WorldObject target, float rnd, bool endTurn);
    void HighlightTiles(WorldObject target);
    void CastSpell(WorldObject target);

    SpellManager.SpellType Type();
    int Cost();
    int Damage();
    float HitChance();
    int FireDamage();
    int FireTurns();
    int FreezeTurns();
    bool IsDirect();
    int Range();
}
