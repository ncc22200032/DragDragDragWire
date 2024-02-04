using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.PlayerLoop;

public class ShieldController : MonoBehaviour
{
    #region ---------------------------- SerializeField
    [SerializeField, Tooltip("体当たりシールド")] private GameObject _attackShield;
    [SerializeField, Tooltip("シールドオブジェクト")] private GameObject[] _shield;

    #endregion -------------------------
    #region ---------------------------- Field
    private const float _look = 0.8f;

    #endregion -------------------------



    #region ---------------------------- UnityMessage
    private void Update()
    {
        Turning();
        ShieldIncrease();
    }

    #endregion -------------------------



    #region ---------------------------- PrivateMethod
    /// <summary>
    /// マウス位置へ方向転換
    /// </summary>
    private void Turning()
    {
        var look = Vector3.zero;
        var dir = Vector3.Lerp(look, transform.position, _look);
        var diff = (look - dir).normalized;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, diff);
    }

    /// <summary>
    /// シールド増加処理
    /// </summary>
    private void ShieldIncrease()
    {
        if (_attackShield.GetComponent<AtackShieldCollisionCheker>()._isOn)
        {
            foreach (GameObject shield in _shield)
            {
                if (!shield.activeSelf)
                {
                    shield.SetActive(true);
                    _attackShield.GetComponent<AtackShieldCollisionCheker>()._isOn = false;
                    break;
                }
            }
            _attackShield.GetComponent<AtackShieldCollisionCheker>()._isOn = false;
        }
    }

    #endregion -------------------------
}
