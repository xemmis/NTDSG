using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ladder : MonoBehaviour, IsInteractable
{
    [SerializeField] private Transform _ladderPosition;
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private Ladder _EndLadder;
    private BoxCollider2D _boxCollider;
    [field: SerializeField] public bool CanInteract { get; set; }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _boxCollider.isTrigger = true;
        CanInteract = true;
    }

    public void Interact()
    {
        if (CanInteract)
        {
            _player.HandleClimb(_ladderPosition);
            StartCoroutine(InteractTick());
        }
    }

    private IEnumerator InteractTick()
    {
        CanInteract = false;
        yield return new WaitForSeconds(5f);
        CanInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            _player = playerMovement;
        }
    }
}