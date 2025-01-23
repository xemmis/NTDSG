using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent (typeof(BoxCollider2D))]
public class BaseAILogic : MonoBehaviour
{
    [SerializeField] protected AIState _state;
    [SerializeField] protected Transform _destination;

    [SerializeField] protected bool _isChilled;
    [SerializeField] protected bool _attacking;

    protected CapsuleCollider2D _capsuleCollider;
    protected BoxCollider2D _boxCollider;
    protected Warrior _thisWarrior;
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected Rigidbody2D _rigidbody;
    [field: SerializeField] protected GameObject Enemy { get; private set; }
    [field: SerializeField] protected ISAlive EnemyLife { get; private set; }


    private void Awake()
    {
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _thisWarrior = GetComponent<Warrior>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _thisWarrior.OnDeathAction += OnDeath;
    }

    private void Start()
    {
        _boxCollider.isTrigger = true;
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
                _animator.SetBool("Chasing", true);
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

    public virtual void HandleIdle()
    {
        ScanSurroundings();
        _animator.SetInteger("AnimState", 1);
    }

    public virtual void OnDeath()
    {
        _animator.SetBool("Chasing", false);
        _animator.SetInteger("AnimState", 0);
        Enemy = null;
        EnemyLife = null;
        _boxCollider.enabled = false;
        StopAllCoroutines();
        _state = AIState.Death;
        _animator.SetTrigger("Death");
    }


    public virtual void HandleChase()
    {
        if (Enemy == null)
            return;

        Flip(Enemy.transform);
        StopAllCoroutines();
        _attacking = false;
        _destination = Enemy.transform;

        Vector3 direction = (_destination.position - transform.position).normalized;


        _rigidbody.velocity = direction * _thisWarrior.MovementSpeed;

    }

    public virtual void HandleAttack()
    {
        if (Enemy == null)
        {
            _state = AIState.Idle;
            return;
        }
        _animator.SetInteger("AnimState", 0);
        _attacking = true;
        Flip(Enemy.transform);
        _animator.SetTrigger("Attack");
        StartCoroutine(AttackTick());
    }

    protected void HitDamage()
    {
        if (Enemy == null) return;
        EnemyLife.TakeHit(_thisWarrior.Strength);
    }

    public virtual void EnemyDeath()
    {
        Enemy = null;
        EnemyLife = null;
        ScanSurroundings();
    }

    protected IEnumerator AttackTick()
    {
        yield return new WaitForSeconds(_thisWarrior.AttackSpeed);
        _attacking = false;
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
        if (Enemy == null)
        {
            _attacking = false;
            _animator.SetBool("Chasing", false);
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _thisWarrior.AttackRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == Enemy)
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
        float rayDistance = _thisWarrior.EnemyCheckRadius; // Дальность луча
        Vector2 direction = _spriteRenderer.flipX ? Vector2.left : Vector2.right; // Направление луча в зависимости от flipX

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayDistance);

        if (hit.collider != null)
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.TryGetComponent<ISAlive>(out ISAlive aliveComponent) && aliveComponent.Health > 0 && this.gameObject != obj)
            {
                aliveComponent.OnDeathAction += OnDeath;
                Enemy = obj;
                EnemyLife = aliveComponent;
                _state = AIState.Chase;
                _animator.SetBool("Chasing", true);

                Debug.DrawRay(transform.position, direction * rayDistance, Color.red, 0.5f); // Визуализация успешного попадания
                return;
            }
        }

        Debug.DrawRay(transform.position, direction * rayDistance, Color.cyan, 0.5f); // Визуализация пустого луча
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Enemy != null || _thisWarrior.Health <= 0) return;

        if (collision.TryGetComponent<ISAlive>(out ISAlive aliveComponent) && aliveComponent.Health > 0 && this.gameObject != collision.gameObject)
        {
            aliveComponent.OnDeathAction += OnDeath;
            Enemy = collision.gameObject;
            EnemyLife = aliveComponent;
            _state = AIState.Chase;
            _animator.SetBool("Chasing", true);
            return;
        }
    }
}


public enum AIState
{
    Idle,       // Состояние покоя
    Patrol,     // Патрулирование
    Chase,      // Преследование
    Attack,     // Атака
    Block,
    Death,
}
