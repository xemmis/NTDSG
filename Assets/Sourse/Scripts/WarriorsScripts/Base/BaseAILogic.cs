using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseAILogic : MonoBehaviour
{
    [SerializeField] protected AIState _state;
    [SerializeField] protected bool _isChilled;
    [SerializeField] protected bool _attacking;
    [SerializeField] protected Transform _destination;
    [SerializeField] private float _rayDistance = 2f;
    [SerializeField] private bool _isHitted;
    public float avoidDistance = 2f;
    public float _rayAngle = 80f;
    protected Warrior _thisWarrior;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    [field: SerializeField] protected Warrior EnemyWarrior { get; private set; }


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _thisWarrior = GetComponent<Warrior>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _thisWarrior.OnDeathAction += OnDeath;
    }

    private void Start()
    {
        _attacking = false;
    }

    private void Update()
    {
        CheckState();
    }

    public virtual void CheckState()
    {
        switch (_state)
        {
            case AIState.Idle:
                HandleIdle();
                break;

            case AIState.Chase:
                ChangeAnimState(2);
                HandleChase();
                AttackRangeCheck();
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
    protected void MoveTowardsTarget()
    {
        Vector3 direction = (_destination.position - transform.position).normalized;

        // Центральный луч
        RaycastHit2D centerRay = Physics2D.Raycast(transform.position, direction, _rayDistance);
        // Левый луч
        Vector3 leftDirection = RotateVector(direction, _rayAngle);
        RaycastHit2D leftRay = Physics2D.Raycast(transform.position, leftDirection, _rayDistance);
        // Правый луч
        Vector3 rightDirection = RotateVector(direction, -_rayAngle);
        RaycastHit2D rightRay = Physics2D.Raycast(transform.position, rightDirection, _rayDistance);

        if (centerRay.collider == null)
        {
            // Центральный путь свободен — движемся к цели
            transform.position = Vector3.MoveTowards(transform.position, _destination.position, _thisWarrior.MovementSpeed * Time.deltaTime);
        }
        else if (leftRay.collider == null)
        {
            // Левый путь свободен — движемся влево
            transform.position = Vector3.MoveTowards(transform.position,
                transform.position + leftDirection * avoidDistance,
                _thisWarrior.MovementSpeed * Time.deltaTime);
        }
        else if (rightRay.collider == null)
        {
            // Правый путь свободен — движемся вправо
            transform.position = Vector3.MoveTowards(transform.position,
                transform.position + rightDirection * avoidDistance,
                _thisWarrior.MovementSpeed * Time.deltaTime);
        }
        else
        {
            // Все пути заблокированы — делаем шаг назад
            Vector3 backwardDirection = -direction;
            transform.position = Vector3.MoveTowards(transform.position,
                transform.position + backwardDirection * avoidDistance,
                _thisWarrior.MovementSpeed * Time.deltaTime * 0.5f); // Замедляем шаг назад
        }
    }

    // Вспомогательная функция для поворота вектора на заданный угол
    Vector3 RotateVector(Vector3 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(
            vector.x * Mathf.Cos(rad) - vector.y * Mathf.Sin(rad),
            vector.x * Mathf.Sin(rad) + vector.y * Mathf.Cos(rad),
            0
        );
    }

    void OnDrawGizmos()
    {
        if (_destination == null) return;

        Vector3 direction = (_destination.position - transform.position).normalized;
        Vector3 leftDirection = RotateVector(direction, _rayAngle);
        Vector3 rightDirection = RotateVector(direction, -_rayAngle);

        // Центральный луч (красный)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + direction * _rayDistance);
        // Левый луч (зелёный)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + leftDirection * _rayDistance);
        // Правый луч (синий)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + rightDirection * _rayDistance);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Warrior>(out Warrior component))
            return;
        _isHitted = true;
    }

public virtual void HandleIdle()
    {
        ChangeAnimState(3);
    }

    public virtual void OnDeath()
    {
        EnemyWarrior = null;
        StopAllCoroutines();
        _state = AIState.Death;
    }


    public virtual void HandleChase()
    {
        Flip(EnemyWarrior.transform);
        StopAllCoroutines();
        _attacking = false;
        _destination = EnemyWarrior.transform;
        MoveTowardsTarget();
    }

    public virtual void HandleAttack()
    {
        if (EnemyWarrior == null)
        {
            _state = AIState.Idle;
            return;
        }
        _attacking = true;
        Flip(EnemyWarrior.transform);
        _animator.SetTrigger("Attack");
        StartCoroutine(AttackTick());
    }

    protected void HitDamage()
    {
        if (EnemyWarrior == null) return;
        EnemyWarrior.TakePhysicalHit(_thisWarrior.Strength);
    }

    public virtual void EnemyDeath()
    {
        EnemyWarrior = null;
        ScanSurroundings();
    }

    protected IEnumerator AttackTick()
    {
        yield return new WaitForSeconds(_thisWarrior.AttackSpeed);
        _attacking = false;
    }

    protected void ChangeAnimState(int index)
    {
        if (_animator.GetInteger("AnimState") == index)
            return;

        _animator.SetInteger("AnimState", index);
    }

    private protected void Flip(Transform Destination)
    {
        if (transform.position.x > Destination.transform.position.x)
            _spriteRenderer.flipX = true;
        else
            _spriteRenderer.flipX = false;
    }

    private protected void AttackRangeCheck()
    {
        if (EnemyWarrior == null)
        {
            _attacking = false;
            _animator.SetBool("Chasing", false);
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _thisWarrior.AttackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == EnemyWarrior.gameObject)
            {
                _state = AIState.Attack;
                _animator.SetBool("Chasing", false);
                return;
            }
        }
        _state = AIState.Chase;
        _animator.SetBool("Chasing", true);
    }

    private protected void ScanSurroundings()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _thisWarrior.EnemyCheckRadius);

        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;

            if (obj.TryGetComponent<Warrior>(out Warrior warriorComponent) && warriorComponent.Health > 0 && _thisWarrior != warriorComponent && warriorComponent.IsPlayerUnit != _thisWarrior.IsPlayerUnit)
            {
                EnemyWarrior = warriorComponent;
                EnemyWarrior.OnDeathAction += EnemyDeath;
                _state = AIState.Chase;
                _animator.SetBool("Chasing", true);
                return;
            }
        }
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
