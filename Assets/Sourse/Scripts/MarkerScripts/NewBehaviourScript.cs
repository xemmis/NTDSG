using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class NewBehaviourScript : MonoBehaviour
{
    public Building Building;
    CircleCollider2D circleCollider;
    Rigidbody2D body;

    private void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        circleCollider.isTrigger = true;
        body.gravityScale = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {       
        if (collision.TryGetComponent<Building>(out Building building)) 
        {
            print(collision.tag);
        }
    }
}
