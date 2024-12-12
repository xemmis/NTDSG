using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Warrior : MonoBehaviour
{
    [field: SerializeField] public float Health { get; private set; }
    [field: SerializeField] public float MaxHealth { get; private set; }
    [field: SerializeField] public int Dexterity { get; private set; }
    [field: SerializeField] public int Strength { get; private set; }
    [field: SerializeField] public int Armor { get; private set; }
    [field: SerializeField] public bool IsPlayerUnit { get; private set; }

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private float _stunPercents;
    [SerializeField] private int MaxArmor = 15;
    [SerializeField] private float _checkSurroudingsTick = .25f;

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
        Health = UnityEngine.Random.Range(35, 55);
        MaxHealth = Health;
        Dexterity = UnityEngine.Random.Range(0, 16);
        Strength = UnityEngine.Random.Range(5, 16);
        Armor = UnityEngine.Random.Range(5, 16);
        _rigidbody2D.gravityScale = 0;
    }

    private void OnDeath()
    {
        if (Health <= 0)
            OnDeathAction?.Invoke();

    }

    private void TakeHitAnimation()
    {
        if (_stunPercents >= 100)
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

