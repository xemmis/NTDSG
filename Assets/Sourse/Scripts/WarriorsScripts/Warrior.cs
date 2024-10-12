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
        // Вычисляем процент урона, который пройдет через броню
        float damageReduction = (Armor / MaxArmor) * 0.45f; // Уменьшение урона от брони (0% до 45%)


        float damageTaken = damage * (1 - damageReduction);

        return damageTaken; 
    }

    private protected bool IsMissed(float dexterity)
    {
        float maxMissChance = 0.25f; // 25%
        float minMissChance = 0.05f; // 5%
        float maxDexterity = 10f;

        // Вычисляем шанс промаха по формуле
        float missChance = maxMissChance - (maxMissChance - minMissChance) * (dexterity / maxDexterity);

        // Ограничиваем значение от 5% до 25%
        missChance = Mathf.Clamp(missChance, minMissChance, maxMissChance);

        // Генерируем случайное число от 0 до 1 и проверяем, произошёл ли промах
        return Random.value <= missChance;
    }
}

