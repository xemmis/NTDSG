using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BuildingZone : MonoBehaviour
{
    private BoxCollider2D _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;
    }
}