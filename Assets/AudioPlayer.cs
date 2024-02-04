using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField, Tooltip("音")] private UnityEvent _destroyClip;
    [SerializeField, Tooltip("ソース")] private AudioSource _destroySource;

    private async void Start()
    {
        _destroyClip?.Invoke();
        await Canceled(UniTask.WaitUntil(() => !_destroySource.isPlaying));
        Destroy(gameObject);
    }

    /// <summary>
    /// UniTaskキャンセル処理
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    private async UniTask Canceled(UniTask task)
    {
        var canceled = await task.SuppressCancellationThrow();
        if (canceled) { return; }
    }
}
