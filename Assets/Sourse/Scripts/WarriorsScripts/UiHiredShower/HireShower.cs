using UnityEngine;

public class HireShower : MonoBehaviour
{
    [SerializeField] private GameObject _uiPosition;
    [SerializeField] private GameObject _uiPrefab;
    [SerializeField] private GameObject _instantiatedUi;

    public void SetPosition(GameObject position) 
    {
        _uiPosition = position;
    }

    private void Start()
    {
        _instantiatedUi = Instantiate(_uiPrefab, _uiPosition.transform);
    }

    private void OnDisable()
    {
        Destroy(_instantiatedUi);
    }
}
