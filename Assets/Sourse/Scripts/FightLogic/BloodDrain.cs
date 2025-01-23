using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BloodDrain : MonoBehaviour, ISpell
{

    public float SpellDuration { get; set; } = 5f;
    public float SpellDamage { get; set; } = 1.25f;
    public float SpellCost { get; set; } = 20f;

    [SerializeField] public float _moveDuration = 0.5f; // Длительность перемещения
    public Vector2 Offset = new Vector2(1f, 1f); // Смещение относительно героя

    [SerializeField] private Transform _hero; // Ссылка на трансформ героя
    [SerializeField] private SpriteRenderer _heroSprite; // Спрайт героя
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CircleCollider2D _circleCollider;
    [SerializeField] private bool _isActivated;
    [SerializeField] private float _pushForce;
    [SerializeField] private float _slowForce;
    private bool _isMoving;
    private float _timeElapsed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        _circleCollider.isTrigger = true;
        _rb.gravityScale = 0;
        Vector3 position = transform.position;
        position.z = -0.25f;
        transform.position = position;
    }
    public void TakeHeroComponent(Transform heroTransform)
    {
        _hero = heroTransform;
        _heroSprite = _hero.GetComponent<SpriteRenderer>();
        _isActivated = false;
        FollowHero();
    }

    private void FollowHero()
    {
        if (_isActivated) return;

        if (_heroSprite.flipX)
            Offset = new Vector2(-2f, 0);
        else
            Offset = new Vector2(2f, 0);

        // Рассчитываем конечную позицию с учетом смещения
        Vector2 targetPosition = new Vector2(_hero.position.x + Offset.x, _hero.position.y + Offset.y);

        // Вычисляем текущую позицию для плавного перемещения
        Vector2 currentPosition = _rb.position;

        // Плавное перемещение с использованием линейной интерполяции
        Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime / _moveDuration);

        // Устанавливаем новую позицию для Rigidbody2D
        _rb.MovePosition(newPosition);

        // Повторяем вызов функции через FixedUpdate
        Invoke(nameof(FollowHero), Time.fixedDeltaTime);
    }

    public void SpellActivate()
    {
        _isActivated = true;
        _isMoving = true;
        _timeElapsed = 0f;
        Vector2 direction = _heroSprite.flipX ? Vector2.left : Vector2.right;
        _rb.velocity = direction * 5f; // Задает направление и скорость движения

        InvokeRepeating(nameof(DealDamage), 0f, 0.25f);
        Invoke(nameof(Explode), SpellDuration);
    }

    private void DealDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius);

        foreach (var collider in hitColliders)
        {            
            if (collider.TryGetComponent<ISAlive>(out ISAlive targetLife) && collider.TryGetComponent<IsEnemy>(out IsEnemy enemyComponent))
            {
                targetLife.TakeHit(SpellDamage);

                // Притягиваем объект к заклинанию
                Rigidbody2D targetRb = collider.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    Vector2 direction = (transform.position - collider.transform.position).normalized;
                    targetRb.AddForce(direction * _pushForce * 10, ForceMode2D.Force); // Сила притяжения

                    // Добавляем временное сопротивление движению
                    targetRb.drag = _slowForce * 10; // Увеличиваем drag для замедления

                    // Сбрасываем drag через небольшое время
                    StartCoroutine(ResetDrag(targetRb, 0.25f));
                }
            }
        }

        // Увеличиваем размер объекта
        transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
    }

    private System.Collections.IEnumerator ResetDrag(Rigidbody2D targetRb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (targetRb != null)
        {
            targetRb.drag = 0f; // Сбрасываем drag обратно
        }
    }

    private void Explode()
    {
        CancelInvoke(nameof(DealDamage));

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _circleCollider.radius);

        foreach (var collider in hitColliders)
        {
            ISAlive target = collider.GetComponent<ISAlive>();
            if (target != null)
            {
                target.TakeHit(SpellDamage * 5.5f); // Наносит урон взрывом
            }
        }

        SpellDeactivate();
    }

    public void SpellDeactivate()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        if (_circleCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _circleCollider.radius);
        }
    }
}
