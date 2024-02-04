using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BaseEnum;

public class StuckEnemyController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("��]")]
    [SerializeField, Tooltip("��]���x")] private float _turnSpeed;
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
        Turn();
    }




    // ---------------------------- PublicMethod
    /// <summary>
    /// �v���C���[�ւ̃_���[�W
    /// </summary>
    public float Damage(GameObject obj)
    {
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
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
    /// ��]����
    /// </summary>
    private void Turn()
    {
        if (_sr.isVisible)
        {
            transform.eulerAngles += new Vector3(0, 0, _turnSpeed * Time.deltaTime);
        }
    }
}
