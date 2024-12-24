using System.Collections;
using UnityEngine;

public class WarriorSpawner : MonoBehaviour
{


    [SerializeField] private NavigationBar _navBar;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private GameObject _warriorPrefab;
    [Header("Точки Патруля")]
    [SerializeField] private Transform _spawnpoint;
    [SerializeField] private Transform _standartPosition;
    [SerializeField] private Transform[] _patrolPoints;
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
            GameObject WarriorObject = Instantiate(_warriorPrefab, _spawnpoint);
            BaseAILogic WarriorAI = WarriorObject.GetComponent<BaseAILogic>();
            Warrior WarriorComponent = WarriorObject.GetComponent<Warrior>();
            WarriorComponent.IsPlayerUnit = true;
            WarriorAI.GetPatrolPoints(_patrolPoints);

        }


    }


    private void OnDestroy()
    {

    }


}



