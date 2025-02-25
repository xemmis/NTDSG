using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour, ISAlive
{
    [SerializeField] private CharacterData _characterData;
    private Animator _animator;

    public Action OnHitAction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _characterData.Initialize();
    }

    public void TakeHit(float damage)
    {
        if (damage < 0) return;
        _animator.SetTrigger("Hurt");
        OnHitAction?.Invoke();
        _characterData.TakeDamage(damage);
    }
}