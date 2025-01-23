using TMPro;
using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
public class FireflyMovement : MonoBehaviour
{
    [Header("Цель и настройки движения")]
    [SerializeField] private Transform _hero; // Ссылка на трансформ героя
    [SerializeField] private SpriteRenderer _heroSprite; // Ссылка на трансформ героя
    [SerializeField] private Rigidbody2D _rb;    
    [SerializeField] public float _moveDuration = 0.5f; // Длительность перемещения
    [SerializeField] private SpriteRenderer _fireFlySprite;
    public Vector2 Offset = new Vector2(2f, 0f); // Смещение относительно героя
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _heroSprite = _hero.GetComponent<SpriteRenderer>();
        if (_hero == null)
        {
            Debug.LogError("Герой не назначен в FireflyFollow!");
            return;
        }

        FollowHero();
    }

    private void FollowHero()
    {
        if (_heroSprite.flipX)
        {
            Offset = new Vector2(-2f, 0f);
            _fireFlySprite.flipX = true;
        }
        else
        {
            Offset = new Vector2(2f, 0f);
            _fireFlySprite.flipX = false;
        }

        // Рассчитываем конечную позицию с учетом смещения
        Vector2 targetPosition = new Vector2(_hero.position.x + Offset.x, _hero.position.y +.5f + Offset.y);

        // Вычисляем текущую позицию для плавного перемещения
        Vector2 currentPosition = _rb.position;

        // Плавное перемещение с использованием линейной интерполяции
        Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime / _moveDuration);

        // Устанавливаем новую позицию для Rigidbody2D
        _rb.MovePosition(newPosition);

        // Повторяем вызов функции через FixedUpdate
        Invoke(nameof(FollowHero), Time.fixedDeltaTime);
    }
}
