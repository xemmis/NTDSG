using System;

public interface ISAlive
{
    public Action OnDeathAction { get; set; }
    public Action OnHitAction { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public int Strength { get; set; }

    public void TakeHit(float damage);
    public void Heal(float healAmount);
    public void CheckHealth();
    public void Death();

}
