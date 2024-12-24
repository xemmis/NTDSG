using System;
using System.Collections;
using UnityEditor.Playables;
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

    [Header("Остальные аттрибуты")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _enemyCheckRadius;
    [Space(20)]
    [Header("Прирост за еденицу ловкости")]
    [SerializeField] private float attackSpeedModifier = 0.05f; // Прирост скорости атаки за 1 ловкость
    [SerializeField] private float movementSpeedModifier = 0.1f; // Прирост скорости передвижения за 1 ловкость
    [Header("Базовая скорость")]
    [Space(20)]
    [SerializeField] private float _baseAttackSpeed = 1f; // Базовая скорость атаки
    [SerializeField] private float _baseMovementSpeed = 5f; // Базовая скорость передвижения

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;

    public Action OnDeathAction;
    public Action OnHitAction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Health = UnityEngine.Random.Range(_minHealthRand, _maxHealthRand);
        Dexterity = UnityEngine.Random.Range(_minDextRand, _maxDextRand);
        Strength = UnityEngine.Random.Range(_minStrRand, _maxStrRand);
        Armor = UnityEngine.Random.Range(_minArmRand, _maxArmRand);
        MaxHealth = Health;
        _rigidbody2D.gravityScale = 0;
        _attackSpeed = _baseMovementSpeed + (Dexterity * movementSpeedModifier);
        _movementSpeed = _baseAttackSpeed + (Dexterity * attackSpeedModifier);        
    }
    private void TakeHitAnimation()
    {

        OnHitAction?.Invoke();
        _animator.SetTrigger("Hurt");

    }

    private void CheckHealth()
    {
        if (Health > 0)
            return;
        OnDeathAction?.Invoke();
        _animator.SetTrigger("Die");
    }

    public virtual void TakePureHit(int Damage)
    {
        Health -= Damage;
        TakeHitAnimation();
    }

    public virtual void TakePhysicalHit(int Damage)
    {
        Health -= CalculateDamage(Damage);
        TakeHitAnimation();
        if (Health <= 0)
        {
            OnDeathAction?.Invoke();
        }
    }

    private protected float CalculateDamage(float damage)
    {
        // Вычисляем процент урона, который пройдет через броню
        float damageReduction = (Armor / _maxArmRand) * 0.45f; // Уменьшение урона от брони (0% до 45%)


        float damageTaken = damage * (1 - damageReduction);

        return damageTaken;
    }  
}

