using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region ---------------------------- Enum
    public enum Scene
    {
        TITLE,
        STAGE01, STAGE02, STAGE03,
    }

    #endregion -------------------------
    #region ---------------------------- SerializeField
    [SerializeField,Tooltip("�ύX��")]private Scene _transitionTarget;

    #endregion -------------------------



    #region ---------------------------- PublicMethod
    /// <summary>
    /// �V�[���ύX
    /// </summary>
    /// <param name="scene">�V�[��</param>
    public void SceneChange()
    {
        SceneManager.LoadScene((int)_transitionTarget);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// �ēǂݍ���
    /// </summary>
    public void SceneReLoad()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }

    #endregion -------------------------
}

