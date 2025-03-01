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
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] CharacterData _characterData;
    private IsInteractable _object;
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

    public void HandleClimb(Transform climbPositon)
    {
        print(climbPositon);
        StartCoroutine(LadderClimbTick(climbPositon));
    }

    private IEnumerator LadderClimbTick(Transform targetY)
    {
        UseLadder(); // Начинаем использовать лестницу

        // Определяем направление движения
        float direction = targetY.position.y > transform.position.y ? 1 : -1;

        // Двигаемся в направлении targetY
        _rigidBody.velocity = new Vector2(0, _climbSpeed * direction);

        // Ждём следующего кадра
        yield return null;

        // Проверяем, достигли ли мы целевой позиции
        if ((direction > 0 && transform.position.y < targetY.position.y) ||
            (direction < 0 && transform.position.y > targetY.position.y))
        {
            // Если не достигли, продолжаем движение
            StartCoroutine(LadderClimbTick(targetY));
        }
        else
        {
            // Если достигли, завершаем движение
            EndLadder();
        }
    }
    private void HandleAnimation()
    {
        if (_currentAction == Action.Run)
        {
            _animator.SetInteger("AnimState", 1);
        }

        if (_currentAction == Action.Stay)
        {
            _animator.SetInteger("AnimState", 0);
        }
        if (Input.GetKeyDown(KeyCode.C) && _currentAction == Action.Run) _animator.SetTrigger("Slide");
    }

    public void StartSlide()
    {
        _characterData.Invincible = true;
        int currentLayer = gameObject.layer;
        int enemyLayer = GetFirstLayerFromMask(_enemyLayer);
        Physics2D.IgnoreLayerCollision(currentLayer, enemyLayer, true);
    }

    public void EndSlide()
    {
        _characterData.Invincible = false;
        int currentLayer = gameObject.layer;
        int enemyLayer = GetFirstLayerFromMask(_enemyLayer);
        Physics2D.IgnoreLayerCollision(currentLayer, enemyLayer, false);
    }

    private void UseLadder()
    {
        int currentLayer = gameObject.layer;
        int groundLayer = LayerMask.NameToLayer("Ground");
        _animator.SetBool("Climbing", true);
        Physics2D.IgnoreLayerCollision(currentLayer, groundLayer, true);
        _currentAction = Action.Climb;
    }

    private int GetFirstLayerFromMask(LayerMask layerMask)
    {
        int layerNumber = 0;
        int mask = layerMask.value;

        if (mask == 0) return -1; // Если маска пуста

        while ((mask & 1) == 0) // Ищем первый установленный бит
        {
            mask >>= 1;
            layerNumber++;
        }

        return layerNumber;
    }

    private void EndLadder()
    {
        _rigidBody.velocity = Vector2.zero;
        int currentLayer = gameObject.layer;
        int groundLayer = LayerMask.NameToLayer("Ground");
        Physics2D.IgnoreLayerCollision(currentLayer, groundLayer, false);
        _animator.SetBool("Climbing", false);
        _currentAction = Action.Stay;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        print("1");
        if (collision.TryGetComponent<IsInteractable>(out IsInteractable interactable))
        {
            if (Input.GetKey(KeyCode.E))
            {
                interactable.Interact();
            }
        }
    }
}
