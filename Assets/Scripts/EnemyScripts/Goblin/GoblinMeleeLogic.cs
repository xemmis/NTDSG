using System.Collections;
using UnityEngine;

public class GoblinMeleeLogic : BaseMeleeLogic
{
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private float _specialCooldown;
    [SerializeField] private bool _specialIsReady;
    [SerializeField] private bool _canRunAway;


    public override void TakeHit(float damage)
    {
        if (_stats.CurrentHealth <= 0) return;
        _stats.TakeDamage(damage);
        _animator.SetTrigger("Damaged");

        if (_stats.CurrentHealth <= 30f)
        {
            _canRunAway = true;
            if (SpecialHandler()) Action = EnemyAction.RunAway;
        }

    }

    public override void RunAwayHandler()
    {
        Vector2 direction = (transform.position - _playerStats.gameObject.transform.position).normalized;
        if (_isDead)
        { 
            return;
        }
        if (CheckArea()) _rigidBody.velocity = direction * _stats.Speed;
        else
        {
            Action = EnemyAction.Idle;
            _animator.SetBool("Avoiding", false);
            return;
        }
        Action = EnemyAction.RunAway;
        _animator.SetBool("Avoiding", true);
        _spriteRenderer.flipX = direction.x < 0 ? true : direction.x > 0 ? false : _spriteRenderer.flipX;
    }

    public override bool SpecialHandler()
    {
        if (_canRunAway)
        {
            float r1 = Random.Range(0, 100);
            float r2 = Random.Range(0, 100);
            for (int i = 0; i < _stats.PercentItems.Count; i++)
            {
                if (_stats.PercentItems[i].Name == "Coward")
                {
                    _stats.PercentItems[i].Value = r2;
                    if (r1 < r2)
                    {
                        _canRunAway = false;
                        return true;
                    }
                }
            }
            _canRunAway = false;
            return false;
        }
        return base.SpecialHandler();
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
