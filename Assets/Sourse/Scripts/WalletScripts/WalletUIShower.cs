using TMPro;
using UnityEngine;
using Zenject;

public class WalletUIShower : MonoBehaviour
{
    
    [Inject] private Wallet _wallet;
    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _wallet.ChangeMoneyAction += ChangeWalletText;
        ChangeWalletText();
    }

    private void ChangeWalletText()
    {
        _text.text = _wallet.Money.ToString();
    }


}
