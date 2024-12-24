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
    [Header("��������")]
    [Space(20)]
    [Header("����������� � ������������ �������� ��������")]
    [SerializeField] private int _minHealthRand;
    [SerializeField] private int _maxHealthRand;
    [Header("����������� � ������������ �������� ����")]
    [SerializeField] private int _minStrRand;
    [SerializeField] private int _maxStrRand;
    [Header("����������� � ������������ �������� �����")]
    [SerializeField] private int _minArmRand;
    [SerializeField] private int _maxArmRand;
    [Header("����������� � ������������ �������� ��������")]
    [SerializeField] private int _minDextRand;
    [SerializeField] private int _maxDextRand;

    [Header("��������� ���������")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _enemyCheckRadius;
    [Space(20)]
    [Header("������� �� ������� ��������")]
    [SerializeField] private float attackSpeedModifier = 0.05f; // ������� �������� ����� �� 1 ��������
    [SerializeField] private float movementSpeedModifier = 0.1f; // ������� �������� ������������ �� 1 ��������
    [Header("������� ��������")]
    [Space(20)]
    [SerializeField] private float _baseAttackSpeed = 1f; // ������� �������� �����
    [SerializeField] private float _baseMovementSpeed = 5f; // ������� �������� ������������

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
        // ��������� ������� �����, ������� ������� ����� �����
        float damageReduction = (Armor / _maxArmRand) * 0.45f; // ���������� ����� �� ����� (0% �� 45%)


        float damageTaken = damage * (1 - damageReduction);

        return damageTaken;
    }  
}

