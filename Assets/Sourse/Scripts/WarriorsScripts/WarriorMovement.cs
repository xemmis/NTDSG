using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WarriorMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private CircleCollider2D _attackRadius;
    private Warrior _thisWarrior;
    private Animator _animator;
    private Tween _tween;
    private int _animState;
    private bool _canHit;

    [SerializeField] private Transform _protectPosition;
    [SerializeField] private float _followTick = .25f;
    [SerializeField] private float _movementSpeed = 5f;
    public Action<bool> OnDistanceToHit;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _attackRadius = GetComponent<CircleCollider2D>();
        _thisWarrior = GetComponent<Warrior>();
    }

    private void Start()
    {
        _attackRadius.isTrigger = true;
        ReturnToDefendPosition();
        _thisWarrior.OnEnemyFind += MoveWarrior;
        _thisWarrior.OnEnemyDeath += ReturnToDefendPosition;
        _thisWarrior.OnDeathAction += OnDeath;

    }

    private void ChangeIntAnimation(int anim)
    {
        _animState = anim;
        _animator.SetInteger("AnimState", _animState);
    }

    private void OnDeath()
    {
        Destroy(this);
    }

    private void ReturnToDefendPosition()
    {
        if (_thisWarrior.EnemyWarrior != null)
        {
            MoveWarrior(_thisWarrior.EnemyWarrior.transform);
            return;
        }
        MoveWarrior(_protectPosition.transform);
    }

    private void MoveWarrior(Transform position)
    {
        _tween.Pause();
        _tween = null;
        _tween = transform.DOMove(position.position, _movementSpeed);
        StartCoroutine(ChangeRunAnimToIdle());
    }

    private IEnumerator ChangeRunAnimToIdle()
    {
        ChangeIntAnimation(2);
        yield return new WaitForSeconds(_movementSpeed);
        ChangeIntAnimation(0);
    }

    private void AttackTheEnemy(Warrior enemy, bool condition)
    {
        _tween.Pause();
        _tween = null;

        OnDistanceToHit?.Invoke(condition);
        StopAllCoroutines();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Warrior>(out Warrior component))
        {
            if (component == _thisWarrior.EnemyWarrior)
            {
                AttackTheEnemy(component, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Warrior>(out Warrior component))
        {
            if (component == _thisWarrior.EnemyWarrior)
            {
                MoveWarrior(_thisWarrior.transform);
                AttackTheEnemy(component, false);                
            }
        }
    }




}



