using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AtackShieldCollisionCheker : MonoBehaviour
{
    #region ---------------------------- Enum
    private enum DebugGUI
    {
        ON, OFF
    }

    #endregion -------------------------
    #region ---------------------------- SerializeField
    [Header("GUIデバッグログ")]
    [SerializeField] private DebugGUI _GUI;

    #endregion -------------------------
    #region ---------------------------- Field
    [HideInInspector] public bool _isOn = false;

    #endregion -------------------------



    #region ---------------------------- UnityMessage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision != null)
        {
            collision.gameObject.GetComponent<IEnemyDamageable>().Die();
            _isOn = true;
        }
    }

    #endregion -------------------------
    #region ---------------------------- GUI
    private void OnGUI()
    {
        if (_GUI == DebugGUI.ON)
        {
            var guiPos = new Rect[30];
            for (int i = 0; i < guiPos.Length; i++)
            {
                guiPos[i] = new Rect(500, 10 + i * 20, 200, 20);
            }
            GUI.TextField(guiPos[1], "------------------------- Shield");
            GUI.TextField(guiPos[2], "isOn:" + _isOn);
        }
    }

    #endregion -------------------------
}
