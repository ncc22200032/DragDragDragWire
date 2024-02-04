using DG.Tweening;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField, Tooltip("�傫��")] private float _scale;
    [SerializeField, Tooltip("���[�v����")] private float _duration;

    private void Start()
    {
        transform.DOScale
            (_scale, _duration)
            .SetEase(Ease.OutBack)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }
}
