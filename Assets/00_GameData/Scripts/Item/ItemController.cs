using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class ItemController : MonoBehaviour
{
    // ---------------------------- SerializeField
    [Header("�f�t�H���g")]
    [SerializeField, Tooltip("�X�R�A")] private float _score;
    [SerializeField, Tooltip("�ړ��ʒu")] private Vector3 _afterPos;
    [SerializeField, Tooltip("�X�P�[��")] private Vector3 _afterTransform;
    [SerializeField, Tooltip("����")] private float _duration;

    [Header("�f�X�g���C")]
    [SerializeField, Tooltip("�G�t�F�N�g")] private GameObject _destroyEffect;
    [SerializeField, Tooltip("�I�[�f�B�I")] private GameObject _audioPlayer;


    // ---------------------------- Property
    /// <summary>
    /// �X�R�A
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
    /// �ό`
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
    /// UniTask�L�����Z������
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    private async UniTask Canceled(UniTask task)
    {
        var canceled = await task.SuppressCancellationThrow();
        if (canceled) { return; }
    }
}
