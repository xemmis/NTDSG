using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]

public class BuildSize : MonoBehaviour
{
    [SerializeField] private MainBuildingLogic _building;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _buildingCollider;
    private BoxCollider2D _boxCollider;

    private void Awake()
    {
        _building = GetComponentInParent<MainBuildingLogic>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _buildingCollider = _building.GetComponent<BoxCollider2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _boxCollider.isTrigger = true;
        _boxCollider.size = _buildingCollider.size;
        _rigidbody.gravityScale = 0;
    }

    public void Placed()
    {
        _spriteRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BuildSize BuildSize))
        {
            _spriteRenderer.color = new Color(255, 0, 0, .30f);
        }
        if (collision.TryGetComponent(out BuildingZone BuildingZone))
        {
            _spriteRenderer.color = new Color(0, 255, 0, .30f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BuildSize BuildSize))
        {
            _spriteRenderer.color = new Color(0, 2550, 0, .30f);
        }
        if (collision.TryGetComponent(out BuildingZone BuildingZone))
        {
            _spriteRenderer.color = new Color(255, 0, 0, .30f);
        }
    }

}
