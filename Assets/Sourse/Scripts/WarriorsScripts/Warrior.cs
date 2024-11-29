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

    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private int MaxArmor = 15;
    [SerializeField] private float _checkSurroudingsTick = .25f;

    public Action OnDeathAction;
    public Action <bool> OnHitAction;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Dexterity = UnityEngine.Random.Range(0, 16);
        Strength = UnityEngine.Random.Range(5, 16);
        Armor = UnityEngine.Random.Range(5, 16);
        _rigidbody2D.gravityScale = 0;
    }    
    private void OnDeath()
    {
        if (Health <= 0)
            OnDeathAction?.Invoke();
        _animator.SetTrigger("Death");
    }
  
    private void TakeHitAnimation()
    {
        if (Health < 37)
            OnHitAction?.Invoke(true);
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
        // ��������� ������� �����, ������� ������� ����� �����
        float damageReduction = (Armor / MaxArmor) * 0.45f; // ���������� ����� �� ����� (0% �� 45%)


        float damageTaken = damage * (1 - damageReduction);

        return damageTaken;
    }

    private protected bool IsMissed(float dexterity)
    {
        float maxMissChance = 0.25f; // 25%
        float minMissChance = 0.05f; // 5%
        float maxDexterity = 15f;

        // ��������� ���� ������� �� �������
        float missChance = maxMissChance - (maxMissChance - minMissChance) * (dexterity / maxDexterity);

        // ������������ �������� �� 5% �� 25%
        missChance = Mathf.Clamp(missChance, minMissChance, maxMissChance);

        // ���������� ��������� ����� �� 0 �� 1 � ���������, ��������� �� ������
        return UnityEngine.Random.value <= missChance;
    }

 
}

