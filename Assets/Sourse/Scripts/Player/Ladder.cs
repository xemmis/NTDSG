using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ladder : MonoBehaviour, IsInteractable
{
    [SerializeField] private float _ladderHeight;
    [SerializeField] private PlayerMovement _player;
    private BoxCollider2D _boxCollider;

    public bool CanInteract { get; set; }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _boxCollider.isTrigger = true;        
    }

    public void Interact()
    {
        if (CanInteract)
        {
            _player.HandleClimb(_ladderHeight);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            _player = playerMovement;
            CanInteract = true;
        }
    }
}