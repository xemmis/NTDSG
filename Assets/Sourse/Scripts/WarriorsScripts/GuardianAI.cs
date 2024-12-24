using System.Collections;
using UnityEngine;

public class GuardianAI : BaseAILogic
{
    [SerializeField] private Transform[] _patrolPoints;
    private int _currentPointIndex = 0;
    private void Start()
    {
        _isChilled = true;
        _state = AIState.Patrol;
    }

    public override void CheckState()
    {
        base.CheckState();
        switch (_state)
        {
            case AIState.Patrol:
                ChangeAnimState(1);
                ScanSurroundings();
                HanlePatrol();
                break;

            default: break;
        }
    }

    public override void HandleIdle()
    {
        base.HandleIdle();
        if (_isChilled)
            _state = AIState.Patrol;

        if (!_isChilled)
            StartCoroutine(ChillTick());
    }

    private IEnumerator ChillTick()
    {
        yield return new WaitForSeconds(1.5f);
        _isChilled = true;
    }

    private void HanlePatrol()
    {
        if (!_isChilled)
        {
            _state = AIState.Idle;
            return;
        }
        Transform targetPoint = _patrolPoints[_currentPointIndex];
        Flip(_patrolPoints[_currentPointIndex]);

        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, _thisWarrior.MovementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            _currentPointIndex = (_currentPointIndex + 1) % _patrolPoints.Length;
            _isChilled = false;
        }
    }
}