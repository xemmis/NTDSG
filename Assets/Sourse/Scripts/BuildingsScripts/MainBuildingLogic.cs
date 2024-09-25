using System;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class MainBuildingLogic : MonoBehaviour
{
    private Rigidbody2D _rigidBody2d;
    private BoxCollider2D _boxCollider;
    [SerializeField] private bool _haveMoneyForBuild;
    [field: SerializeField] public Building Build { get; private set; }
    [field: SerializeField] public bool _canBuild { get; private set; }    


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

    private void ConditionEvent()
    {
        Build.ChangeCondition(false);
    }

    public void CheckCostToWallet(Wallet wallet)
    {
        _haveMoneyForBuild = wallet.CheckBuildingCost(Build.BuildCost);
    }

    public void CheckBuildPermission()
    {
        if (_canBuild && _haveMoneyForBuild)
        {
            Build.ChangeColor(new Color(0, 255, 0, .50f));
        }
        else
        {
            Build.ChangeColor(new Color(255, 0, 0, .50f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {          
        if (collision.TryGetComponent<MainBuildingLogic>(out MainBuildingLogic AnotherBuilding))
        {
            _canBuild = false;
            CheckBuildPermission();
        }

        if (collision.TryGetComponent<BuildingZone>(out BuildingZone Zone) && _haveMoneyForBuild)
        {
            _canBuild = true;
            CheckBuildPermission();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {          
        if (collision.TryGetComponent<MainBuildingLogic>(out MainBuildingLogic AnotherBulding) && _haveMoneyForBuild)
        {
            _canBuild = true;
            CheckBuildPermission();
        }
        if (collision.TryGetComponent<BuildingZone>(out BuildingZone zone))
        {
            _canBuild = false;
            CheckBuildPermission();
        }
    }
}
