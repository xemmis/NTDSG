using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class GuardianAI : BaseAILogic
{
    [SerializeField] private Transform _target;
    [SerializeField] private PlayerInput _pcInput;
    [SerializeField] private float _stopDistance;

    private void Start()
    {
        _isChilled = true;
        _state = AIState.Idle;
        _stopDistance = Random.Range(0.1f, 1.995f);
    }

    public void GetInput(PlayerInput input)
    {
        _pcInput = input;
        _pcInput.OnDestinationChanged += ChangePoint;
    }

    private void OnDisable()
    {
        _pcInput.OnDestinationChanged -= ChangePoint;
    }

    private void ChangePoint(Transform destination)
    {       
           
        Flip(destination);
        destination.position = new Vector2(destination.position.x + Random.Range(-.55f, .55f), destination.position.y);
        destination.position = new Vector2(destination.position.x, destination.position.y + Random.Range(.55f, -.55f));
        _destination = destination;
        _state = AIState.Patrol;
    }

    public override void CheckState()
    {
        base.CheckState();
        switch (_state)
        {
            case AIState.Patrol:
                _animator.SetInteger("AnimState", 2);
                HanlePatrol();
                ScanSurroundings();
                break;

            default: break;
        }
    }

    public override void HandleIdle()
    {
        base.HandleIdle();
        ScanSurroundings();
    }

    public override void HandleChase()
    {
        base.HandleChase();
    }
    
    private void HanlePatrol()
    {
        if ((transform.position - _destination.position).magnitude < _stopDistance)
        {
            print("stop");
            _state = AIState.Idle;
            return;
        }

        _state = AIState.Patrol;
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, _thisWarrior.MovementSpeed * Time.deltaTime);
    }
    
}



