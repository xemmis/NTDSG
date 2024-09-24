using System;
using UnityEngine;

public class BuildPlacementLogic : MonoBehaviour
{
    [SerializeField] private MainBuildingLogic _flyingBuilding;
    [SerializeField] private NavigationBar _navigationBar;
    [SerializeField] private Wallet _wallet;

    public event Action IsBuilded;
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        _wallet = _navigationBar.Wallet;
    }

    private void Update()
    {
        CheckLogic();
    }

    private void CheckLogic()
    {
        if (_flyingBuilding == null)
            return;
        MoveFlyingBuilding();

        if (Input.GetMouseButtonDown(0) && _flyingBuilding._canBuild)
        {
            _wallet.SpendMoney(_flyingBuilding.Build.BuildCost);
            IsBuilded?.Invoke();
            _flyingBuilding = null;
        }

        if (Input.GetMouseButtonDown(1))
            Destroy(_flyingBuilding.gameObject);
    }

    public void StartPlacingBuilding(MainBuildingLogic BuildingPrefab)
    {
        if (_flyingBuilding != null)
            return;
        _flyingBuilding = Instantiate(BuildingPrefab);
        _flyingBuilding.Build.TakeNavigationBar(_navigationBar);
        _flyingBuilding.SubscribeToIsBuildAction(IsBuilded);
        _flyingBuilding.CheckCostToWallet(_wallet);
    }

    private void MoveFlyingBuilding()
    {
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(MousePosition.x);
        int y = Mathf.RoundToInt(MousePosition.y);

        _flyingBuilding.transform.position = new Vector3(x, y, 0);
    }
}
