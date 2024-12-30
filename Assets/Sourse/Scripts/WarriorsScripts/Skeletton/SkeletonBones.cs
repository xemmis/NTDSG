using UnityEngine;

public class SkeletonBones : MonoBehaviour, IsInteractable
{
    [field: SerializeField] public bool CanInteract { get; set; }
    public int MinEarnings;
    public int MaxEarnings;

    public NavigationBar NavigationBar;
    private bool _isInteracted = false;
    private int _earnings;
    public void Interact()
    {
        if (_isInteracted || !CanInteract)
        {
            print("Return");
            return;
        }
        _isInteracted = true;
        _earnings = Random.Range(MinEarnings, MaxEarnings);
        NavigationBar.Wallet.EarnMoney(_earnings);
        Destroy(gameObject);

    }
}
