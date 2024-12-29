using UnityEngine;

public class Skeleton : Warrior, IsInteractable
{
    public bool CanInteract { get; set; }
    private bool _isInteracted = false;
    private int _earnings;

    [SerializeField] private int _minEarnings;
    [SerializeField] private int _maxEarnings;
    [SerializeField] NavigationBar _navBar;


    public void Interact()
    {
        if (Health > 0 || _isInteracted)
            return;
        _isInteracted = true;
        _earnings = Random.Range(_minEarnings, _maxEarnings);
        _navBar.Wallet.EarnMoney(_earnings);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PcInput>(out PcInput pcInput))
        {
            CanInteract = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PcInput>(out PcInput pcInput))
        {
            CanInteract = false;
        }
    }

    public override void TakeHitAnimation()
    {
        CheckHealth();
        OnHitAction?.Invoke();
    }
}
