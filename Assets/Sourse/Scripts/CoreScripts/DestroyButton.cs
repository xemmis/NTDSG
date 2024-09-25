using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class DestroyButton : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    [field: SerializeField] public bool CanDelete { get; private set; }
    [SerializeField] private bool BuildingCondition;
    [SerializeField] private Building _building;
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidBody.gravityScale = 0;
        _boxCollider.isTrigger = true;
    }

    private void Update()
    {
        InputLogic(_building);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Building>(out Building component))
        {
            BuildingCondition = component.GetCondition();
            if (!BuildingCondition)
                return;
            CanDelete = true;
            _building = component;
        }
        else
        {
            CanDelete = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Building>(out Building component))
        {
            CanDelete = false;
            _building = null;
        }
    }

    private void InputLogic(Building building)
    {
        if (Input.GetMouseButton(1))
        {
            Destroy(gameObject);
        }

        if (!CanDelete)
            return;

        if (CanDelete && Input.GetMouseButton(0))
        {
            building.StopAllCoroutines();
            Destroy(building.gameObject);
        }
    }

}
