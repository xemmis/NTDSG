using System;
using UnityEngine;

public class MainBuildingSpawner : MonoBehaviour
{
    [SerializeField] private MainBuildingLogic _flyingBuilding;
    [SerializeField] private NavigationBar _navigationBar;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private BuildPlacementLogic _buildPlacementLogic;      

    void Start()
    {
        _wallet = _navigationBar.Wallet;        
    }

    private void IsPlacedIvent()
    {        
        _flyingBuilding = null;
        _buildPlacementLogic.IsPlaced -= IsPlacedIvent;
        _buildPlacementLogic = null;
    }

    public void StartPlacingBuilding(MainBuildingLogic BuildingPrefab)
    {
        if (_flyingBuilding != null)
            return;
        _flyingBuilding = Instantiate(BuildingPrefab);
        _buildPlacementLogic = _flyingBuilding.GetComponent<BuildPlacementLogic>();
        _buildPlacementLogic.IsPlaced += IsPlacedIvent;
        _flyingBuilding.Build.TakeNavigationBar(_navigationBar);        
        _flyingBuilding.CheckCostToWallet(_wallet);
        _flyingBuilding.ChangeCondition(true);
    }


}
