using TMPro;
using UnityEngine;

public class WalletUIShower : MonoBehaviour
{
    [SerializeField] private NavigationBar _navBar;
    private Wallet _wallet;
    private TextMeshProUGUI _text;

    private void Start()
    {
        _wallet = _navBar.Wallet;
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _wallet.ChangeMoneyAction += ChangeWalletText;
        ChangeWalletText();
    }

    private void ChangeWalletText()
    {
        _text.text = _wallet.Money.ToString();
    }


}
