using DG.Tweening;
using UnityEngine;

public class StateController : MonoBehaviour
{
    private UIManager UI;

    private void Start()
    {
        UI = UIManager.Instance;
    }

    // ---------------------------- PublicMethod
    /// <summary>
    /// ポーズ
    /// </summary>
    public void OnPause()
    {
        if (!UI.IsMoveMenu)
        {
            UI.OnPause();
        }
    }

    /// <summary>
    /// バック
    /// </summary>
    public void OnBack()
    {
        if (!UI.IsMoveMenu)
        {
            UI.OnBack();
        }
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void ApplicationQuit()
    {
        UI.ApplicationQuit();
    }
}
