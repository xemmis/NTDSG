using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Warrior : MonoBehaviour
{
    [field: SerializeField] public float Health { get; private set; }    
    [field: SerializeField] public int Dexterity { get; private set; }    
    [field: SerializeField] public int Armor { get; private set; }


    [SerializeField] private int MaxArmor = 15;
    [SerializeField] private CircleCollider2D _circleCollider;
    [SerializeField] private Rigidbody2D _rigidbody2D;

    private void Awake()
    {        
        _circleCollider = GetComponent<CircleCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _circleCollider.isTrigger = true;
        _rigidbody2D.gravityScale = 0;
    }

    public virtual void TakePureHit(int Damage)
    {
        Health -= Damage;
    }

    public virtual void TakePhysicalHit(int Damage, float Dexterity)
    {
        if (IsMissed(Dexterity))
            return;
        Health -= CalculateDamage(Damage);

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
        float maxDexterity = 10f;

        // ��������� ���� ������� �� �������
        float missChance = maxMissChance - (maxMissChance - minMissChance) * (dexterity / maxDexterity);

        // ������������ �������� �� 5% �� 25%
        missChance = Mathf.Clamp(missChance, minMissChance, maxMissChance);

        // ���������� ��������� ����� �� 0 �� 1 � ���������, ��������� �� ������
        return Random.value <= missChance;
    }
}

