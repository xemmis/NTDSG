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

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;

    public Action OnDeathAction;
    public Action OnHitAction;

    private void Awake()
    {
        HireShower= GetComponent<HireShower>();  
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
        AttackSpeed = _baseAttackSpeed / (1 + Dexterity * attackSpeedModifier);
        MovementSpeed = _baseMovementSpeed + (Dexterity * movementSpeedModifier);
    }

    public virtual void TakeHitAnimation()
    {
        OnHitAction?.Invoke();
        _animator.SetTrigger("Hurt");
    }

    protected void CheckHealth()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, EnemyCheckRadius);
    }
}

