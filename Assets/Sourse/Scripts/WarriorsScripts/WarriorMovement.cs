using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class WarriorMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CircleCollider2D _circleCollider;
    [SerializeField] private GameObject _ProtectPosition;
    private Tween _tween;
    private float _changeInterval = 2f;
    private float _movementSpeed = 5f;
    private void Start()
    {
        MoveToPoint();
    }

    private void MoveToPoint() 
    {
        _tween = transform.DOMove(_ProtectPosition.transform.position, 5f);
    }




}



