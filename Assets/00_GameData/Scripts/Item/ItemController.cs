using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class ItemController : MonoBehaviour
{
    // ---------------------------- SerializeField
    [Header("デフォルト")]
    [SerializeField, Tooltip("スコア")] private float _score;
    [SerializeField, Tooltip("移動位置")] private Vector3 _afterPos;
    [SerializeField, Tooltip("スケール")] private Vector3 _afterTransform;
    [SerializeField, Tooltip("時間")] private float _duration;

    [Header("デストロイ")]
    [SerializeField, Tooltip("エフェクト")] private GameObject _destroyEffect;
    [SerializeField, Tooltip("オーディオ")] private GameObject _audioPlayer;


    // ---------------------------- Property
    /// <summary>
    /// スコア
    /// </summary>
    public float Score { get { return _score; } }


    // ---------------------------- UnityMessage

    private void Start()
    {
        Transform();
    }


    // ---------------------------- PublicMethod
    public void Destroy()
    {
        Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        Instantiate(_audioPlayer, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// 変形
    /// </summary>
    private void Transform()
    {
        transform.DOMove
            (transform.position + _afterPos, _duration)
            .SetEase(Ease.OutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);

        transform.DOScale
            (_afterTransform, _duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
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
