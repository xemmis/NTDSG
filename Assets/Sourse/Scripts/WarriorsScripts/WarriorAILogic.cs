using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WarriorAILogic : MonoBehaviour
{
    private CircleCollider2D _circleCollider;

    private Warrior _thisWarrior;
    private WarriorFight _warFight;
    private Animator _animator;
    private AIState _state;
    private Rigidbody2D _rb;

    [SerializeField] private bool _attacking;
    [SerializeField] private bool _canHit;

    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _enemyCheckRadius = 5f;
    [SerializeField] private int _animState;
    [SerializeField] private int _currentPatrolIndex = 0;
    [field: SerializeField] public Warrior EnemyWarrior { get; private set; }

    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private Transform _protectPosition;

    //public Action<bool> OnDistanceToHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _thisWarrior = GetComponent<Warrior>();
        _warFight = GetComponent<WarriorFight>();
    }

    private void Start()
    {

        _circleCollider.isTrigger = true;
        _thisWarrior.OnDeathAction += OnDeath;
        _state = AIState.Patrol;
        StartCoroutine(CheckSurroundingsTick());
    }
    private void OnDestroy()
    {
        _thisWarrior.OnDeathAction -= OnDeath;
    }

    private void Update()
    {
        switch (_state)
        {
            case AIState.Idle:
                HandleIdle();
                break;

            case AIState.Patrol:
                HandlePatrol();
                break;

            case AIState.Chase:
                HandleChase();
                break;

            case AIState.Attack:
                HandleAttack();
                break;
        }
    }

    private void HandleIdle()
    {

    }

    private void OnDeath()
    {
        StopAllCoroutines();
        EnemyWarrior = null;
        _state = AIState.Idle;
        StartCoroutine(DeathTick());
    }

    private IEnumerator DeathTick()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void HandleAttack()
    {
        if (_attacking && !_canHit)
        {
            print("atackreturn");
            return;
        }

        _attacking = true;
        StartCoroutine(AttackTick());
    }

    private IEnumerator AttackTick()
    {

        yield return new WaitForSeconds(_attackSpeed);
        if (!_attacking && EnemyWarrior.Health <= 0)
        {
            yield break;
            print("atackbreak");
        }
        print("atackk");
        _animator.SetTrigger("Attack");
        EnemyWarrior.TakePhysicalHit(_thisWarrior.Strength);
        _attacking = false;
    }






    private void HandlePatrol()
    {
        if (_patrolPoints.Length == 0) return;
        _state = AIState.Patrol;
        Transform targetPoint = _patrolPoints[_currentPatrolIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, _movementSpeed * Time.deltaTime);

        // Проверяем, достигли ли точки патруля
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length; // Переход к следующей точке
        }

    }

    private void HandleChase()
    {
        _state = AIState.Chase;
        transform.position = Vector2.MoveTowards(transform.position, EnemyWarrior.transform.position, _movementSpeed * Time.deltaTime);

    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Warrior>(out Warrior warrior))
        {
            if (warrior == EnemyWarrior)
            {
                _canHit = true;
                _state = AIState.Attack;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Warrior>(out Warrior warrior))
        {
            if (warrior == EnemyWarrior && EnemyWarrior.Health > 0)
            {
                _attacking = false;
                HandleChase();
            }
        }
    }

    private void ForgetEnemy()
    {
        if (EnemyWarrior.Health <= 0)
        {
            EnemyWarrior = null;
            _attacking = false;
            _state = AIState.Patrol;
        }
    }

    private IEnumerator CheckSurroundingsTick()
    {
        if (EnemyWarrior == null)
            ScanSurroundings();
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CheckSurroundingsTick());
    }

    private void ScanSurroundings()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _enemyCheckRadius);

        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;

            if (obj.TryGetComponent<Warrior>(out Warrior warriorComponent))
            {
                if (warriorComponent.Health > 0 && _thisWarrior != warriorComponent && warriorComponent.IsPlayerUnit != _thisWarrior.IsPlayerUnit)
                {
                    print(warriorComponent);
                    EnemyWarrior = warriorComponent;
                    EnemyWarrior.OnDeathAction += ForgetEnemy;
                    HandleChase();
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _enemyCheckRadius);
    }
}


public enum AIState
{
    Idle,       // Состояние покоя
    Patrol,     // Патрулирование
    Chase,      // Преследование
    Attack,     // Атака
}



