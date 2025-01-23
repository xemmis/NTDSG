using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();
    [SerializeField] private List<GameObject> _Currentenemies = new List<GameObject>();
    [Inject] private Wallet _wallet;
    [SerializeField] private int _index = 0; // Начальное значение
    [SerializeField] private Transform[] _points;

    private void Start()
    {
        int safetyCounter = 0; // Защита от бесконечного цикла
        while (_Currentenemies.Count < 5 && safetyCounter < 100)
        {
            SpawnEnemy();
            safetyCounter++;
        }

        if (safetyCounter >= 100)
        {
            Debug.LogWarning("Достигнут лимит попыток спавна врагов. Проверьте настройки шанса появления.");
        }
    }

    private void SpawnEnemy()
    {
        if (_index >= _enemies.Count)
        {
            _index = 0; // Сброс индекса, если он вышел за пределы списка
        }

        GameObject enemyPrefab = _enemies[_index];

        // Проверка компонента Skeleton в дочерних объектах
        Skeleton skeleton = enemyPrefab.GetComponentInChildren<Skeleton>();
        if (skeleton != null)
        {
            if (skeleton.SpawnPersents > SetPercentsSpawn())
            {
                int randomPoint = Random.Range(0, _points.Length);
                GameObject newEnemy = Instantiate(enemyPrefab, _points[randomPoint].position, Quaternion.identity);

                // Получаем Skeleton у заспавненного врага (в дочерних объектах)
                Skeleton newSkeleton = newEnemy.GetComponentInChildren<Skeleton>();
                if (newSkeleton != null)
                {
                    newSkeleton.Wallet = _wallet;
                }

                newEnemy.transform.position = new Vector3(
                    newEnemy.transform.position.x,
                    newEnemy.transform.position.y,
                    Random.Range(1.0009f, 2.009f)
                );

                SpriteRenderer spriteRenderer = newEnemy.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 3;
                }

                _Currentenemies.Add(newEnemy);
            }
        }
        else
        {
            Debug.LogWarning($"Элемент с индексом {_index} в _enemies не содержит компонент Skeleton на дочерних объектах.");
        }

        _index++; // Увеличиваем индекс после каждой итерации
    }


    private float SetPercentsSpawn()
    {
        return Random.Range(0, 100);
    }
}

