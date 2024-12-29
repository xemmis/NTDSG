using UnityEngine;

public class NavigationBar : MonoBehaviour
{
    [field: SerializeField] public Wallet Wallet {  get; private set; }
    [field: SerializeField] public BuildPlacementLogic BuildLogic {  get; private set; }
    [field: SerializeField] public PcInput PcInput { get; private set; }   
    
}
