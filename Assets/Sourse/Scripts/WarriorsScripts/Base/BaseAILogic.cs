using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BaseAILogic : MonoBehaviour
{
    [SerializeField] protected AIState _state;
    [SerializeField] protected bool _isChilled;
    [SerializeField] protected bool _attacking;

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
        transform.position = Vector2.MoveTowards(transform.position, EnemyWarrior.transform.position, _thisWarrior.MovementSpeed * Time.deltaTime);
    }

    public virtual void HandleAttack()
    {
        _attacking = true;
        Flip(EnemyWarrior.transform);
        _animator.SetTrigger("Attack");
        StartCoroutine(AttackTick());
    }

    protected void HitDamage()
    {
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
        _state = AIState.Idle;
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
