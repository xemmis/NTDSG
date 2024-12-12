using UnityEngine;
using UnityEngine.UI;

public class WarriorUIHealthShower : MonoBehaviour
{
    [SerializeField] private Image _healthImage;
    [SerializeField] private Warrior _warrior;

    private void Start()
    {
        _warrior = GetComponent<Warrior>();
        _warrior.OnHitAction += ChangeUiHealth;
    }
    private void OnDestroy()
    {
        _warrior.OnHitAction -= ChangeUiHealth;
    }

    private void ChangeUiHealth()
    {
        _healthImage.fillAmount = _warrior.Health / _warrior.MaxHealth; ;
    }


}