using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bomb : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _speed;
    [SerializeField] private float _timer;
    [SerializeField] private float _bombRadius;
    private Rigidbody2D _body;

    private void Awake() 
    { 
        _body = GetComponent<Rigidbody2D>();
    }    

    public void BombFly(Vector2 direction)
    {
        _body.velocity = direction * _speed;
    }

    public void BombBlow()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _bombRadius);
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++) 
            {
                if (colliders[i].TryGetComponent<PlayerStats>(out PlayerStats stats))
                {
                    stats.TakeHit(_damage);
                }
            }
        }
    }

    public void BombDestroy()
    {
        Destroy(gameObject);
    }

}
