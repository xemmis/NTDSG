using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerInput : MonoBehaviour
{
    private GameObject _interactableObject;
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private CircleCollider2D _circleCollider;
    private Animator _handler;
    private Camera _camera;
    private PlayerStats _playerStats;

    private bool _isCrouched = false;
    private bool _isAttacked = false;
    private IsInteractable _interactableComponent;
    private Vector3 _destination;

    [Space(20)]
    [Header("Аттрибуты скорости")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _deceleration = 35f;
    [Space(20)]
    [Header("Аттрибуты aттаки")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _comboTick = 35f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private int _attackCombo = 0;

    public Action<Transform> OnDestinationChanged;

    private void Awake()
    {
        _handler = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        _camera = Camera.main;
        _circleCollider.isTrigger = true;        
    }

    public void Update()
    {        
        CheckPlayerInput();
        MoveLogic();
    }

    private void CheckPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) print("escapeMenu");
        if (Input.GetKeyDown(KeyCode.M)) print("Map");
        if (Input.GetKeyDown(KeyCode.J)) print("Journal");
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_interactableObject == null) return;
            _interactableComponent.Interact();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isCrouched = !_isCrouched;
            _handler.SetBool("Crouch", _isCrouched);
        }
    }  

    private protected void ScanSurroundings()
    {
        float rayDistance = _attackRange; // Дальность луча
        Vector2 direction = _spriteRenderer.flipX ? Vector2.left : Vector2.right; // Направление луча в зависимости от flipX

        // Получаем все попадания луча
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, rayDistance);

        bool hitSomething = false; // Флаг для отслеживания успешного попадания

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null || hit.collider.gameObject == this.gameObject)
                continue; // Пропускаем собственный коллайдер

            GameObject obj = hit.collider.gameObject;
            print($"Hit: {obj.name}");

            if (obj.TryGetComponent<ISAlive>(out ISAlive aliveComponent) && aliveComponent.Health > 0)
            {
                print($"Dealing damage to: {obj.name}");
                aliveComponent.TakeHit(_playerStats.Strength);
                Debug.DrawRay(transform.position, direction * rayDistance, Color.red, 0.5f); // Визуализация успешного попадания
                hitSomething = true;
                return;
            }
        }

        if (!hitSomething)
        {
            print("No valid targets hit");
            Debug.DrawRay(transform.position, direction * rayDistance, Color.cyan, 0.5f); // Визуализация пустого луча
            return;
        }
    }
    private void MoveLogic()
    {
        if (_isCrouched)
            return;
        float horizontal = Input.GetAxis("Horizontal");
        bool jump = Input.GetButtonDown("Jump");
        Flip(horizontal);

        // Движение по горизонтали
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            _handler.SetInteger("AnimState", 2);
            _rigidBody.velocity = new Vector3(horizontal * _speed, _rigidBody.velocity.y, 0);
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            Vector3 currentVelocity = _rigidBody.velocity;
            currentVelocity.x = Mathf.Lerp(currentVelocity.x, 0, _deceleration * Time.fixedDeltaTime);
            _rigidBody.velocity = currentVelocity;
            _handler.SetInteger("AnimState", 1);
        }
    }

    private void Flip(float direction)
    {
        if (direction == 0) return;
        if (direction > 0)
            _spriteRenderer.flipX = false;
        else
            _spriteRenderer.flipX = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IsInteractable>(out IsInteractable component))
        {
            component.CanInteract = true;
            _interactableComponent = component;
            _interactableObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IsInteractable>(out IsInteractable component))
        {
            component.CanInteract = false;
            _interactableComponent = null;
            _interactableObject = null;
        }
    }
}

