using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShooterEnemyController : MonoBehaviour, IEnemyDamageable
{
    // ---------------------------- SerializeField
    [Header("�e�p�����[�^")]
    [SerializeField, Tooltip("�e��")] private GameObject _muzzle;
    [SerializeField, Tooltip("�e�I�u�W�F�N�g")] private GameObject _bulletObj;
    [SerializeField, Tooltip("�e��")] private float _generationVolume;
    [SerializeField, Tooltip("���[�g")] private float _generationRate;
    [SerializeField, Tooltip("�����Ԋu")] private float _generationInterval;
    [Header("�_���[�W�p�����[�^")]
    [SerializeField, Tooltip("�^�_���[�W")] private float _damage;
    [SerializeField, Tooltip("�m�b�N�o�b�N�З�")] private float _knockBackForce;
    [Header("�G�t�F�N�g")]
    [SerializeField, Tooltip("����")] private GameObject _knockEffect;

    // ---------------------------- Field
    //  ������
    private SpriteRenderer _sr;
    private Rigidbody2D _rb;

    //  �Ǐ]
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

    /// <summary>
    /// �����̎擾
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDir()
    {
        return Vector2.zero;
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// �ړ�����
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

    /// <summary>
    /// ���ˎ����X�V
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
    /// �ˌ�
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
