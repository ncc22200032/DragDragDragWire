using DG.Tweening;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField, Tooltip("大きさ")] private float _scale;
    [SerializeField, Tooltip("ループ時間")] private float _duration;

    private void Start()
    {
        transform.DOScale
            (_scale, _duration)
            .SetEase(Ease.OutBack)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
