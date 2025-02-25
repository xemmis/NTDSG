using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BaseMeleeLogic : MonoBehaviour, ISAlive
{
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private bool _isAttack;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private protected CharacterData _stats;
    public EnemyAction Action;
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
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
        _stats.Initialize();
        _stats.OnDeathEvent += OnDeath;
    }

    private void Update()
    {
        StateHandler();
    }

    public enum EnemyAction
    {
        Idle,
        Chase,
        Block,
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

            case EnemyAction.Block:
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
        // Вычисляем направление к игроку
        Vector2 direction = (_playerStats.gameObject.transform.position - transform.position).normalized;
        // Если игрок вне зоны атаки, двигаемся к нему
        if (!AttackRangeCheck())
        {
            _rigidBody.velocity = direction * _stats.Speed;
        }
        else
        {
            // Если игрок в зоне атаки, переключаемся на атаку
            Action = EnemyAction.Attack;
        }

        // Отражаем спрайт в зависимости от направления
        _spriteRenderer.flipX = direction.x < 0 ? true : direction.x > 0 ? false : _spriteRenderer.flipX;
        // Управляем анимациями
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

        // Запускаем анимацию атаки
        _animator.SetTrigger("Attack");
    }

    private IEnumerator AttackTick()
    {
        _isAttack = true;

        // Ждём перед атакой
        yield return new WaitForSeconds(_attackSpeed);

        // Если игрок ушёл из зоны атаки, возвращаемся к преследованию


        // Сбрасываем флаг атаки
        _isAttack = false;
    }

    private bool AttackRangeCheck()
    {
        // Вычисляем расстояние до игрока
        float distanceToPlayer = Vector2.Distance(_playerStats.gameObject.transform.position, transform.position);

        // Если игрок в зоне атаки
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
        // Наносим урон игроку
        _playerStats.TakeHit(_stats.Damage);
        Debug.Log($"Enemy dealt {_stats.Damage} damage to player.");
    }


    public virtual bool BlockHandler()
    {
        float rand = Random.Range(0, 100);
        float blockRand = 0;
        for (int i = 0; i < _stats.PercentItems.Count; i++)
        {
            if (_stats.PercentItems[i].Name == "Block")
            {
                blockRand = _stats.PercentItems[i].Value;
            }
        }
        if (rand > blockRand) return false;
        _animator.SetTrigger("Block");
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
    }

    public void TakeHit(float damage)
    {
        if (_stats.CurrentHealth <= 0) return;
        if (BlockHandler()) return;
        _stats.TakeDamage(damage);
        _animator.SetTrigger("Damaged");
    }
}

