using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class LightBolt : MonoBehaviour, ISpell
{
    public float SpellDuration { get; set; }
    public float SpellDamage { get; set; }
    public float SpellCost { get; set; }

    [SerializeField] public float _moveDuration = 0.5f; // ������������ �����������

    public Vector2 Offset = new Vector2(1f, 1f); // �������� ������������ �����
    [SerializeField] private Transform _hero; // ������ �� ��������� �����
    [SerializeField] private SpriteRenderer _heroSprite; // ������ �� ��������� �����
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

        // ������������ �������� ������� � ������ ��������
        Vector2 targetPosition = new Vector2(_hero.position.x + Offset.x, _hero.position.y + .5f + Offset.y);

        // ��������� ������� ������� ��� �������� �����������
        Vector2 currentPosition = _rb.position;

        // ������� ����������� � �������������� �������� ������������
        Vector2 newPosition = Vector2.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime / _moveDuration);

        // ������������� ����� ������� ��� Rigidbody2D
        _rb.MovePosition(newPosition);

        // ��������� ����� ������� ����� FixedUpdate
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

