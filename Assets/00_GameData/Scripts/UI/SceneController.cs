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
    [SerializeField,Tooltip("変更先")]private Scene _transitionTarget;

    #endregion -------------------------



    #region ---------------------------- PublicMethod
    /// <summary>
    /// シーン変更
    /// </summary>
    /// <param name="scene">シーン</param>
    public void SceneChange()
    {
        SceneManager.LoadScene((int)_transitionTarget);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// 再読み込み
    /// </summary>
    public void SceneReLoad()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1.0f;
    }

    #endregion -------------------------
}

