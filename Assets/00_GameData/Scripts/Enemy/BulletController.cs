using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("��b�p�����[�^")]
    [SerializeField, Tooltip("�ړ����x")] private float _moveSpeed;
    [Header("�_���[�W�p�����[�^")]
    [SerializeField, Tooltip("�^�_���[�W")] private float _damage;
    [SerializeField, Tooltip("�m�b�N�o�b�N�З�")] private float _knockBackForce;
    [Header("�G�t�F�N�g")]
    [SerializeField, Tooltip("����")] private GameObject _knockEffect;

    // ---------------------------- Field
    private Vector3 _addDir;

    // ---------------------------- Property
    /// <summary>
    /// �����X�V
    /// </summary>
    public (Vector3 Dir, Quaternion Rotation) Dir
    {
        set
        {
            _addDir = value.Dir;
            transform.rotation = value.Rotation;
        }
    }


    // ---------------------------- UnityMessage
    private void Update()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag(TagName.Ground))
        {
            Die();
        }
        if (obj.CompareTag(TagName.Belt))
        {
            Die();
        }
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
        Destroy(gameObject);
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// �ړ����x
    /// </summary>
    private void Move()
    {
        transform.position += _addDir * _moveSpeed * Time.deltaTime;
    }
}
