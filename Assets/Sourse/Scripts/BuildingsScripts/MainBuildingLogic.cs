using System;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class MainBuildingLogic : MonoBehaviour
{
    private Rigidbody2D _rigidBody2d;
    private BoxCollider2D _boxCollider;
    [field: SerializeField] public bool IsSpawned { get; private set; }
    [field: SerializeField] public Building Build { get; private set; }
    [field: SerializeField] public bool CanBuild { get; private set; }
    public Action IsPlaced;


    private void Awake()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _rigidBody2d.gravityScale = 0;
        _boxCollider.isTrigger = true;
    }

    public void ChangeCondition(bool Spawed)
    {
        IsSpawned = Spawed;
    }

    public bool CheckCostToWallet(Wallet wallet)
    {         
        return wallet.CheckCost(Build.BuildCost); 
    }

    public void HandleEnter(Collider2D collision)
    {
        if (collision.TryGetComponent<MainBuildingLogic>(out MainBuildingLogic AnotherBuilding))
        {
            CanBuild = false;
        }

        if (collision.TryGetComponent<BuildingZone>(out BuildingZone Zone) && CheckCostToWallet(Build._navBar.Wallet))
        {
            CanBuild = true;
        }
    }

    public void HandleExit(Collider2D collision)
    {
        if (collision.TryGetComponent<MainBuildingLogic>(out MainBuildingLogic AnotherBuilding))
        {
            CanBuild = true;
        }

        if (collision.TryGetComponent<BuildingZone>(out BuildingZone Zone))
        {
            CanBuild = false;
        }
    }

}
