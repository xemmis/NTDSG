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
        if (Health > 0 || _isInteracted || !CanInteract)
        {
            print("Return");
            return;
        }
        _isInteracted = true;
        _earnings = Random.Range(_minEarnings, _maxEarnings);
        _navBar.Wallet.EarnMoney(_earnings);
        
        print("ne return" + _earnings);
    }

    public override void TakeHitAnimation()
    {
        CheckHealth();
        OnHitAction?.Invoke();
    }
}
