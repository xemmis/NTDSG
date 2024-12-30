using System.ComponentModel;
using UnityEngine;

public class Skeleton : Warrior, IsEnemy
{    
    [field: SerializeField] public float SpawnPersents { get; set; }
    [SerializeField] public NavigationBar NavBar;

    [SerializeField] private Transform _transform;
    [SerializeField] private SkeletonBones _bonesPrefab;
    [SerializeField] private BaseAILogic _logic;
    [SerializeField] private int _minEarnings;
    [SerializeField] private int _maxEarnings;


    public override void CheckHealth()
    {
        base.CheckHealth();
        SkeletonBones Bones = Instantiate(_bonesPrefab, _transform);
        Bones.NavigationBar = NavBar;
        Bones.MinEarnings = _minEarnings;
        Bones.MaxEarnings = _maxEarnings;        
        Destroy(_logic);
        Destroy(this);
    }

    public override void TakeHitAnimation()
    {
        CheckHealth();
        OnHitAction?.Invoke();
    }

}
