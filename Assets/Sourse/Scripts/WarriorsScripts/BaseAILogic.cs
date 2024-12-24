using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseAILogic : MonoBehaviour
{
    [SerializeField] private AIState _state;
    [SerializeField] private bool _isChilled;
    [SerializeField] private bool _attacking;

    private Warrior _thisWarrior;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform[] _patrolPoints;
    [field: SerializeField] public Warrior EnemyWarrior { get; private set; }


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _thisWarrior = GetComponent<Warrior>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _thisWarrior.OnDeathAction += OnDeath;
    }

    private void Start()
    {
        _attackSpeed = UnityEngine.Random.Range(3, 5);
        _state = AIState.Patrol;
    }

    private void Update()
    {
        switch (_state)
        {
            case AIState.Idle:
                if (_isChilled)
                    _state = AIState.Patrol;
                HandleIdle();
                break;

            case AIState.Patrol:
                if (!_isChilled)
                    _state = AIState.Idle;
                if (EnemyWarrior != null)
                    _state = AIState.Chase;
                HandlePatrol();
                ChangeAnimState(1);
                break;

            case AIState.Chase:
                ChangeAnimState(2);
                HandleChase();
                break;

            case AIState.Attack:
                AttackRangeCheck();
                if (!_attacking)
                    HandleAttack();
                break;

            case AIState.Death:
                OnDeath();
                break;
            default: break;
        }
    }

    private void OnDeath()
    {
        EnemyWarrior = null;
        StopAllCoroutines();
        _state = AIState.Death;
    }

    private void HandleIdle()
    {
        ChangeAnimState(3);
        if (!_isChilled)
            StartCoroutine(IdleWaiting());
    }

    private IEnumerator IdleWaiting()
    {
        yield return new WaitForSeconds(1.5f);
        if (EnemyWarrior != null)
            yield break;
        _isChilled = true;
    }

    private void HandleChase()
    {
        StopAllCoroutines();
        _attacking = false;
        transform.position = Vector2.MoveTowards(transform.position, EnemyWarrior.transform.position, _movementSpeed * Time.deltaTime);
    }

    private void HandleAttack()
    {
        Flip(EnemyWarrior.transform);
        _attacking = true;
        _animator.SetTrigger("Attack");
    }
    public void HitDamage()
    {
        EnemyWarrior.TakePhysicalHit(_thisWarrior.Strength);
        StartCoroutine(AttackTick());
    }
    private void EnemyDeath()
    {
        EnemyWarrior.OnDeathAction -= EnemyDeath;
        EnemyWarrior = null;
        ScanSurroundings();
    }
    private IEnumerator AttackTick()
    {
        yield return new WaitForSeconds(_attackSpeed);
        _attacking = false;
    }

    private void ChangeAnimState(int index)
    {
        if (_animator.GetInteger("AnimState") == index)
            return;

        _animator.SetInteger("AnimState", index);
    }

    public void GetPatrolPoints(Transform[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            _patrolPoints[i] = points[i];
        }
    }

    public void GetPatrolPoints(Transform points)
    {
        if (_patrolPoints.Length > 1)
        {
            Transform[] _newPatrolPoints = new Transform[1];
            _patrolPoints = _newPatrolPoints;
            _patrolPoints[1] = points;
        }
        else
            _patrolPoints[1] = points;
    }

    private void Flip(Transform Destination)
    {
        if (transform.position.x > Destination.transform.position.x)
            _spriteRenderer.flipX = true;
        else
            _spriteRenderer.flipX = false;
    }

    private void HandlePatrol()
    {
        if (_patrolPoints.Length == 0) return;
        Transform targetPoint = _patrolPoints[_currentPatrolIndex];
        Flip(targetPoint);
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, _movementSpeed * Time.deltaTime);

        // Проверяем, достигли ли точки патруля
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            _isChilled = false;
            _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length; // Переход к следующей точке
        }

    }

    private void AttackRangeCheck()
    {
        if (EnemyWarrior == null)
        {
            _state = AIState.Idle;
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == EnemyWarrior.gameObject)
            {
                _state = AIState.Attack;
                _animator.SetBool("Chasing", false);
                return;
            }
        }
        _animator.SetBool("Chasing", true);
        _state = AIState.Chase;
    }
    private void ScanSurroundings()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _enemyCheckRadius);

        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;
            if (obj.TryGetComponent<Warrior>(out Warrior warriorComponent) && warriorComponent.Health > 0 && _thisWarrior != warriorComponent && warriorComponent.IsPlayerUnit != _thisWarrior.IsPlayerUnit)
            {
                EnemyWarrior = warriorComponent;
                EnemyWarrior.OnDeathAction += EnemyDeath;
                _state = AIState.Chase;
                return;
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
    Death,
}
