using System;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PcInput : Warrior
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private Vector3 _destination;
    [SerializeField] private Transform _destinationTransform;

    [SerializeField] private GameObject _interactableObject;
    private Rigidbody2D _rigidBody;
    private CircleCollider2D _circleCollider;
    private Camera _camera;

    private GameObject _escapeUIMenu;
    private GameObject _mapGui;

    public Action<Transform> OnDestinationChanged;



    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        _camera = Camera.main;
        _circleCollider.isTrigger = true;
        _rigidBody.gravityScale = 0;
    }

    public void Update()
    {
        CheckPlayerInput();
        MoveLogic();
        ClickLogic();
    }

    private void CheckPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) print("escapeMenu");
        if (Input.GetKeyDown(KeyCode.M)) print("Map");
        if (Input.GetKeyDown(KeyCode.J)) print("Journal");
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsInteractable interactComponent = _interactableObject.GetComponent<IsInteractable>();
            interactComponent.Interact();
        }
    }

    private void ClickLogic()
    {
        Vector3 clickPosition;
        if (Input.GetMouseButton(1)) // Правая кнопка мыши
        {
            // Преобразуем экранные координаты мыши в мировые координаты
            clickPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            // Устанавливаем z-позицию камеры (или другого объекта) для корректного преобразования
            clickPosition.z = _camera.transform.position.z;

            // Сохраняем позицию назначения
            _destination = clickPosition;

            _destinationTransform.position = _destination;
            // Вызываем событие, если оно подписано
            OnDestinationChanged?.Invoke(_destinationTransform);
        }
    }


    private void MoveLogic()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
        }
        _rigidBody.velocity = new Vector3(moveDirection.x * _speed, moveDirection.z * _speed, 0);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<UnitDealer>(out UnitDealer component))
        {
            component.CanInteract = true;
            _interactableObject = component.gameObject;
        }
        if (collision.TryGetComponent<Skeleton>(out Skeleton skeleton))
        {
            _interactableObject = skeleton.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IsInteractable>(out IsInteractable component))
        {
            component.CanInteract = false;
            _interactableObject = null;
        }
    }
}