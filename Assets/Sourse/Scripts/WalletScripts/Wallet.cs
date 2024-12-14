using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field: SerializeField] public int Money { get; private set; }
    public Action ChangeMoneyAction;
    public void SpendMoney(int cost)
    {
        if (CheckCost(cost))
            Money -= cost;
        ChangeMoneyAction?.Invoke();
    }

    public bool CheckCost(int cost)
    {
        if (Money < cost)
            return false;
        else
            return true;
    }
    public void EarnMoney(int EarnedMoney)
    {
        Money += EarnedMoney;
        ChangeMoneyAction?.Invoke();        
    }
}
