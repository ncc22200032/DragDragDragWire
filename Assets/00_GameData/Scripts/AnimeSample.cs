using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//  使用時に自動追加されますが
//  この using がないとUniTask DOTweenは使用できません
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
    /// サンプル
    /// </summary>
    private async void TaskSample()
    {
        //  async　をつけてメソッドをつくる
        //  await　でその処理が終わるまで次の処理を待つ
        //  UniTaskはゲームが終了する等の強力なイベントがない限り
        //  処理し続けます、なのでキャンセル処理を挟むことで安全に使用します


        // ------ 時間を待つ　(TimeSpan.FromSeconds(時間), timeScale == 0 でも動くかどうか))
        await Canceled(UniTask.Delay(TimeSpan.FromSeconds(_waitTime), true));


        // ------ 移動
        await Canceled(ObjMove(gameObject, _vec3Pos, Ease.Linear));


        // ------ UI移動
        await Canceled(UIMove(_rect, _vec3Pos, Ease.Linear));


        // ------ フェード
        await Canceled(Fade(_sprite, _endValue, Ease.Linear));
        await Canceled(Fade(_image, _endValue, Ease.Linear));


        // ------ リストに入れた処理を全て行うまで待つ
        var tasks = new List<UniTask>()
        {
            //  宣言時にリストに要素を入れる
            Canceled(ObjMoveX(gameObject,_floatPos,Ease.Linear)),
            Canceled(ObjMoveX(gameObject,_floatPos,Ease.Linear))
        };
        //  宣言したあとに追加する
        tasks.Add(Canceled(UIMoveX(_rect, _floatPos, Ease.Linear)));
        tasks.Add(Canceled(UIMoveY(_rect, _floatPos, Ease.Linear)));
        //  全ての処理を行う
        await Canceled(UniTask.WhenAll(tasks));
    }

    #region ------ ObjMove
    //  Transformオブジェクトの移動

    /// <summary>
    /// 移動
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
            (pos, _duration)    //  目標値、演出時間
            .SetEase(ease)      //  動き方
            .SetUpdate(true)    //  timeScale == 0 でも動くかどうか
            .SetLink(obj);      //  安全にDOTweenを使用するための設定
    }

    /// <summary>
    /// 移動x
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
    /// 移動y
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
    //  RectTransformオブジェクトの移動

    /// <summary>
    /// UI移動
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
    /// UI移動x
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
    /// UI移動ｙ
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
    /// スプライトフェード
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
    /// イメージフェード
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
