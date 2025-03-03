using System.Collections;
using UnityEngine;

public class GoblinMeleeLogic : BaseMeleeLogic
{
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private float _specialCooldown;
    [SerializeField] private bool _specialIsReady;


    public override void TakeHit(float damage)
    {
        if (_stats.CurrentHealth <= 0) return;
        _stats.TakeDamage(damage);
        _animator.SetTrigger("Damaged");
    }

    private void ThrowBomb()
    {
        GameObject newBombObj = Instantiate(_bombPrefab, transform.position, Quaternion.identity);
        Bomb bombComp = newBombObj.GetComponent<Bomb>();
        StartCoroutine(SpecialAttackReload());
        Vector2 direction = _spriteRenderer.flipX ? new Vector2(-1, 1) : new Vector2(1, 1);
        direction.Normalize();
        bombComp.BombFly(direction);
    }

    private IEnumerator SpecialAttackReload()
    {
        _specialIsReady = false;
        yield return new WaitForSeconds(_specialCooldown);
        _specialIsReady = true;
    }

    public override void AttackHandler()
    {        
        if (!_specialIsReady)
        {
            base.AttackHandler();
            return;
        }

        if (SpecialHandler())
        {
            StartCoroutine(AttackTick());
            StartCoroutine(SpecialAttackReload());
            return;
        }
        base.AttackHandler();

    }

}
