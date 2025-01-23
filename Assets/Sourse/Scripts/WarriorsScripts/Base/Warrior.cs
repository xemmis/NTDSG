using System;
using System.Collections;
using UnityEditor.Playables;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Warrior : MonoBehaviour, ISAlive
{
    public bool IsPlayerUnit;
    [field: SerializeField] public float Health { get; set; }
    [field: SerializeField] public float MaxHealth { get; set; }
    [field: SerializeField] public int Agility { get; set; }
    [field: SerializeField] public int Strength { get; set; }
    [field: SerializeField] public int Armor { get; set; }
    public Action OnDeathAction { get; set; }
    public Action OnHitAction { get; set; }

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
    public HireShower HireShower;
    public float MovementSpeed;
    public float AttackSpeed;
    public float AttackRange;
    public float EnemyCheckRadius = 5f;
    [Space(20)]
    [Header("������� �� ������� ��������")]
    [SerializeField] private float attackSpeedModifier = 0.5f; // ������� �������� ����� �� 1 ��������
    [SerializeField] private float movementSpeedModifier = 0.5f; // ������� �������� ������������ �� 1 ��������
    [Header("������� ��������")]
    [Space(20)]
    [SerializeField] private float _baseAttackSpeed = 1f; // ������� �������� �����
    [SerializeField] private float _baseMovementSpeed = 5f; // ������� �������� ������������

    private protected Animator _animator;
    private Rigidbody2D _rigidbody2D;



    private void Awake()
    {
        HireShower = GetComponent<HireShower>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Health = UnityEngine.Random.Range(_minHealthRand, _maxHealthRand);
        Agility = UnityEngine.Random.Range(_minDextRand, _maxDextRand);
        Strength = UnityEngine.Random.Range(_minStrRand, _maxStrRand);
        Armor = UnityEngine.Random.Range(_minArmRand, _maxArmRand);
        MaxHealth = Health;
        AttackSpeed = _baseAttackSpeed / (1 + Agility * attackSpeedModifier);
        MovementSpeed = _baseMovementSpeed + (Agility * movementSpeedModifier);
    }

    public virtual void CheckHealth()
    {
        if (Health > 0)
            return;
        _animator.SetTrigger("Death");
        OnDeathAction?.Invoke();
    }


    private protected float CalculateDamage(float damage)
    {
        // ��������� ������� �����, ������� ������� ����� �����
        float damageReduction = (Armor / _maxArmRand) * 0.45f; // ���������� ����� �� ����� (0% �� 45%)


        float damageTaken = damage * (1 - damageReduction);

        return damageTaken;
    }

    public virtual void TakeHit(float Damage)
    {
        OnHitAction?.Invoke();
        _animator.SetTrigger("Hurt");
        Health -= Damage;
        CheckHealth();
    }

    public void Heal(float healAmount)
    {
        throw new NotImplementedException();
    }

    public void Death()
    {
        throw new NotImplementedException();
    }
}

