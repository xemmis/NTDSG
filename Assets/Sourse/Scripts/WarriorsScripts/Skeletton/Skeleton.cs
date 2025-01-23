using UnityEngine;
using Zenject;

public class Skeleton : Warrior, IsEnemy, ISAlive
{
    [field: SerializeField] public float SpawnPersents { get; set; }
    [Inject] public Wallet Wallet;
    [SerializeField] private int _minEarnings;
    [SerializeField] private int _maxEarnings;
    [SerializeField] private float _percents;
    [SerializeField] private float _blockPercents;
    public override void TakeHit(float Damage)
    {
        _percents = Random.Range(0, 100);
        if (_percents > _blockPercents)
        {
            base.TakeHit(Damage);
        }
        else
        {
            _animator.SetTrigger("Block");
            CheckHealth();
        }
    }
}
