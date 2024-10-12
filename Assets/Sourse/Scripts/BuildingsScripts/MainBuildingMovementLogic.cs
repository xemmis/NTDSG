using UnityEngine;

public class MainBuildingMovementLogic : MonoBehaviour
{
    [SerializeField] private Building _building;

    private void Start()
    {
        _building = GetComponentInChildren<Building>();
    }

    private void Update()
    {
        MoveFlyingBuilding();
    }

    private void MoveFlyingBuilding()
    {
        if (_building.GetCondition())
            Destroy(this);

        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(MousePosition.x);
        int y = Mathf.RoundToInt(MousePosition.y);

        transform.position = new Vector3(x, y, 0);
    }
}