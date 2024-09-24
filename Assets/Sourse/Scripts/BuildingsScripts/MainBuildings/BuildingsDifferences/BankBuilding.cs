using System.Collections;
using UnityEngine;

public class BankBuilding : Building
{

    [SerializeField] private Wallet _wallet;
    [SerializeField] private float _tickForGoldEarn = 5f;
    [SerializeField] private int _goldForTick = 5;

    private void Start()
    {
        _wallet = _navBar.Wallet;
    }

    private IEnumerator MoneyPerTick()
    {
        yield return new WaitForSeconds(_tickForGoldEarn);
        _wallet.EarnMoney(_goldForTick * LevelOfBuilding);
        ChangeCondition(false);
    }
    public override void ChangeCondition(bool condition)
    {
        print(condition);
        _isPlacing = condition;
        if (!condition)
            StartCoroutine(MoneyPerTick());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
