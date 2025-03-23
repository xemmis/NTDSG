using UnityEngine;

public interface ISpell
{
    public float SpellDuration { get; set; }
    public float SpellDamage { get; set; }
    public float SpellCost { get; set; }

    public void TakeHeroComponent(Transform heroTransform);
    public void SpellActivate();
    public void SpellDeactivate();
}

