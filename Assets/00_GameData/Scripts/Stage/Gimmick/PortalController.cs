using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{

    // ---------------------------- Enum

    // ---------------------------- SerializeField
    [SerializeField, Tooltip("�ړI���W")] private Transform _warpPos;
    [SerializeField, Tooltip("�x������")] private float _duration;

    // ---------------------------- Field
    static bool _isWarping = false;
    static float _time = 0;



    // ---------------------------- UnityMessage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_isWarping)
            {
                collision.transform.position = _warpPos.position;
                StartCoroutine(WarpingTimer());
            }
        }
    }



    // ---------------------------- PrivateMethod
    /// <summary>
    /// ���[�v
    /// </summary>
    /// <returns></returns>
    private IEnumerator WarpingTimer()
    {
        _isWarping = true;
        while (_time < _duration)
        {
            _time += Time.deltaTime;

            yield return null;
        }
        _time = 0;
        _isWarping = false;
        yield break;
    }
}
