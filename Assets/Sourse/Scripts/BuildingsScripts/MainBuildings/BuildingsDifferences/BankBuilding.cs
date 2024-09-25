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

    public override void ChangeCondition(bool condition)
    {        
        _isPlacing = condition;
        if (!condition)
            StartCoroutine(LogicPerTick());
    }
    private IEnumerator LogicPerTick()
    {
        yield return new WaitForSeconds(_tickForGoldEarn);
        StartCoroutine(MoneyPerTick());
    }
    private IEnumerator MoneyPerTick()
    {
        _wallet.EarnMoney(_goldForTick * LevelOfBuilding);        
        yield return StartCoroutine(LogicPerTick());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
