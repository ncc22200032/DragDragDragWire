using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class BlockSizeSetter : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField, Tooltip("�X�P�[��")] private SpriteRenderer _volume;


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
    // �X�V����
    private void EditorUpdate()
    {
        foreach (var go in Selection.gameObjects)
        {
            // �I�𒆂̃I�u�W�F�N�g�̂ݍX�V
            if (go == this.gameObject)
            {
                // �P�^�U�O�b�ɂP��X�V
                if ((EditorApplication.timeSinceStartup - waitTime) >= 0.01666f)
                {
                    if (_volume != null)
                    {
                        transform.localScale = (Vector3)_volume.size;
                    }

                    SceneView.RepaintAll(); // �V�[���r���[�X�V
                    waitTime = EditorApplication.timeSinceStartup;
                    break;
                }
            }
        }
    }
#endif
}