using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class BlockSizeSetter : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("スケール")] private SpriteRenderer _volume;


#if UNITY_EDITOR
    // ---------------------------- Field
    static double waitTime = 0;

    // ---------------------------- UnityMessage
    void OnEnable()
    {
        waitTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += EditorUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    // ---------------------------- PrivateMethod
    // 更新処理
    private void EditorUpdate()
    {
        foreach (var go in Selection.gameObjects)
        {
            // 選択中のオブジェクトのみ更新
            if (go == this.gameObject)
            {
                // １／６０秒に１回更新
                if ((EditorApplication.timeSinceStartup - waitTime) >= 0.01666f)
                {
                    if (_volume != null)
                    {
                        transform.localScale = (Vector3)_volume.size;
                    }

                    SceneView.RepaintAll(); // シーンビュー更新
                    waitTime = EditorApplication.timeSinceStartup;
                    break;
                }
            }
        }
    }
#endif
}