using System;
using UnityEngine;

public class BuildPlacementLogic : MonoBehaviour
{
    [SerializeField] private MainBuildingLogic _flyingBuilding;
    [SerializeField] private NavigationBar _navigationBar;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private Building _building;
    [SerializeField] private Collider2D _anotherCollider;
    [SerializeField] private bool _isEnter;
    public Action IsPlaced;

    private void Start()
    {
        _flyingBuilding = GetComponent<MainBuildingLogic>();
        _building = GetComponentInChildren<Building>();
        _navigationBar = _building._navBar;
        _wallet = _navigationBar.Wallet;
    }

    private void Update()
    {
        CheckLogic();
    }

    private void CheckLogic()
    {

        if (_anotherCollider != null)
        {
            if (_isEnter)
                _flyingBuilding.HandleEnter(_anotherCollider);
            else if (!_isEnter)
                _flyingBuilding.HandleExit(_anotherCollider);
        }

        if (Input.GetMouseButtonDown(0) && _flyingBuilding.CanBuild)
        {

            _wallet.SpendMoney(_flyingBuilding.Build.BuildCost);
            _flyingBuilding.Build.ChangeCondition(true);
            IsPlaced?.Invoke();
            Destroy(this);
        }
        if (Input.GetMouseButtonDown(1))
            Destroy(_flyingBuilding.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _anotherCollider = collision;
        _isEnter = true;
        _flyingBuilding.HandleEnter(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _anotherCollider = collision;
        _isEnter = false;
        _flyingBuilding.HandleExit(collision);
    }
}
