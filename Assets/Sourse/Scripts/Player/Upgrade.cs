using System;
using TMPro;
using UnityEngine;
using Zenject;

public class Upgrade : MonoBehaviour
{
    [Inject] private Wallet _wallet;
    [field: SerializeField] public static int ArmorCount { get; private set; }
    [field: SerializeField] public static int WeaponCount { get; private set; }

    [SerializeField] private GameObject _upgradeUI;
    [SerializeField] private GameObject[] _armorUI;
    [SerializeField] private GameObject[] _weaponUI;


    [SerializeField] private TextMeshProUGUI _weaponCostText;
    [SerializeField] private TextMeshProUGUI _armorCostText;
    [SerializeField] private int _weaponUpgradeCost;
    [SerializeField] private int _armorUpgradeCost;

    public static Action OnArmorUpgrade;
    public static Action OnWeaponUpgrade;

    private void Start()
    {
        _weaponUpgradeCost = 50;
        _armorUpgradeCost = 50;
        _upgradeUI.SetActive(false);
    }

    private void UpdateCostText(TextMeshProUGUI text,int cost)
    {
        text.text = cost.ToString();
    }

    private void OnEnable()
    {
        UpdateCostText(_armorCostText, _armorUpgradeCost);
        UpdateCostText(_weaponCostText, _weaponUpgradeCost);
    }

    public void UpgradeWeapon()
    {
        if (WeaponCount > _weaponUI.Length - 1)
            return;
        if (_wallet.CheckCost(_weaponUpgradeCost))
            _wallet.SpendMoney(_weaponUpgradeCost);
        else
            return;
        _weaponUpgradeCost *= 2;
        UpdateCostText(_weaponCostText, _weaponUpgradeCost);
        _weaponUI[WeaponCount].SetActive(true);
        WeaponCount++;
        OnWeaponUpgrade?.Invoke();
    }

    public void UpgradeArmor()
    {
        if (ArmorCount > _armorUI.Length - 1)
            return;
        if (_wallet.CheckCost(_armorUpgradeCost))
            _wallet.SpendMoney(_armorUpgradeCost);
        else
            return;
        _armorUpgradeCost *= 2;
        UpdateCostText(_armorCostText, _armorUpgradeCost);
        _armorUI[ArmorCount].SetActive(true);
        ArmorCount++;
        OnArmorUpgrade?.Invoke();
    }
}
