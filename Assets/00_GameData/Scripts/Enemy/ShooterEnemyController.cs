using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterEnemyController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("弾パラメータ")]
    [SerializeField, Tooltip("銃口")] private GameObject _muzzle;
    [SerializeField, Tooltip("弾オブジェクト")] private GameObject _bulletObj;
    [SerializeField, Tooltip("弾数")] private float _generationVolume;
    [SerializeField, Tooltip("レート")] private float _generationRate;
    [SerializeField, Tooltip("生成間隔")] private float _generationInterval;
    [Header("ダメージパラメータ")]
    [SerializeField, Tooltip("与ダメージ")] private float _damage;
    [SerializeField, Tooltip("ノックバック威力")] private float _knockBackForce;
    [Header("エフェクト")]
    [SerializeField, Tooltip("消滅")] private GameObject _knockEffect;

    // ---------------------------- Field
    //  初期化
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;

    //  追従
    private const float _look = 0.8f;




    // ---------------------------- UnityMessage
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        StartCoroutine(ShooterCycle());
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

    /// <summary>
    /// 方向の取得
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDir()
    {
        return Vector2.zero;
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if (_sr.isVisible)
        {
            Turn();
        }
        else
        {
            _rb.Sleep();
        }
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

    /// <summary>
    /// 発射周期更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShooterCycle()
    {
        while (true)
        {
            if (_sr.isVisible)
            {
                for (int i = 0; i < _generationVolume; i++)
                {
                    Shoot();
                    yield return new WaitForSeconds(_generationRate);
                }
            }
            yield return new WaitForSeconds(_generationInterval);
            yield return null;
        }
    }

    /// <summary>
    /// 射撃
    /// </summary>
    private void Shoot()
    {
        var bullet =
            Instantiate(_bulletObj, transform.position, Quaternion.identity);
        var dir = _muzzle.transform.position - transform.position;
        bullet.GetComponent<BulletController>().Dir =
            (dir, transform.rotation);
    }
}
