using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class DestroyButton : MonoBehaviour
{
    [field: SerializeField] public bool CanDelete { get; private set; }
    [field: SerializeField] public bool Spawned { get; private set; }

    private MainBuildingLogic _buildingToDelete;
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    [SerializeField] private bool BuildingCondition;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidBody.gravityScale = 0;
        _boxCollider.isTrigger = true;
    }

    private void Update()
    {
        InputLogicToDelete();
    }

    private void InputLogicToDelete()
    {
        if (Input.GetMouseButtonDown(1))
            Destroy(gameObject);

        if (_buildingToDelete != null) 
        {
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(_buildingToDelete.gameObject);
                Destroy(gameObject);
            }        
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision.name);
        if (collision.TryGetComponent<MainBuildingLogic>(out MainBuildingLogic building))
        {
            if (!building.Build.GetCondition())
                Destroy(gameObject);
            CanDelete = true;
            _buildingToDelete = building;
        }
        else
            CanDelete = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        print(collision.name);
        if (collision.TryGetComponent<MainBuildingLogic>(out MainBuildingLogic building))
        {
            CanDelete = false;
            _buildingToDelete = null;
        }
    }
}
