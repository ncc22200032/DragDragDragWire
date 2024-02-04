using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class GizmosIconDrawer
{
    #region ---------------------------- Field
    private const string MENU_PATH = "ExTools/GizmosIconDrawer";

    #endregion -------------------------



    #region ---------------------------- PrivateMethod
    /// <summary>
    /// �؊����ۑ�
    /// </summary>
    [MenuItem(MENU_PATH, priority = 41)]
    private static void MenuToggle()
    {
        EditorPrefs.SetBool(MENU_PATH, !EditorPrefs.GetBool(MENU_PATH, false));
    }

    /// <summary>
    /// �؊���
    /// </summary>
    /// <returns></returns>
    [MenuItem(MENU_PATH, true, priority = 41)]
    private static bool MenuToggleValidate()
    {
        Menu.SetChecked(MENU_PATH, EditorPrefs.GetBool(MENU_PATH, false));
        return true;
    }

    /// <summary>
    /// ����
    /// </summary>
    private static bool IsValid()
    {
        return EditorPrefs.GetBool(MENU_PATH, false);
    }

    /// <summary>
    /// Transform�A�C�R���\��
    /// </summary>
    /// <param name="scr"></param>
    /// <param name="gizmoType"></param>
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    private static void DrawGizmo(Transform scr, GizmoType gizmoType)
    {
        if (IsValid())
        {
            Gizmos.DrawIcon(scr.transform.position, "TransformIcon.png", true);
        }
    }

    #endregion -------------------------
}
