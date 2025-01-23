using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class LightBolt : MonoBehaviour, ISpell
{
    public float SpellDuration { get; set; }
    public float SpellDamage { get; set; }
    public float SpellCost { get; set; }

    [SerializeField] public float _moveDuration = 0.5f; // Длительность перемещения

    public Vector2 Offset = new Vector2(1f, 1f); // Смещение относительно героя
    [SerializeField] private Transform _hero; // Ссылка на трансформ героя
    [SerializeField] private SpriteRenderer _heroSprite; // Ссылка на трансформ героя
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private CircleCollider2D _circleCollider;

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
        FollowHero();
    }

    private void FollowHero()
    {
        if (_heroSprite.flipX)
            Offset = new Vector2(-2f, 1);
        else
            Offset = new Vector2(2f, 1);

        // Рассчитываем конечную позицию с учетом смещения
        Vector2 targetPosition = new Vector2(_hero.position.x + Offset.x, _hero.position.y + .5f + Offset.y);

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

    }
    public void SpellDeactivate()
    {
        Destroy(this.gameObject);
    }
}

