using UnityEngine;
using UnityEngine.AI;
using static BaseEnum;

public class TrackingEnemyController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("�Ǐ]")]
    [SerializeField, Tooltip("�Ǐ]")] private Switch _move;
    [SerializeField, Tooltip("�ړ����x")] private float _speed;
    [Header("�_���[�W�p�����[�^")]
    [SerializeField, Tooltip("�^�_���[�W")] private float _damage;
    [SerializeField, Tooltip("�m�b�N�o�b�N�З�")] private float _knockBackForce;
    [Header("�G�t�F�N�g")]
    [SerializeField, Tooltip("����")] private GameObject _knockEffect;

    // ---------------------------- Field
    //  ������
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    private NavMeshAgent _agent;

    //  �Ǐ]
    private const float _look = 0.8f;




    // ---------------------------- UnityMessage
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Move();
    }





    // ---------------------------- PublicMethod
    /// <summary>
    /// �v���C���[�ւ̃_���[�W
    /// </summary>
    public float Damage(GameObject obj)
    {
        var dir = (obj.transform.position - transform.position).normalized;
        obj.GetComponent<Rigidbody2D>().AddForce(dir * _knockBackForce);
        return _damage;
    }

    /// <summary>
    /// �G����
    /// </summary>
    public void Die()
    {
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// �ړ�����
    /// </summary>
    private void Move()
    {
        if (_sr.isVisible)
        {
            if (_move == Switch.ON)
            {
                TrackingMove();
            }
            Turn();
        }
        else
        {
            _rb.Sleep();
        }
    }

    /// <summary>
    /// �Ǐ]�ړ�
    /// </summary>
    private void TrackingMove()
    {
        _agent.SetDestination(PlayerController.Instance.transform.position);   //  �v���C���[�Ǐ]
    }

    /// <summary>
    /// �����]��
    /// </summary>
    private void Turn()
    {
        var playerPos = PlayerController.Instance.transform.position;

        //  �v���C���[�����։�]
        var dir = Vector3.Lerp(playerPos, transform.position, _look);
        var diff = (playerPos - dir).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, diff);
    }
}
