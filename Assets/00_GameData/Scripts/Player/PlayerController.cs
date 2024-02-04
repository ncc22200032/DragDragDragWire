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
    [Header("GUIデバッグログ")]
    [SerializeField] private Switch _GUI;
    [Header("基礎パラメータ")]
    [SerializeField, Tooltip("インプット")] private PlayerInput _input;
    [SerializeField, Tooltip("HP上限")] private float _maxHp;
    [SerializeField, Tooltip("接地レイサイズ")] private float _groundRayDis;
    [Header("ラインオブジェクト")]
    [SerializeField, Tooltip("ライン")] private GameObject[] _lineObjects;

    [SerializeField, Tooltip("レイヤーマスク")] private LayerMask[] _layerMasks;
    [SerializeField, Tooltip("先端オブジェクト")] private GameObject _hookHead;
    [SerializeField, Tooltip("ロープレイサイズ")] private float _wireDis;
    [SerializeField, Tooltip("ワイヤー加速度")] private float _wireAddForce;
    [SerializeField, Tooltip("追従加速度")] private float _trackingAddForce;

    [SerializeField, Tooltip("バウンド")] private float _boundForce;
    [Header("エフェクト")]
    [SerializeField, Tooltip("地面")] private GameObject _boundEffect;
    [SerializeField, Tooltip("ダメージエフェクト")] private GameObject _damageEffect;
    [SerializeField, Tooltip("点滅回数")] private int _flashingLimit;
    [SerializeField, Tooltip("点滅時間")] private float _flashingTime;

    [Header("オーディオ")]
    [SerializeField, Tooltip("衝突")] private UnityEvent _contactClip;
    [SerializeField, Tooltip("被ダメージ")] private UnityEvent _damageClip;

    // ---------------------------- Field
    private UIManager UI;
    private SystemBase Base;

    //  インプットシステム
    private Vector3 _lookDir, _aimDir;
    private DirInput _dirInput;
    private Vector3 _lookPos = Vector3.zero;
    private InputActionPhase _ctxShot;

    //  基礎
    private float _hp;
    private bool _canTakenDamage = true;

    //  移動
    private Rigidbody2D _rb;
    private Vector2 _moveForce;
    private bool _isGrounding;

    //  フック
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
        if (UI.State == GameState.DEFAULT)   //  ポーズ中更新停止
        {
            PlayerMove();   //  移動出力
        }
    }

    private void Update()
    {
        LineActive();   //  ライン有効

        if (UI.State == GameState.DEFAULT)   //  ポーズ中更新停止
        {
            UpdateLookPos();    //  フック入力更新

            GroundDecision();   //  接地判定

            HookShot();     //  フック処理
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (obj.CompareTag(TagName.Ground))
        {
            _contactClip?.Invoke();
            var contactPoint = collision.contacts[0].point;
            //  フックショット時バウンド制御
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
    /// スティック入力
    /// </summary>
    /// <param name="ctx"></param>
    public void OnLook(InputAction.CallbackContext ctx)
    {
        _dirInput = DirInput.LOOK;
        _lookDir = (Vector3)ctx.ReadValue<Vector2>();

    }

    /// <summary>
    /// マウス位置入力
    /// </summary>
    /// <param name="ctx">コンテキスト</param>
    public void OnAim(InputAction.CallbackContext ctx)
    {
        _dirInput = DirInput.AIM;
        _aimDir = (Vector3)ctx.ReadValue<Vector2>();
    }

    /// <summary>
    /// 射出入力
    /// </summary>
    /// <param name="ctx">コンテキスト</param>
    public void OnHookShot(InputAction.CallbackContext ctx)
    {
        _ctxShot = ctx.phase;
    }

    /// <summary>
    /// 入力方法変更
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
    /// ライン両端取得
    /// </summary>
    /// <param name="type">描写タイプ</param>
    /// <returns>両端位置</returns>
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
    /// フック接触解除
    /// </summary>
    public void SetUnHookHit()
    {
        _isHookHit = false;
    }

    /// <summary>
    /// デバッグ用ワイヤー調整
    /// </summary>
    /// <param name="add"></param>
    public void SetWireDis(float add)
    {
        _wireDis += add;
    }

    /// <summary>
    /// リスポーン
    /// </summary>
    /// <param name="pos"></param>
    public void Respawn(Vector3 pos)
    {
        transform.position = pos;
    }



    // ---------------------------- PrivateMethod
    /// <summary>
    /// 方向入力更新
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
    /// 接地判定
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
        //  コンベア加速追加
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
    /// 移動更新処理
    /// </summary>
    private void PlayerMove()
    {
        //  フック接触時加速追加
        if (_isHookHit)
        {
            _moveForce = _hookShotForce;
        }
        else
        {
            _moveForce = Vector2.zero;
        }

        _rb.AddForce(_moveForce);   //  加速出力
    }

    /// <summary>
    /// ライン有効
    /// </summary>
    private void LineActive()
    {
        foreach (var item in _lineObjects)
        {
            item.SetActive((UI.State == GameState.DEFAULT));
        }
    }

    /// <summary>
    /// 被ダメージ
    /// </summary>
    /// <param name="damage"></param>
    private void DamageTaken(float damage)
    {
        if (_canTakenDamage)
        {
            _canTakenDamage = false;

            //  ダメージ処理
            _hp -= damage;

            //  効果音
            _damageClip?.Invoke();

            //  エフェクト
            Instantiate(_damageEffect, transform.position, Quaternion.identity)
                .transform.SetParent(transform);

            //  点滅
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
    /// フックショット
    /// </summary>
    #region ------ HookShot

    /// <summary>
    /// 射出処理
    /// </summary>
    private void HookShot()
    {
        if (_ctxShot == InputActionPhase.Performed) //  射出入力
        {
            HookActive(GetHitColliderType());
        }
        else
        {
            HookInactive(GetHitColliderType());
        }

        _hookHead.transform.position = _headPos[(int)LineType.ROPE];    //  接触位置移動
    }

    /// <summary>
    /// 起動時処理
    /// </summary>
    /// <param name="type"></param>
    private void HookActive(HitCollider type)
    {
        var playerPos = transform.position;

        //  接触位置代入
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

        //  加速代入
        Vector2 force = (_hitPos - (Vector2)playerPos).normalized * _wireAddForce;

        //  パラメータ代入
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
        /// パラメータ更新
        /// </summary>
        /// <param name="rope">ロープ先端位置</param>
        /// <param name="indicator">インジケーター先端位置</param>
        /// <param name="force">ワイヤーによる加速度</param>
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
    /// 非起動時処理
    /// </summary>
    /// <param name="type">接触している</param>
    private void HookInactive(HitCollider type)
    {
        var playerPos = transform.position;
        _isHookHit = false;

        //  先端位置代入
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

        //  制限時非表示
        if (_ctxShot == InputActionPhase.Performed)
        {
            _headPos[(int)LineType.INDICATOR] = playerPos;
        }
    }

    /// <summary>
    /// 接触レイ指定
    /// </summary>
    /// <returns>接触したレイに対応した列挙型変数(RayType)</returns>
    private HitCollider GetHitColliderType()
    {
        var playerPos = transform.position;

        //  レイ描写
        var rayDir = _lookPos - playerPos;
        var ray = new Ray2D(playerPos, rayDir.normalized);
        Debug.DrawRay
            (playerPos
            , rayDir.normalized * _wireDis
            , Color.green);

        //  レイの長さを計測・判定
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

        //  返り値
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