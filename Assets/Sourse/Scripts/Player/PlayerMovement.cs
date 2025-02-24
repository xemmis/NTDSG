using System.Collections;
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
        if (_currentAction != Action.Climb) HandleMovement();
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
            _currentSpeed = 0;
        }

        _rigidBody.velocity = new Vector2(_currentSpeed, _rigidBody.velocity.y);
    }
    
    public void HandleClimb(float climbDistance)
    {
        StartCoroutine(LadderClimbTick(climbDistance));
    }

    private IEnumerator LadderClimbTick(float distance)
    {
        UseLadder();
        yield return new WaitForFixedUpdate();
        _rigidBody.velocity = new Vector2(0, _climbSpeed);
        if (transform.position.y < distance) StartCoroutine(LadderClimbTick(distance));
        else EndLadder();
    }

    private void HandleAnimation()
    {
        if (_currentAction == Action.Run) _animator.SetInteger("AnimState", 1);
        if (_currentAction == Action.Stay) _animator.SetInteger("AnimState", 0);
    }

    private void UseLadder()
    {
        _animator.SetBool("Climbing", true);
        _currentAction = Action.Climb;
    }

    private void EndLadder()
    {
        _animator.SetBool("Climbing", false);
        _currentAction = Action.Stay;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IsInteractable>(out IsInteractable interactable))
        {
            interactable.Interact();
        }
    }
}
