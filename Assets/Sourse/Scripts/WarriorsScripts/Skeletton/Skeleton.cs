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


    public void ChangeObject()
    {
        _transform.position = transform.position;
        SkeletonBones Bones = Instantiate(_bonesPrefab, _transform);
        Bones.transform.position = new Vector3(
        Bones.transform.position.x,
        Bones.transform.position.y,
        Random.Range(1.0009f, 2.009f)
    
        );
        SpriteRenderer spriteRenderer = Bones.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 2;
        }
        Bones.NavigationBar = NavBar;
        Bones.MinEarnings = _minEarnings;
        Bones.MaxEarnings = _maxEarnings;
        Destroy(_logic);
        Destroy(this.gameObject);

    }


    public override void TakeHitAnimation()
    {
        CheckHealth();
        OnHitAction?.Invoke();
    }

}
