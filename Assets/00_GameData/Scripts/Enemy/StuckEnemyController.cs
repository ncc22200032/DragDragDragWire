using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static BaseEnum;

public class StuckEnemyController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("回転")]
    [SerializeField, Tooltip("回転速度")] private float _turnSpeed;
    [Header("ダメージパラメータ")]
    [SerializeField, Tooltip("与ダメージ")] private float _damage;
    [SerializeField, Tooltip("ノックバック威力")] private float _knockBackForce;
    [Header("エフェクト")]
    [SerializeField, Tooltip("消滅")] private GameObject _knockEffect;

    // ---------------------------- Field
    //  初期化
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;
    private NavMeshAgent _agent;

    //  追従
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
    /// プレイヤーへのダメージ
    /// </summary>
    public float Damage(GameObject obj)
    {
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
        var dir = (obj.transform.position - transform.position).normalized;
        obj.GetComponent<Rigidbody2D>().AddForce(dir * _knockBackForce);
        return _damage;
    }

    /// <summary>
    /// 敵消滅
    /// </summary>
    public void Die()
    {
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// 回転処理
    /// </summary>
    private void Turn()
    {
        if (_sr.isVisible)
        {
            transform.eulerAngles += new Vector3(0, 0, _turnSpeed * Time.deltaTime);
        }
    }
}
