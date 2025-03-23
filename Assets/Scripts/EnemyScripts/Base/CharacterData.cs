using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/Character Data", order = 51)]
public class CharacterData : ScriptableObject
{
    [Header("Speed Settings")]
    public float Speed;

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f; // Максимальное здоровье
    [SerializeField] private float _currentHealth; // Текущее здоровье
    [SerializeField] public bool Invincible;

    [Header("Damage Settings")]
    public float Damage;

    [Header("Custom Percent Items")]
    public List<PercentItem> PercentItems;

    // Событие для оповещения о смерти
    public delegate void OnDeath();
    public event OnDeath OnDeathEvent;

    // Свойство для доступа к текущему здоровью
    public float CurrentHealth => _currentHealth;

    // Инициализация здоровья
    public void Initialize()
    {
        _currentHealth = _maxHealth;
    }

    // Метод для получения урона
    public virtual void TakeDamage(float damage)
    {
        if (Invincible) return;
        if (_currentHealth <= 0) return; // Если уже мертв, ничего не делаем

        _currentHealth -= damage;
        _currentHealth = Mathf.Max(_currentHealth, 0); // Не даём здоровью уйти ниже 0

        Debug.Log($"{name} took {damage} damage. Current health: {_currentHealth}");

        // Проверяем, умер ли объект
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    // Метод для лечения
    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, _maxHealth); // Не даём здоровью превысить максимум

        Debug.Log($"{name} healed for {amount}. Current health: {_currentHealth}");
    }

    // Метод для обработки смерти
    private void Die()
    {
        Debug.Log($"{name} has died.");
        OnDeathEvent?.Invoke(); // Вызываем событие смерти
    }

    // Метод для создания глубокой копии CharacterData
    public CharacterData Clone()
    {
        // Создаём новый экземпляр CharacterData
        CharacterData clone = Instantiate(this);
        // Копируем простые поля
        clone.Speed = Speed;
        clone._maxHealth = _maxHealth;
        clone._currentHealth = _currentHealth;
        clone.Damage = Damage;        

        // Копируем список PercentItems (глубокая копия)
        clone.PercentItems = new List<PercentItem>();
        foreach (var item in PercentItems)
        {
            clone.PercentItems.Add(new PercentItem { Name = item.Name, Value = item.Value });
        }

        return clone;
    }
}

[System.Serializable] // Позволяет отображать класс в инспекторе
public class PercentItem
{
    public string Name; // Название элемента
    public float Value; // Значение элемента
}