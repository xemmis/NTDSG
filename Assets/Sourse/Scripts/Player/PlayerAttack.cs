using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int _attackCombo;
    [SerializeField] private float _damage;
    [SerializeField] private float _comboTick = .5f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private CharacterData _playerStats;
    private Collider2D[] _colliders;
    private Animator _animator;
    private bool _isCombo;
    private Coroutine _comboCoroutine;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (Input.GetMouseButtonDown(0)) Attack();
    }

    public void ResetCombo()
    {
        _attackCombo = 1;
    }

    private IEnumerator AttackTick()
    {
        // ����������� ������� �����
        _attackCombo++;

        // ������������� �������� ����� � ���������
        _animator.SetInteger("AttackCombo", _attackCombo);

        // ���������, ��� ����� �������
        _isCombo = true;

        // ��� ��������� ����� ����� ������� �����
        yield return new WaitForSeconds(_comboTick);

        // ���� ����� �� ���� ����������, ���������� �������
        if (_isCombo)
        {
            _attackCombo = 0;
            _animator.SetInteger("AttackCombo", _attackCombo); // ���������� ��������
            _isCombo = false;
        }
    }

    // ����� ��� ������ �����
    public void Attack()
    {
        // ���� ����� �������, ������������� ���������� ��������
        if (_isCombo && _comboCoroutine != null)
        {
            StopCoroutine(_comboCoroutine);
        }

        // ��������� ����� �������� ��� ��������� �����
        _comboCoroutine = StartCoroutine(AttackTick());
    }
    private void AttackRangeCheck()
    {
        _colliders = Physics2D.OverlapCircleAll(transform.position, _attackRange);

        if (_colliders.Length > 0)
        {
            for (int i = 0; i < _colliders.Length; i++)
            {
                if (_colliders[i].TryGetComponent<ISAlive>(out ISAlive component) && _colliders[i].gameObject != this.gameObject)
                {
                    print(_colliders[i].gameObject.name);
                    component.TakeHit(_playerStats.Damage);
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

}
