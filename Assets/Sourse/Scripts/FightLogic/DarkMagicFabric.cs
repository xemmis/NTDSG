using System;
using System.Collections.Generic;
using UnityEngine;


public class DarkMagicFabric : MonoBehaviour, ISpellFabric
{
    [SerializeField] private Transform _heroTransform;

    [Serializable]
    public struct SpellEntry
    {
        public string SpellName;
        public GameObject SpellPrefab;
    }

    [SerializeField]
    private List<SpellEntry> spellEntries; // Список для настройки заклинаний в инспекторе

    private Dictionary<string, GameObject> spellPrefabs; // Словарь для быстрого доступа к заклинаниям

    private void Start()
    {
        // Инициализируем словарь на основе списка
        spellPrefabs = new Dictionary<string, GameObject>();
        foreach (var entry in spellEntries)
        {
            if (!spellPrefabs.ContainsKey(entry.SpellName))
            {
                spellPrefabs.Add(entry.SpellName, entry.SpellPrefab);
            }
            else
            {
                Debug.LogWarning($"Заклинание с именем {entry.SpellName} уже существует в словаре.");
            }
        }
    }

    public ISpell SpelInit(string spellName, Transform fireFlyPos)
    {
        if (spellPrefabs.TryGetValue(spellName, out GameObject spellPrefab))
        {
            GameObject spellObj = Instantiate(spellPrefab, fireFlyPos.position, fireFlyPos.rotation);
            ISpell spellComp = spellObj.GetComponent<ISpell>();            
            if (spellComp == null)
            {
                Debug.LogError($"Префаб заклинания {spellName} не содержит компонента, реализующего ISpell.");
                return null;
            }
            spellComp.TakeHeroComponent(_heroTransform);
            return spellComp;
        }
        else
        {
            Debug.LogError($"Заклинание с именем {spellName} не найдено.");
            return null;
        }
    }
}
