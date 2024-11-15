using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Warrior : MonoBehaviour
{
    [field: SerializeField] public float Health { get; private set; }
    [field: SerializeField] public int Dexterity { get; private set; }
    [field: SerializeField] public int Strength { get; private set; }
    [field: SerializeField] public int Armor { get; private set; }
    [field: SerializeField] public bool IsPlayerUnit { get; private set; }
    [field: SerializeField] public Warrior EnemyWarrior { get; private set; }

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private WarriorMovement _movement;
    [SerializeField] private float _enemyCheckRadius = 6f;
    [SerializeField] private int MaxArmor = 15;
    [SerializeField] private float _checkSurroudingsTick = .25f;

    public Action OnDeathAction;
    public Action<Transform> OnEnemyFind;
    public Action OnEnemyDeath;

    private void OnDeath()
    {
        if (Health <= 0)
            OnDeathAction?.Invoke();
        _animator.SetTrigger("Death");
        Destroy(this);
    }

    private void Awake()
    {
        _movement = GetComponent<WarriorMovement>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Dexterity = UnityEngine.Random.Range(0, 16);
        Strength = UnityEngine.Random.Range(5, 16);
        Armor = UnityEngine.Random.Range(5, 16);
        _rigidbody2D.gravityScale = 0;
        StartCoroutine(CheckSurroundingsTick());
    }

    public bool CheckEnemyLife()
    {
        if (EnemyWarrior.Health <= 0)
            EnemyWarrior = null;
        return (EnemyWarrior.Health > 0);
    }

    private void TakeHitAnimation()
    {
        _animator.SetTrigger("Hurt");
    }  

    private void CheckHealth()
    {
        if (Health > 0)
            return;
        OnDeath();
    }

    public virtual void TakePureHit(int Damage)
    {
        Health -= Damage;
        TakeHitAnimation();
        CheckHealth();
    }

    public virtual void TakePhysicalHit(int Damage)
    {
        if (IsMissed(Dexterity))
            return;
        Health -= CalculateDamage(Damage);
        CheckHealth();
        TakeHitAnimation();
    }

    private protected float CalculateDamage(float damage)
    {
        // Вычисляем процент урона, который пройдет через броню
        float damageReduction = (Armor / MaxArmor) * 0.45f; // Уменьшение урона от брони (0% до 45%)


        float damageTaken = damage * (1 - damageReduction);

        return damageTaken;
    }

    private protected bool IsMissed(float dexterity)
    {
        float maxMissChance = 0.25f; // 25%
        float minMissChance = 0.05f; // 5%
        float maxDexterity = 15f;

        // Вычисляем шанс промаха по формуле
        float missChance = maxMissChance - (maxMissChance - minMissChance) * (dexterity / maxDexterity);

        // Ограничиваем значение от 5% до 25%
        missChance = Mathf.Clamp(missChance, minMissChance, maxMissChance);

        // Генерируем случайное число от 0 до 1 и проверяем, произошёл ли промах
        return UnityEngine.Random.value <= missChance;
    }

    private void FocusedOnEnemy(Warrior enemy)
    {
        EnemyWarrior = enemy;
        OnEnemyFind?.Invoke(enemy.transform);
    }

    private void UnFocusedEnemy()
    {
        OnEnemyDeath?.Invoke();
        EnemyWarrior = null;
        ScanSurroundings();
    }

    private void ScanSurroundings()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _enemyCheckRadius);

        foreach (var hitCollider in hitColliders)
        {
            GameObject obj = hitCollider.gameObject;

            if (obj.TryGetComponent<Warrior>(out Warrior warriorComponent))
            {
                if (warriorComponent.Health > 0 && this != warriorComponent && warriorComponent.IsPlayerUnit != IsPlayerUnit)
                {
                    warriorComponent.OnDeathAction += UnFocusedEnemy;
                    FocusedOnEnemy(warriorComponent);
                    EnemyWarrior = warriorComponent;
                }
            }

        }
    }

    private IEnumerator CheckSurroundingsTick()
    {
        if (EnemyWarrior == null)
            ScanSurroundings();
        yield return new WaitForSeconds(_checkSurroudingsTick);
        StartCoroutine(CheckSurroundingsTick());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _enemyCheckRadius);
    }

}

