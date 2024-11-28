using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WarriorAILogic : MonoBehaviour
{
    private CircleCollider2D _circleCollider;

    private Warrior _thisWarrior;
    private Animator _animator;
    private AIState _state;
    private Rigidbody2D _rb;

    [SerializeField] private bool _attacking = false;
    [SerializeField] private bool _canHit;
    [SerializeField] private bool _readyForHit;
    [SerializeField] private bool _receivedDamage;

    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _enemyCheckRadius = 5f;
    [SerializeField] private int _animState;
    [SerializeField] private int _currentPatrolIndex = 0;


    [SerializeField] private Warrior _enemyWarrior;
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private Transform _protectPosition;
    public Action<bool> OnDistanceToHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _thisWarrior = GetComponent<Warrior>();
    }

    private void Start()
    {
        _attackSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
        _circleCollider.isTrigger = true;
        _thisWarrior.OnDeathAction += OnDeath;
        _thisWarrior.OnHitAction += ReceivedDamageLogic;
        _state = AIState.Patrol;
        StartCoroutine(CheckSurroundingsTick());
    }


    private void Update()
    {
        print(_state);
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
    private void ChangeToFalseAttackCondition()
    {
        _attacking = false;
    }

    private void ReceivedDamageLogic(bool condition)
    {
        _receivedDamage = condition;
    }

    private void ChangeTrueAttackCondition()
    {
        _attacking = true;
    }
    private void HandleIdle()
    {

    }

    private void OnDeath()
    {
        StopAllCoroutines();
        _state = AIState.Idle;
        StartCoroutine(DeathTick());
    }

    private IEnumerator DeathTick()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
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
        transform.position = Vector2.MoveTowards(transform.position, _enemyWarrior.transform.position, _movementSpeed * Time.deltaTime);

    }

    private void HandleAttack()
    {
        if (_attacking || !_canHit || !_readyForHit)
            return;
        StartCoroutine(AttackTick());
    }

    private IEnumerator AttackTick()
    {
        _readyForHit = false;
        yield return new WaitForSeconds(_attackSpeed);
        if (_receivedDamage && !_canHit)
        {
            _receivedDamage = false;
            _attacking = false;
            _readyForHit = true;
            Debug.Log("залочили сууки");
            yield break;
        }
        _animator.SetTrigger("Attack");
        _enemyWarrior.TakePhysicalHit(_thisWarrior.Strength);
        _readyForHit = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Warrior>(out Warrior warrior))
        {
            if (warrior == _enemyWarrior)
            {
                _canHit = true;
                _readyForHit = true;
                _state = AIState.Attack;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Warrior>(out Warrior warrior))
        {
            if (warrior == _enemyWarrior && _enemyWarrior.Health > 0)
            {
                _canHit = false;
                _readyForHit = false;
                HandleChase();
            }
        }
    }

    private IEnumerator CheckSurroundingsTick()
    {
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
                    _enemyWarrior = warriorComponent;
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



