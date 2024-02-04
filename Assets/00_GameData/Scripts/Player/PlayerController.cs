using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static BaseEnum;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    // ---------------------------- Enum
    public enum LineType
    {
        ROPE, INDICATOR
    }

    private enum DirInput
    {
        LOOK, AIM
    }
    private enum LinePos
    {
        ORIGIN, HEAD
    }
    private enum HitCollider
    {
        HIT, UNHIT, TRACKINGHIT,
        NULL
    }


    // ---------------------------- SerializeField
    [Header("GUI�f�o�b�O���O")]
    [SerializeField] private Switch _GUI;
    [Header("��b�p�����[�^")]
    [SerializeField, Tooltip("�C���v�b�g")] private PlayerInput _input;
    [SerializeField, Tooltip("HP���")] private float _maxHp;
    [SerializeField, Tooltip("�ڒn���C�T�C�Y")] private float _groundRayDis;
    [Header("���C���I�u�W�F�N�g")]
    [SerializeField, Tooltip("���C��")] private GameObject[] _lineObjects;

    [SerializeField, Tooltip("���C���[�}�X�N")] private LayerMask[] _layerMasks;
    [SerializeField, Tooltip("��[�I�u�W�F�N�g")] private GameObject _hookHead;
    [SerializeField, Tooltip("���[�v���C�T�C�Y")] private float _wireDis;
    [SerializeField, Tooltip("���C���[�����x")] private float _wireAddForce;
    [SerializeField, Tooltip("�Ǐ]�����x")] private float _trackingAddForce;

    [SerializeField, Tooltip("�o�E���h")] private float _boundForce;
    [Header("�G�t�F�N�g")]
    [SerializeField, Tooltip("�n��")] private GameObject _boundEffect;
    [SerializeField, Tooltip("�_���[�W�G�t�F�N�g")] private GameObject _damageEffect;
    [SerializeField, Tooltip("�_�ŉ�")] private int _flashingLimit;
    [SerializeField, Tooltip("�_�Ŏ���")] private float _flashingTime;

    [Header("�I�[�f�B�I")]
    [SerializeField, Tooltip("�Փ�")] private UnityEvent _contactClip;
    [SerializeField, Tooltip("��_���[�W")] private UnityEvent _damageClip;

    // ---------------------------- Field
    private UIManager UI;
    private SystemBase Base;

    //  �C���v�b�g�V�X�e��
    private Vector3 _lookDir, _aimDir;
    private DirInput _dirInput;
    private Vector3 _lookPos = Vector3.zero;
    private InputActionPhase _ctxShot;

    //  ��b
    private float _hp;
    private bool _canTakenDamage = true;

    //  �ړ�
    private Rigidbody2D _rb;
    private Vector2 _moveForce;
    private bool _isGrounding;

    //  �t�b�N
    private HitCollider _activeHitPos;
    private RaycastHit2D[] _hit = new RaycastHit2D[Enum.GetValues(typeof(HitCollider)).Length - 1];
    private Vector2 _hitPos;
    private Vector2 _hookShotForce;
    private bool _isHookHit;
    private Vector3[] _headPos = new Vector3[Enum.GetValues(typeof(LineType)).Length];
    private GameObject _targetObj;


    // ---------------------------- UnityMessage
    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _hp = _maxHp;
    }

    private void Start()
    {
        UI = UIManager.Instance;
        Base = SystemBase.Instance;
        UI.HPSet(_maxHp, _hp);
    }

    private void FixedUpdate()
    {
        if (UI.State == GameState.DEFAULT)   //  �|�[�Y���X�V��~
        {
            PlayerMove();   //  �ړ��o��
        }
    }

    private void Update()
    {
        LineActive();   //  ���C���L��

        if (UI.State == GameState.DEFAULT)   //  �|�[�Y���X�V��~
        {
            UpdateLookPos();    //  �t�b�N���͍X�V

            GroundDecision();   //  �ڒn����

            HookShot();     //  �t�b�N����
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag(TagName.Ground))
        {
            _contactClip?.Invoke();
            var contactPoint = collision.contacts[0].point;
            //  �t�b�N�V���b�g���o�E���h����
            if (_isHookHit)
            {
                var dir = ((Vector2)transform.position - contactPoint).normalized;
                _rb.AddForce(dir * _boundForce);
            }
            Instantiate(_boundEffect, contactPoint, Quaternion.identity);
        }
        if (obj.CompareTag(TagName.Enemy))
        {
            var IEnemy = obj.GetComponent<IEnemyDamageable>();
            DamageTaken(IEnemy.Damage(gameObject));
            UI.HPSet(_maxHp, _hp);
        }
        if (obj.CompareTag(TagName.Respawn))
        {
            DamageTaken(1);
            UI.HPSet(_maxHp, _hp);
            Respawn(RespawnManager.Instance.RespawnPos.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag(TagName.Goal))
        {
            UI.GameClear();
        }
        if (obj.CompareTag(TagName.Item))
        {
            var item = obj.GetComponent<ItemController>();
            UI.CoinScore = item.Score;
            item.Destroy();
        }
        if (obj.CompareTag(TagName.Enemy))
        {
            var IEnemy = obj.GetComponent<IEnemyDamageable>();
            DamageTaken(IEnemy.Damage(gameObject));
            IEnemy.Die();
            UI.HPSet(_maxHp, _hp);
        }
    }



    // ---------------------------- OnGUI
    private void OnGUI()
    {
        if (_GUI == Switch.ON && SystemBase.Instance.DebugSwitch())
        {
            var guiPos = new Rect[30];
            for (int i = 0; i < guiPos.Length; i++)
            {
                guiPos[i] = new Rect(10, 1080 - i * 30, 300, 30);
            }
            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 25;


            GUI.TextField(guiPos[1], $"control: {0}", style);
        }
    }



    // ---------------------------- PublicMethod
    #region ------ InputSystem
    /// <summary>
    /// �X�e�B�b�N����
    /// </summary>
    /// <param name="ctx"></param>
    public void OnLook(InputAction.CallbackContext ctx)
    {
        _dirInput = DirInput.LOOK;
        _lookDir = (Vector3)ctx.ReadValue<Vector2>();

    }

    /// <summary>
    /// �}�E�X�ʒu����
    /// </summary>
    /// <param name="ctx">�R���e�L�X�g</param>
    public void OnAim(InputAction.CallbackContext ctx)
    {
        _dirInput = DirInput.AIM;
        _aimDir = (Vector3)ctx.ReadValue<Vector2>();
    }

    /// <summary>
    /// �ˏo����
    /// </summary>
    /// <param name="ctx">�R���e�L�X�g</param>
    public void OnHookShot(InputAction.CallbackContext ctx)
    {
        _ctxShot = ctx.phase;
    }

    /// <summary>
    /// ���͕��@�ύX
    /// </summary>
    public void OnControlsChanged()
    {
        var currentString = _input.currentControlScheme;
        if (currentString == Scheme.KeyboardMouse.ToString())
        {
            Base?.ControlChange(Scheme.KeyboardMouse, null);
        }
        else if (currentString == Scheme.Gamepad.ToString())
        {
            Base?.ControlChange(Scheme.Gamepad, UI.ControlChange);
        }
    }

    #endregion

    /// <summary>
    /// ���C�����[�擾
    /// </summary>
    /// <param name="type">�`�ʃ^�C�v</param>
    /// <returns>���[�ʒu</returns>
    public Vector3[] GetEdge(LineType type)
    {
        var edge = new Vector3[Enum.GetNames(typeof(LinePos)).Length];
        edge[(int)LinePos.ORIGIN] = transform.position;
        if (UI.State == GameState.DEFAULT && !Base.IsFade)
        {
            switch (type)
            {
                case LineType.ROPE:
                    edge[(int)LinePos.HEAD] = _headPos[(int)LineType.ROPE];
                    break;

                case LineType.INDICATOR:
                    edge[(int)LinePos.HEAD] = _headPos[(int)LineType.INDICATOR];
                    break;
            }
        }
        else
        {
            edge[(int)LinePos.HEAD] = transform.position;
        }

        return edge;
    }

    /// <summary>
    /// �t�b�N�ڐG����
    /// </summary>
    public void SetUnHookHit()
    {
        _isHookHit = false;
    }

    /// <summary>
    /// �f�o�b�O�p���C���[����
    /// </summary>
    /// <param name="add"></param>
    public void SetWireDis(float add)
    {
        _wireDis += add;
    }

    /// <summary>
    /// ���X�|�[��
    /// </summary>
    /// <param name="pos"></param>
    public void Respawn(Vector3 pos)
    {
        transform.position = pos;
    }



    // ---------------------------- PrivateMethod
    /// <summary>
    /// �������͍X�V
    /// </summary>
    private void UpdateLookPos()
    {
        switch (_dirInput)
        {
            case DirInput.LOOK:
                _lookPos = transform.position + _lookDir;
                break;
            case DirInput.AIM:
                if (Camera.main != null)
                {
                    var camera = Camera.main.ScreenToWorldPoint(_aimDir);
                    camera.z = 0;
                    _lookPos = camera;
                }
                break;
        }
    }

    /// <summary>
    /// �ڒn����
    /// </summary>
    private void GroundDecision()
    {
        var ray = new Ray2D(transform.position, new Vector2(0, -1));
        var hit = Physics2D.Raycast
            (ray.origin
            , ray.direction
            , _groundRayDis
            , _layerMasks[(int)HitCollider.HIT]);

        Debug.DrawRay(ray.origin, ray.direction * _groundRayDis, Color.green);

        _isGrounding = hit.collider;
        //  �R���x�A�����ǉ�
        if (_isGrounding)
        {
            var obj = hit.collider.gameObject;
            if (obj.CompareTag(TagName.Belt))
            {
                var add = obj.GetComponent<ConveyorController>().GetSpeed();
                _rb.AddForce(new Vector2(add, 0));
            }
        }
    }

    /// <summary>
    /// �ړ��X�V����
    /// </summary>
    private void PlayerMove()
    {
        //  �t�b�N�ڐG�������ǉ�
        if (_isHookHit)
        {
            _moveForce = _hookShotForce;
        }
        else
        {
            _moveForce = Vector2.zero;
        }

        _rb.AddForce(_moveForce);   //  �����o��
    }

    /// <summary>
    /// ���C���L��
    /// </summary>
    private void LineActive()
    {
        foreach (var item in _lineObjects)
        {
            item.SetActive((UI.State == GameState.DEFAULT));
        }
    }

    /// <summary>
    /// ��_���[�W
    /// </summary>
    /// <param name="damage"></param>
    private void DamageTaken(float damage)
    {
        if (_canTakenDamage)
        {
            _canTakenDamage = false;

            //  �_���[�W����
            _hp -= damage;

            //  ���ʉ�
            _damageClip?.Invoke();

            //  �G�t�F�N�g
            Instantiate(_damageEffect, transform.position, Quaternion.identity)
                .transform.SetParent(transform);

            //  �_��
            var fromColor = GetComponent<SpriteRenderer>().color;
            DOVirtual.Color(
                Color.black, fromColor,
                _flashingTime,
                (result) =>
                {
                    GetComponent<SpriteRenderer>().color = result;
                })
                .SetEase(Ease.Linear)
                .SetLoops(_flashingLimit, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    _canTakenDamage = true;
                })
                .SetLink(gameObject);
        }
        else
        {
            _contactClip?.Invoke();
        }

    }

    /// <summary>
    /// �t�b�N�V���b�g
    /// </summary>
    #region ------ HookShot

    /// <summary>
    /// �ˏo����
    /// </summary>
    private void HookShot()
    {
        if (_ctxShot == InputActionPhase.Performed) //  �ˏo����
        {
            HookActive(GetHitColliderType());
        }
        else
        {
            HookInactive(GetHitColliderType());
        }

        _hookHead.transform.position = _headPos[(int)LineType.ROPE];    //  �ڐG�ʒu�ړ�
    }

    /// <summary>
    /// �N��������
    /// </summary>
    /// <param name="type"></param>
    private void HookActive(HitCollider type)
    {
        var playerPos = transform.position;

        //  �ڐG�ʒu���
        if (!_isHookHit)
        {
            _activeHitPos = type;
            if (type != HitCollider.NULL
                && type != HitCollider.TRACKINGHIT)
                _hitPos = _hit[(int)_activeHitPos].point;

            switch (type)
            {
                case HitCollider.HIT:
                    _isHookHit = true;
                    break;
                case HitCollider.TRACKINGHIT:
                    _targetObj = _hit[(int)_activeHitPos].collider.gameObject;
                    _isHookHit = true;
                    break;

                case HitCollider.UNHIT:
                case HitCollider.NULL:
                    _isHookHit = false;
                    break;
            }
        }

        //  �������
        Vector2 force = (_hitPos - (Vector2)playerPos).normalized * _wireAddForce;

        //  �p�����[�^���
        switch (_activeHitPos)
        {
            case HitCollider.HIT:
                UpdateParam(_hitPos, playerPos, force);
                break;

            case HitCollider.UNHIT:
                UpdateParam(playerPos, _hitPos, Vector2.zero);
                break;

            case HitCollider.TRACKINGHIT:
                if (_targetObj != null)
                {
                    _hitPos = _targetObj.transform.position;
                    force *= _trackingAddForce;
                }
                else
                {
                    _hitPos = transform.position;
                    force = Vector2.zero;
                }
                UpdateParam(_hitPos, playerPos, force);
                break;

            case HitCollider.NULL:
                var indicator =
                    playerPos - (playerPos - _lookPos).normalized * _wireDis;
                UpdateParam(playerPos, indicator, Vector2.zero);
                break;
        }

        /// <summary>
        /// �p�����[�^�X�V
        /// </summary>
        /// <param name="rope">���[�v��[�ʒu</param>
        /// <param name="indicator">�C���W�P�[�^�[��[�ʒu</param>
        /// <param name="force">���C���[�ɂ������x</param>
        void UpdateParam(
            Vector3 rope,
            Vector3 indicator,
            Vector2 force)
        {
            _headPos[(int)LineType.ROPE] = rope;
            _headPos[(int)LineType.INDICATOR] = indicator;
            _hookShotForce = force;
        }
    }



    /// <summary>
    /// ��N��������
    /// </summary>
    /// <param name="type">�ڐG���Ă���</param>
    private void HookInactive(HitCollider type)
    {
        var playerPos = transform.position;
        _isHookHit = false;

        //  ��[�ʒu���
        _headPos[(int)LineType.ROPE] = playerPos;
        switch (type)
        {
            case HitCollider.HIT:
            case HitCollider.UNHIT:
                _headPos[(int)LineType.INDICATOR] = _hit[(int)type].point;
                break;

            case HitCollider.TRACKINGHIT:
                _headPos[(int)LineType.INDICATOR] = _hit[(int)type].transform.position;
                break;

            case HitCollider.NULL:
                _headPos[(int)LineType.INDICATOR] =
                    playerPos - (playerPos - _lookPos).normalized * _wireDis;
                break;
        }

        //  ��������\��
        if (_ctxShot == InputActionPhase.Performed)
        {
            _headPos[(int)LineType.INDICATOR] = playerPos;
        }
    }

    /// <summary>
    /// �ڐG���C�w��
    /// </summary>
    /// <returns>�ڐG�������C�ɑΉ������񋓌^�ϐ�(RayType)</returns>
    private HitCollider GetHitColliderType()
    {
        var playerPos = transform.position;

        //  ���C�`��
        var rayDir = _lookPos - playerPos;
        var ray = new Ray2D(playerPos, rayDir.normalized);
        Debug.DrawRay
            (playerPos
            , rayDir.normalized * _wireDis
            , Color.green);

        //  ���C�̒������v���E����
        float[] dis = new float[_hit.Length];
        var count = 0;
        for (int i = 0; i < _hit.Length; i++)
        {
            _hit[i] = Physics2D.Raycast(ray.origin, ray.direction, _wireDis, _layerMasks[i]);
            if (_hit[i].collider)
            {
                dis[i] = (_hit[i].point - (Vector2)playerPos).sqrMagnitude;
                count++;
            }
            else
            {
                dis[i] = _wireDis * _wireDis;
            }
        }

        //  �Ԃ�l
        if (count != 0)
        {
            return (HitCollider)Array.IndexOf(dis, dis.Min());
        }
        else
        {
            return HitCollider.NULL;
        }
    }

    #endregion
}