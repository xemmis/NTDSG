using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, ISAlive
{

    private Animator _animator;
    public Action OnDeathAction { get; set; }
    public Action OnHitAction { get; set; }
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public int Strength { get; set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MaxHealth = 100f;
        Health = MaxHealth;
    }

    public void CheckHealth()
    {
        if (Health <= 0) Death();        
    }

    public void Death()
    {
        print("Player Is Dead");
    }

    public void Heal(float healAmount)
    {
        if (healAmount < 0) return;
        if (healAmount + Health > MaxHealth) Health = MaxHealth;
        else Health += healAmount;
    }

    public void TakeHit(float damage)
    {
        _animator.SetTrigger("Hurt");
        OnHitAction?.Invoke();
        if (damage < 0) return;
        Health -= damage;
        CheckHealth();
    }
}