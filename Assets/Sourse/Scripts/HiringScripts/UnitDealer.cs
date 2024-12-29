using UnityEngine;

public class UnitDealer : MonoBehaviour, IsInteractable
{
    private bool _isInteracted = false;
    [SerializeField] private GameObject _dialogeUi;

    public bool CanInteract { get ; set ; }

    public void Interact()
    {
        if (!CanInteract)
            return;
        _isInteracted = !_isInteracted;
        _dialogeUi.SetActive(_isInteracted);
        print("interacted");
    }

}
