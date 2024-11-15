using System.Collections;
using UnityEngine;

public class WarriorSpawner : MonoBehaviour
{
    [SerializeField] private Warrior _warriorPrefab;
    [SerializeField] private Transform _spawnpoint;
    [SerializeField] private float _timeBetweenSpawn = 5f;
    private IEnumerator SpawnTick()
    {
        yield return new WaitForSeconds(_timeBetweenSpawn);
        Instantiate(_warriorPrefab, _spawnpoint);
    }

    private void OnEnable()
    {
        StartCoroutine(SpawnTick());
    }

    private void OnDestroy()
    {
        StopCoroutine(SpawnTick());
    }


}



