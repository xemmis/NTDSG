using UnityEngine;

public class DestroyBtnMovenent : MonoBehaviour
{
    [SerializeField] private DestroyButton _destroyButton;

    private void Start()
    {
        _destroyButton.GetComponent<DestroyButton>();
    }

    private void Update()
    {
        PrefabMovement();
    }

    private void PrefabMovement()
    {
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(MousePosition.x);
        int y = Mathf.RoundToInt(MousePosition.y);

        _destroyButton.transform.position = new Vector3(x, y, 0);
    }

}
