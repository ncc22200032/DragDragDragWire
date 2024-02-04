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
    /// �|�[�Y
    /// </summary>
    public void OnPause()
    {
        if (!UI.IsMoveMenu)
        {
            UI.OnPause();
        }
    }

    /// <summary>
    /// �o�b�N
    /// </summary>
    public void OnBack()
    {
        if (!UI.IsMoveMenu)
        {
            UI.OnBack();
        }
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void ApplicationQuit()
    {
        UI.ApplicationQuit();
    }
}
