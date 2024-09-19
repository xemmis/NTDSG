using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]

public class MainBuildingLogic : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _boxCollider;      
    [SerializeField] private bool _isBuilded;       
    [SerializeField] private bool _haveMoneyForBuild;    
    [field: SerializeField] public Building Build {  get; private set; }    
    [field: SerializeField] public bool _canBuild { get; private set; }

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();        
    }

    private void Start()
    {   
        if (Build == null)
        {
            Debug.LogError("Building component not found in children!");
        }

        _boxCollider.isTrigger = true;
    }  

    public void CheckCostToWallet(Wallet wallet)
    {        
        _haveMoneyForBuild = wallet.CheckBuildingCost(Build.BuildCost);
        print(_haveMoneyForBuild);
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
