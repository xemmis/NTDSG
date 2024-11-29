using System.Collections;
using UnityEngine;

public class WarriorFight : MonoBehaviour
{
    private WarriorAILogic _thisWarriorAI;
    private Warrior _thisWarrior;
    private Warrior _enemyInMemory;
    private Animator _animator;

    private float _attackSpeed;

    [SerializeField] private bool _attacking = false;
    [SerializeField] private bool _canHit;
    [SerializeField] private bool _readyForHit = true;
    [SerializeField] private bool _receivedDamage;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _thisWarrior = GetComponent<Warrior>();
        _thisWarriorAI = GetComponent<WarriorAILogic>();
    }

    private void Start()
    {
        _attackSpeed = UnityEngine.Random.Range(0.5f, 1.5f);
        //_thisWarriorAI.OnDistanceToHit += StartAttacking;
    }

    private void StartAttacking(bool condition)
    {
        if (_enemyInMemory == null)
        {
            _enemyInMemory = _thisWarriorAI.EnemyWarrior;
            _enemyInMemory.OnDeathAction += ForgetEnemy;
            StartCoroutine(AttackTick(condition));
        }
        if (_enemyInMemory == _thisWarriorAI.EnemyWarrior)
        {
            StartCoroutine(AttackTick(condition));
        }
    }

    private void OnDestroy()
    {
        //_thisWarriorAI.OnDistanceToHit -= StartAttacking;
        _enemyInMemory.OnDeathAction -= ForgetEnemy;
    }

    private void ForgetEnemy()
    {
        StartCoroutine(AttackTick(false));
        _enemyInMemory.OnDeathAction -= ForgetEnemy;
        _enemyInMemory = null;
    }

    private IEnumerator AttackTick(bool condition)
    {
        if (!condition)
            yield break;
        yield return new WaitForSeconds(_attackSpeed);

        _animator.SetTrigger("Attack");
        _thisWarriorAI.EnemyWarrior.TakePhysicalHit(_thisWarrior.Strength);
    }
}



