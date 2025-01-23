using TMPro;
using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
public class FireflyMovement : MonoBehaviour
{
    [Header("���� � ��������� ��������")]
    [SerializeField] private Transform _hero; // ������ �� ��������� �����
    [SerializeField] private SpriteRenderer _heroSprite; // ������ �� ��������� �����
    [SerializeField] private Rigidbody2D _rb;    
    [SerializeField] public float _moveDuration = 0.5f; // ������������ �����������
    [SerializeField] private SpriteRenderer _fireFlySprite;
    public Vector2 Offset = new Vector2(2f, 0f); // �������� ������������ �����
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _heroSprite = _hero.GetComponent<SpriteRenderer>();
        if (_hero == null)
        {
            Debug.LogError("����� �� �������� � FireflyFollow!");
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

        // ������������ �������� ������� � ������ ��������
        Vector2 targetPosition = new Vector2(_hero.position.x + Offset.x, _hero.position.y +.5f + Offset.y);

        // ��������� ������� ������� ��� �������� �����������
        Vector2 currentPosition = _rb.position;

        // ������� ����������� � �������������� �������� ������������
        Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime / _moveDuration);

        // ������������� ����� ������� ��� Rigidbody2D
        _rb.MovePosition(newPosition);

        // ��������� ����� ������� ����� FixedUpdate
        Invoke(nameof(FollowHero), Time.fixedDeltaTime);
    }
}
