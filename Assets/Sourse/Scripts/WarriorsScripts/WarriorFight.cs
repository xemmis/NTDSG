using System.Collections;
using UnityEngine;

public class WarriorFight : MonoBehaviour
{
    [SerializeField] private Warrior _warrior;
    [SerializeField] private WarriorMovement _warriorMovement;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _atackTick = 1.5f;
    [SerializeField] private bool _isAttack;
    [SerializeField] private bool _canAttack;
    [SerializeField] private int _animState;
    private void Start()
    {
        _warrior = GetComponent<Warrior>();
        _warriorMovement = GetComponent<WarriorMovement>();
        _animator = GetComponent<Animator>();
        _warriorMovement.OnDistanceToHit += CanHitEnemy;
        _warrior.OnDeathAction += OnDeath;
        _warrior.OnEnemyDeath += ForgetEnemy;
    }

    private void OnDeath()
    {
        Destroy(this);
    }

    private void CanHitEnemy(bool condition)
    {
        _canAttack = condition;

        if (condition == false || _isAttack) return;

        _isAttack = condition;

        ChangeIntAnimation(1);
        InvokeRepeating("HitEnemy", 1, _atackTick);
    }

    private void ForgetEnemy()
    {
        _canAttack = false;
        CancelInvoke("HitEnemy");
    }

    private void ChangeIntAnimation(int anim)
    {
        _animState = anim;
        _animator.SetInteger("AnimState", _animState);
    }

    private void HitEnemy()
    {
        if (!_canAttack) return;
        _animator.SetTrigger("Attack");
        _warrior.EnemyWarrior.TakePhysicalHit(_warrior.Strength);
    }






}



