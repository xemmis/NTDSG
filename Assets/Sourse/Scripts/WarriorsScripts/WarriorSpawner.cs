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

    private void ChangePatrolPoint()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        else
            return;
    }

    public void SpawnWarrior()
    {
        if(_wallet.CheckCost(_cost))
        {
            _wallet.SpendMoney(_cost);
            GameObject WarriorObject = Instantiate(_warriorPrefab,_spawnpoint);
            WarriorAILogic WarriorAI = WarriorObject.GetComponent<WarriorAILogic>();
            Warrior WarriorComponent = WarriorObject.GetComponent<Warrior>();
            WarriorComponent.IsPlayerUnit = true;
            WarriorAI.GetPatrolPoints(_patrolPoints);
            
        }


    }


    private void OnDestroy()
    {

    }


}



