using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BaseMeleeLogic : MonoBehaviour, ISAlive
{
    [SerializeField] private protected PlayerStats _playerStats;
    [SerializeField] private CharacterData _template;
    [SerializeField] private  protected CharacterData _stats;
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private bool _isAttack;
    public EnemyAction Action;
    private protected Rigidbody2D _rigidBody;
    private protected BoxCollider2D _boxCollider;
    private protected SpriteRenderer _spriteRenderer;
    private protected Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _stats = _template.Clone();
        _stats.Initialize();
        _stats.OnDeathEvent += OnDeath;
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer, true);
    }

    private void Update()
    {
        StateHandler();
    }

    public enum EnemyAction
    {
        Idle,
        Chase,
        Attack,
        RunAway,
        Death
    }

    public virtual void StateHandler()
    {
        switch (Action)
        {
            case EnemyAction.Idle:
                if (_stats.CurrentHealth <= 0) Action = EnemyAction.Death;
                IdleHandler();
                break;

            case EnemyAction.Chase:
                if (_stats.CurrentHealth <= 0) Action = EnemyAction.Death;
                ChaseHandler();
                break;

            case EnemyAction.Attack:
                if (_stats.CurrentHealth <= 0) Action = EnemyAction.Death;
                if (!AttackRangeCheck())
                {
                    Action = EnemyAction.Chase;
                    _isAttack = false;
                    return;
                }

                if (!_isAttack) AttackHandler();
                break;

            case EnemyAction.RunAway:
                break;

            case EnemyAction.Death:
                _animator.SetBool("Death", true);
                break;

            default:
                break;
        }
    }

    public virtual void IdleHandler()
    {
        _animator.SetBool("Idle", true);
        _animator.SetBool("Chase", false);
        CheckArea();
    }

    private protected void CheckArea()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _detectionRange);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i] == null) continue;
            if (colliders[i].TryGetComponent<PlayerStats>(out PlayerStats stats))
            {
                _playerStats = stats;
                Action = EnemyAction.Chase;
            }
        }
    }

    public virtual void ChaseHandler()
    {
        Vector2 direction = (_playerStats.gameObject.transform.position - transform.position).normalized;

        if (!AttackRangeCheck()) _rigidBody.velocity = direction * _stats.Speed;
        else Action = EnemyAction.Attack;

        _spriteRenderer.flipX = direction.x < 0 ? true : direction.x > 0 ? false : _spriteRenderer.flipX;
        _animator.SetBool("Idle", false);
        _animator.SetBool("Chase", true);
    }

    public virtual void AttackHandler()
    {
        StartCoroutine(AttackTick());
        if (!AttackRangeCheck())
        {
            Action = EnemyAction.Chase;
            _isAttack = false;
            return;
        }

        _animator.SetTrigger("Attack");
    }

    private protected IEnumerator AttackTick()
    {
        _isAttack = true;
        yield return new WaitForSeconds(_attackSpeed);
        _isAttack = false;
    }

    private bool AttackRangeCheck()
    {
        float distanceToPlayer = Vector2.Distance(_playerStats.gameObject.transform.position, transform.position);

        if (distanceToPlayer <= _attackRange)
        {
            Action = EnemyAction.Attack;
            _animator.SetBool("Idle", true);
            _animator.SetBool("Chase", false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DealDamage()
    {
        _playerStats.TakeHit(_stats.Damage);
        Debug.Log($"Enemy dealt {_stats.Damage} damage to player.");
    }

    public virtual bool SpecialHandler()
    {
        float rand = Random.Range(0, 100);
        float blockRand = 0;
        for (int i = 0; i < _stats.PercentItems.Count; i++)
        {
            if (_stats.PercentItems[i].Name == "Special")
            {
                blockRand = _stats.PercentItems[i].Value;
            }
        }
        if (rand > blockRand) return false;
        _animator.SetTrigger("Special");
        return true;

    }

    public virtual void RunAwayHandler() { }

    public virtual void OnDeath()
    {
        Action = EnemyAction.Death;
        _boxCollider.enabled = false;
        _rigidBody.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, _detectionRange);
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    public virtual void TakeHit(float damage)
    {
        if (_stats.CurrentHealth <= 0) return;
        if (SpecialHandler()) return;
        _stats.TakeDamage(damage);
        _animator.SetTrigger("Damaged");
    }
}

