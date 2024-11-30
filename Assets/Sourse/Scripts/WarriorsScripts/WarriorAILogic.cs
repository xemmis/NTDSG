using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class WarriorAILogic : MonoBehaviour
{
    private CircleCollider2D _circleCollider;

    private Warrior _thisWarrior;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private AIState _state;

    [SerializeField] private bool _stunned = false;
    [SerializeField] private bool _attacking;
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _enemyCheckRadius = 5f;
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private int _animState;
    [SerializeField] private int _currentPatrolIndex = 0;
    [field: SerializeField] public Warrior EnemyWarrior { get; private set; }

    [SerializeField] private Transform[] _patrolPoints;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _thisWarrior = GetComponent<Warrior>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _attackSpeed = UnityEngine.Random.Range(3, 5);
        _thisWarrior.OnDeathAction += OnDeath;
        _thisWarrior.OnStunAction += StunLogic;
        _state = AIState.Patrol;
        StartCoroutine(CheckSurroundingsTick());
    }

    private void Update()
    {
        switch (_state)
        {
            case AIState.Idle:
                HandleIdle();
                break;

            case AIState.Patrol:
                _animator.SetInteger("AnimState", 2);
                HandlePatrol();
                break;

            case AIState.Chase:
                _animator.SetInteger("AnimState", 2);
                HandleChase();
                break;

            case AIState.Attack:
                _animator.SetInteger("AnimState", 1);
                HandleAttack();
                break;

            case AIState.Death:
                OnDeath();
                break;
            case AIState.Stunned:
                if (_stunned)
                    break;
                _stunned = true;
                StartCoroutine(StunDelay());
                break;
        }
    }
    private void Flip(Transform Destination)
    {
        if (transform.position.x > Destination.transform.position.x)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;
    }
    private void StunLogic()
    {
        _state = AIState.Stunned;
    }

    private IEnumerator StunDelay()
    {
        yield return new WaitForSeconds(_attackSpeed * 1.5f);
        _state = AIState.Attack;
        _stunned = false;

    }

    private void HandleIdle()
    {

    }

    private void OnDeath()
    {
        StopAllCoroutines();
        if (EnemyWarrior != null)
            EnemyWarrior.OnDeathAction -= EnemyDeath;

    }


    private void HandleAttack()
    {
        StartCoroutine(AttackTick());
    }

    private IEnumerator AttackTick()
    {
        if (_attacking == true)
        {
            yield break;

        }
        _attacking = true;
        yield return new WaitForSeconds(_attackSpeed);
        if (EnemyWarrior == null)
            yield break;

        _animator.SetTrigger("Attack");
        EnemyWarrior.TakePhysicalHit(_thisWarrior.Strength);

        _attacking = false;
    }

    private void HandlePatrol()
    {
        if (_patrolPoints.Length == 0) return;
        _state = AIState.Patrol;
        Transform targetPoint = _patrolPoints[_currentPatrolIndex];
        Flip(targetPoint);
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
        Flip(EnemyWarrior.transform);
        transform.position = Vector2.MoveTowards(transform.position, EnemyWarrior.transform.position, _movementSpeed * Time.deltaTime);
        if (AttackRangeCheck())
            _state = AIState.Attack;
    }


    private IEnumerator CheckSurroundingsTick()
    {
        if (EnemyWarrior == null)
        {
            if (ScanSurroundings(_enemyCheckRadius))
                _state = AIState.Chase;
        }

        yield return new WaitForSeconds(0.25f);
        StartCoroutine(CheckSurroundingsTick());
    }
    private bool AttackRangeCheck()
    {
        if (EnemyWarrior == null)
        {
            return false;
        }
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == EnemyWarrior.gameObject)
            {
                return true;
            }
        }
        _state = AIState.Chase;
        return false;

    }

    private void EnemyDeath()
    {
        EnemyWarrior.OnDeathAction -= EnemyDeath;
        EnemyWarrior = null;
        if (ScanSurroundings(_enemyCheckRadius))
            _state = AIState.Chase;
        else
            _state = AIState.Patrol;
    }

    private bool ScanSurroundings(float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;
            if (obj.TryGetComponent<Warrior>(out Warrior warriorComponent) && warriorComponent.Health > 0 && _thisWarrior != warriorComponent && warriorComponent.IsPlayerUnit != _thisWarrior.IsPlayerUnit)
            {
                EnemyWarrior = warriorComponent;
                EnemyWarrior.OnDeathAction += EnemyDeath;
                return true;
            }
        }
        return false;
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
    Death,
    Stunned,
}



