using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthShower : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Image _healthImage;

    private void Awake()
    {
        _stats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        _stats.OnHitAction += ChangeUiHealth;
    }
    private void OnDestroy()
    {
        _stats.OnHitAction -= ChangeUiHealth;
    }

    private void ChangeUiHealth()
    {
        _healthImage.fillAmount = _stats.Health / _stats.MaxHealth;
    }
}
