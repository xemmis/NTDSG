using System.Collections;
using UnityEngine;

public class WarriorSpawner : MonoBehaviour
{


    [SerializeField] private NavigationBar _navBar;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private GameObject _warriorPrefab;

    [SerializeField] private GameObject _HireTransformPosition;

    [SerializeField] private Transform _spawnpoint;
    [Header("Стоимость Война")]
    [SerializeField] private int _cost;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        _wallet = _navBar.Wallet;
    }

    public void SpawnWarrior()
    {
        if (_wallet.CheckCost(_cost))
        {
            _wallet.SpendMoney(_cost);
            Vector3 point = new Vector3(_spawnpoint.localPosition.x, _spawnpoint.localPosition.y, Random.Range(0.001f, 2.999f));
            _spawnpoint.position = point;
            GameObject WarriorObject = Instantiate(_warriorPrefab, _spawnpoint);
            Warrior WarriorComponent = WarriorObject.GetComponent<Warrior>();
            GuardianAI guardianAI = WarriorObject.GetComponent<GuardianAI>();
            guardianAI.GetInput(_navBar.PcInput);
            WarriorComponent.HireShower.SetPosition(_HireTransformPosition);
            WarriorComponent.IsPlayerUnit = true;

        }


    }


    private void OnDestroy()
    {

    }


}



