using UnityEngine;

public class DestroyPrebabSpawner : MonoBehaviour
{
    private DestroyButton _destroyPrefab; 

    public void DestroySpawner(DestroyButton Prefab)
    {
        _destroyPrefab = Instantiate(Prefab);
    }
}
