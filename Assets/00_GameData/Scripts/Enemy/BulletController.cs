using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("基礎パラメータ")]
    [SerializeField, Tooltip("移動速度")] private float _moveSpeed;
    [Header("ダメージパラメータ")]
    [SerializeField, Tooltip("与ダメージ")] private float _damage;
    [SerializeField, Tooltip("ノックバック威力")] private float _knockBackForce;
    [Header("エフェクト")]
    [SerializeField, Tooltip("消滅")] private GameObject _knockEffect;

    // ---------------------------- Field
    private Vector3 _addDir;

    // ---------------------------- Property
    /// <summary>
    /// 方向更新
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
    /// プレイヤーへのダメージ
    /// </summary>
    public float Damage(GameObject obj)
    {
        var dir = (obj.transform.position - transform.position).normalized;
        obj.GetComponent<Rigidbody2D>().AddForce(dir * _knockBackForce);
        return _damage;
    }

    /// <summary>
    /// 敵消滅
    /// </summary>
    public void Die()
    {
        Destroy(gameObject);
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// 移動速度
    /// </summary>
    private void Move()
    {
        transform.position += _addDir * _moveSpeed * Time.deltaTime;
    }
}
