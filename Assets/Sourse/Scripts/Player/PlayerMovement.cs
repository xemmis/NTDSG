using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 15f; // Максимальная скорость движения
    [SerializeField] private float _climbSpeed = 7f;
    [SerializeField] private float _acceleration = 5f; // Ускорение
    [SerializeField] private float _deceleration = 5f; // Замедление
    [SerializeField] private float _currentSpeed = 0f;
    [SerializeField] private Action _currentAction;

    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;

    public enum Action
    {
        Run,
        Stay,
        Climb
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _currentAction = Action.Stay;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        HandleAnimation();
    }

    private void HandleMovement()
    {
        float dirX = Input.GetAxis("Horizontal");

        _spriteRenderer.flipX = dirX < 0 ? true : dirX > 0 ? false : _spriteRenderer.flipX;

        if (dirX != 0)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, dirX * _maxSpeed, _acceleration * Time.fixedDeltaTime);
            _currentAction = Action.Run;
        }
        else
        {
            _currentAction = Action.Stay;
            _currentSpeed = Mathf.Lerp(_currentSpeed, 0f, _deceleration);
        }

        _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
    }

    private void HandleClimb()
    {        
        float dirY = Input.GetAxis("Vertical");

        if (dirY != 0)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, dirY * _climbSpeed, _acceleration * Time.fixedDeltaTime);
            _currentAction = Action.Climb;
        }
        else
        {
            _currentSpeed = 0;
        }
    }


    private void HandleAnimation()
    {
        if (_currentAction == Action.Run) _animator.SetInteger("AnimState", 1);
        if (_currentAction == Action.Stay) _animator.SetInteger("AnimState", 0);
        if (_currentAction == Action.Climb) _animator.SetTrigger("Climbing");
        
    }

    private void UseLadder()
    {
        _currentAction = Action.Climb;
    }

    private void EndLadder()
    {
        _currentAction = Action.Stay;
    }
}
