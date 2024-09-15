using UnityEngine;

public class Wallet : MonoBehaviour
{
    [field: SerializeField] public int Money { get; private set; }

    public void SpendMoney(int cost)
    {
        if (CheckBuildingCost(cost))
            Money -= cost;
        print(Money);
    }

    public bool CheckBuildingCost(int cost)
    {
        if (Money < cost)
            return false;
        else
            return true;
    }
    public void EarnMoney(int EarnedMoney)
    {
        Money += EarnedMoney;
        print(Money);
    }
}
