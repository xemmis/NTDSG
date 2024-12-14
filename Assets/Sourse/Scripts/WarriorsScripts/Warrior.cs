using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Warrior : MonoBehaviour
{
    [field: SerializeField] public float Health { get; private set; }
    public bool IsPlayerUnit;
    public float MaxHealth { get; private set; }
    public int Dexterity { get; private set; }
    public int Strength { get; private set; }
    public int Armor { get; private set; }
    [Header("Здоровье")]
    [Space(20)]
    [Header("Минимальное и максимальное значение здоровья")]
    [SerializeField] private int _minHealthRand;
    [SerializeField] private int _maxHealthRand;
    [Header("Минимальное и максимальное значение силы")]
    [SerializeField] private int _minStrRand;
    [SerializeField] private int _maxStrRand;
    [Header("Минимальное и максимальное значение брони")]
    [SerializeField] private int _minArmRand;
    [SerializeField] private int _maxArmRand;
    [Header("Минимальное и максимальное значение Ловкости")]
    [SerializeField] private int _minDextRand;
    [SerializeField] private int _maxDextRand;




    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private float _stunPercents;
    private float _stunResistance;
    private int MaxArmor = 15;

    public Action OnDeathAction;
    public Action OnStunAction;
    public Action OnHitAction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Health = UnityEngine.Random.Range(_minHealthRand, _maxHealthRand);
        MaxHealth = Health;
        Dexterity = UnityEngine.Random.Range(_minDextRand, _maxDextRand);
        Strength = UnityEngine.Random.Range(_minStrRand, _maxStrRand);
        Armor = UnityEngine.Random.Range(_minArmRand, _maxArmRand);
        _rigidbody2D.gravityScale = 0;
        _stunResistance = Strength * 1.5f;
    }

    private void OnDeath()
    {
        if (Health <= 0)
            OnDeathAction?.Invoke();

    }

    private void TakeHitAnimation()
    {
        if (_stunPercents >= _stunResistance)
        {
            OnStunAction?.Invoke();
            OnHitAction?.Invoke();
            _stunPercents = 0;
            _animator.SetTrigger("Stunned");
        }
        else
        {
            OnHitAction?.Invoke();
            _animator.SetTrigger("Hurt");
        }
    }

    private void CheckHealth()
    {
        if (Health > 0)
            return;
        OnDeath();
        _animator.SetTrigger("Death");
    }

    public virtual void TakePureHit(int Damage)
    {
        Health -= Damage;
        TakeHitAnimation();
    }

    public virtual void TakePhysicalHit(int Damage)
    {
        if (IsMissed(Dexterity))
            return;
        _stunPercents += Damage * 1.5f;
        Health -= CalculateDamage(Damage);
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


}

