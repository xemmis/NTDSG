using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BuildingZone : MonoBehaviour
{
    private BoxCollider2D _collider;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _rigidbody.gravityScale = 0;
        _collider.isTrigger = true;
    }
}