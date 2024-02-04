using UnityEngine;
using UnityEngine.AI;
using static BaseEnum;

public class TrackingEnemyController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("追従")]
    [SerializeField, Tooltip("追従")] private Switch _move;
    [SerializeField, Tooltip("移動速度")] private float _speed;
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
        Move();
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
        Instantiate(_knockEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// 移動処理
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
    /// 追従移動
    /// </summary>
    private void TrackingMove()
    {
        _agent.SetDestination(PlayerController.Instance.transform.position);   //  プレイヤー追従
    }

    /// <summary>
    /// 方向転換
    /// </summary>
    private void Turn()
    {
        var playerPos = PlayerController.Instance.transform.position;

        //  プレイヤー方向へ回転
        var dir = Vector3.Lerp(playerPos, transform.position, _look);
        var diff = (playerPos - dir).normalized;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, diff);
    }
}
