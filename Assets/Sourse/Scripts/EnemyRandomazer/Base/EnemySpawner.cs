using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemies = new List<GameObject>();
    [SerializeField] private List<GameObject> _Currentenemies = new List<GameObject>();
    [SerializeField] private NavigationBar _navigationBar;
    [SerializeField] private int _index;
    [SerializeField] private int _currentindex;
    [SerializeField] private Transform[] _points;

    private void Start()
    {
        while (_Currentenemies.Count < 5)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (_enemies[_index].TryGetComponent<Skeleton>(out Skeleton skeleton))
        {
            if (_index > _enemies.Count - 1)
                _index = 0;
            if (skeleton.SpawnPersents > SetPercentsSpawn())
            {
                int randomPoint = Random.Range(0, 4);
                GameObject newEnemy = Instantiate(_enemies[_index], _points[randomPoint]);
                skeleton.NavBar = _navigationBar;
                skeleton.transform.position = new Vector3(
                skeleton.transform.position.x,
                skeleton.transform.position.y,
                Random.Range(1.0009f, 2.009f)
                );

                SpriteRenderer spriteRenderer = newEnemy.GetComponent<SpriteRenderer>();

                if (spriteRenderer != null)
                {
                    spriteRenderer.sortingOrder = 3;
                }

                _Currentenemies.Add(newEnemy);
            }
            _index++;
        }
        else
            Debug.LogWarning("тут не враг в массиве дурень");
    }


    private float SetPercentsSpawn()
    {
        return Random.Range(0, 100);
    }
}

