using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, ISAlive
{
    private Animator _handler;

    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; } = 40;
    [field: SerializeField] public int Agility { get; set; } = 5;
    [field: SerializeField] public int Strength { get; set; } = 5;
    [field: SerializeField] public int Armor { get; set; } = 5;
    public Action OnDeathAction { get; set; }
    public Action OnHitAction { get; set; }

    private void Awake()
    {
        _handler = GetComponent<Animator>();
    }

    private void Start()
    {
        Health = MaxHealth;
        Upgrade.OnArmorUpgrade += ArmorUprgade;
        Upgrade.OnWeaponUpgrade += WeaponUprgade;
    }

    private void ArmorUprgade()
    {
        Armor += 2;
    }

    private void WeaponUprgade()
    {
        Strength += 2;
    }

    public void TakeHit(float Damage)
    {
        Health -= Damage;
        CheckHealth();
       _handler.SetTrigger("Hurt");
        OnHitAction?.Invoke();
    }

    public void Heal(float healAmount)
    {
        throw new NotImplementedException();
    }

    public void CheckHealth()
    {
        if (Health > 0) return;
        _handler.SetTrigger("Death");
        OnDeathAction?.Invoke();
    }

    public void Death()
    {
        throw new NotImplementedException();
    }
}
