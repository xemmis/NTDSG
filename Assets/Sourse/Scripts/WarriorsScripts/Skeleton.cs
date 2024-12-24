using UnityEngine;

public class Skeleton : Warrior
{


    public override void TakeHitAnimation()
    {
        CheckHealth();
        OnHitAction?.Invoke();
    }
}
