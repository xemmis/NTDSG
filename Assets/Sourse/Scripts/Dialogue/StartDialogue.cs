using UnityEngine;

public class StartDialogue : MonoBehaviour, IsInteractable
{
    [SerializeField] private DialogueSystem _dialogueSystem;
    [SerializeField] private GameObject _text;
    [SerializeField] private GameObject _EInteractUI;
    public bool CanInteract { get; set; }
    private bool _isInteracted;

    private void Start()
    {
        _EInteractUI.SetActive(false);
    }

    public void Interact()
    {
        if (!CanInteract) return;

        _isInteracted = true;
        _EInteractUI.SetActive(false);
        _dialogueSystem.ShowDialogue("Greetings");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
        {

            _isInteracted = false;
            _EInteractUI.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
        {
            _EInteractUI.SetActive(true);
        }
    }
}
