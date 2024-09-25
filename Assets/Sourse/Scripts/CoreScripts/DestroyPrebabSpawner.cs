using UnityEngine;

public class DestroyPrebabSpawner : MonoBehaviour
{
    private DestroyButton _destroySprite;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {        
        if (_destroySprite == null)
            return;
        PrefabMovement();
    }

    public void DestroySpawner(DestroyButton Prefab)
    {
        _destroySprite = Instantiate(Prefab);
    }

    

    private void PrefabMovement()
    {
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(MousePosition.x);
        int y = Mathf.RoundToInt(MousePosition.y);

        _destroySprite.transform.position = new Vector3(x, y, 0);
    }

}
