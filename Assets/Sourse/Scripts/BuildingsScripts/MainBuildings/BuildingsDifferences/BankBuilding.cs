using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankBuilding : Building
{
    [field: SerializeField] public int LevelOfBuilding {  get;  private set; }
    [SerializeField] private Wal,let _wallet;
    [SerializeField] private float _tickForGoldEarn = 5f;
    public NavigationBar NavBar;

    public void ChangeLevel(int level)
    {
        LevelOfBuilding = level;
    }


    private IEnumerator MoneyPerTick()
    {
        yield return new WaitForSeconds(_tickForGoldEarn);
    }
}
