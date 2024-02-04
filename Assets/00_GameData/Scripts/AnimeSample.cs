using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//  �g�p���Ɏ����ǉ�����܂���
//  ���� using ���Ȃ���UniTask DOTween�͎g�p�ł��܂���
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class AnimeSample : MonoBehaviour
{
    // ---------------------------- SerializeField
    [SerializeField] private float _waitTime;

    [SerializeField] private float _floatPos;
    [SerializeField] private Vector3 _vec3Pos;
    [SerializeField] private float _duration;

    [SerializeField] private float _endValue;

    [SerializeField] private RectTransform _rect;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Image _image;


    // ---------------------------- UnityMessage
    private void Start()
    {
        TaskSample();
    }




    // ---------------------------- PrivateMethod
    /// <summary>
    /// �T���v��
    /// </summary>
    private async void TaskSample()
    {
        //  async�@�����ă��\�b�h������
        //  await�@�ł��̏������I���܂Ŏ��̏�����҂�
        //  UniTask�̓Q�[�����I�����铙�̋��͂ȃC�x���g���Ȃ�����
        //  �����������܂��A�Ȃ̂ŃL�����Z�����������ނ��Ƃň��S�Ɏg�p���܂�


        // ------ ���Ԃ�҂@(TimeSpan.FromSeconds(����), timeScale == 0 �ł��������ǂ���))
        await Canceled(UniTask.Delay(TimeSpan.FromSeconds(_waitTime), true));


        // ------ �ړ�
        await Canceled(ObjMove(gameObject, _vec3Pos, Ease.Linear));


        // ------ UI�ړ�
        await Canceled(UIMove(_rect, _vec3Pos, Ease.Linear));


        // ------ �t�F�[�h
        await Canceled(Fade(_sprite, _endValue, Ease.Linear));
        await Canceled(Fade(_image, _endValue, Ease.Linear));


        // ------ ���X�g�ɓ��ꂽ������S�čs���܂ő҂�
        var tasks = new List<UniTask>()
        {
            //  �錾���Ƀ��X�g�ɗv�f������
            Canceled(ObjMoveX(gameObject,_floatPos,Ease.Linear)),
            Canceled(ObjMoveX(gameObject,_floatPos,Ease.Linear))
        };
        //  �錾�������Ƃɒǉ�����
        tasks.Add(Canceled(UIMoveX(_rect, _floatPos, Ease.Linear)));
        tasks.Add(Canceled(UIMoveY(_rect, _floatPos, Ease.Linear)));
        //  �S�Ă̏������s��
        await Canceled(UniTask.WhenAll(tasks));
    }

    #region ------ ObjMove
    //  Transform�I�u�W�F�N�g�̈ړ�

    /// <summary>
    /// �ړ�
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="pos"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask ObjMove
        (GameObject obj
        , Vector3 pos
        , Ease ease)
    {
        await obj.transform.DOMove
            (pos, _duration)    //  �ڕW�l�A���o����
            .SetEase(ease)      //  ������
            .SetUpdate(true)    //  timeScale == 0 �ł��������ǂ���
            .SetLink(obj);      //  ���S��DOTween���g�p���邽�߂̐ݒ�
    }

    /// <summary>
    /// �ړ�x
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="pos"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask ObjMoveX
        (GameObject obj
        , float pos
        , Ease ease)
    {
        await obj.transform.DOMoveX
            (pos, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(obj);
    }

    /// <summary>
    /// �ړ�y
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="pos"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask ObjMoveY
        (GameObject obj
        , float pos
        , Ease ease)
    {
        await obj.transform.DOMoveY
            (pos, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(obj);
    }

    #endregion

    #region ------ UIMove
    //  RectTransform�I�u�W�F�N�g�̈ړ�

    /// <summary>
    /// UI�ړ�
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="pos"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask UIMove
        (RectTransform rect
        , Vector3 pos
        , Ease ease)
    {
        await rect.DOAnchorPos
            (pos, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(rect.gameObject);
    }

    /// <summary>
    /// UI�ړ�x
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="pos"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask UIMoveX
        (RectTransform rect
        , float pos
        , Ease ease)
    {
        await rect.DOAnchorPosX
            (pos, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(rect.gameObject);
    }

    /// <summary>
    /// UI�ړ���
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="pos"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask UIMoveY
        (RectTransform rect
        , float pos
        , Ease ease)
    {
        await rect.DOAnchorPosY
            (pos, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(rect.gameObject);
    }

    #endregion

    #region ------ Fade
    /// <summary>
    /// �X�v���C�g�t�F�[�h
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="endValue"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask Fade
        (SpriteRenderer sprite
        , float endValue
        , Ease ease)
    {
        await sprite.DOFade
            (endValue, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(sprite.gameObject);
    }

    /// <summary>
    /// �C���[�W�t�F�[�h
    /// </summary>
    /// <param name="image"></param>
    /// <param name="endValue"></param>
    /// <param name="ease"></param>
    /// <returns></returns>
    private async UniTask Fade
        (Image image
        , float endValue
        , Ease ease)
    {
        await image.DOFade
            (endValue, _duration)
            .SetEase(ease)
            .SetUpdate(true)
            .SetLink(image.gameObject);
    }
    #endregion



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
