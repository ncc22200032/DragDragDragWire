using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using DG.Tweening;

public class SliderAnimation : MonoBehaviour
{
    // ---------------------------- SerializeField
    [Header("イメージ")]
    [SerializeField, Tooltip("ゲージバック")] private Image _fillFrameImg;
    [SerializeField, Tooltip("ゲージ")] private Image _fillImg;
    [SerializeField, Tooltip("ハンドルバック")] private Image _handleFrameImg;
    [SerializeField, Tooltip("ハンドル")] private Image _handleImg;

    [Header("アニメーションパラメータ")]
    [SerializeField, Tooltip("再生時間")] private float _animeDuration;

    [Header("ノーマル")]
    [SerializeField, Tooltip("カラー")] private Color _normalColor;
    [SerializeField, Tooltip("カラー")] private Color _normalFrameColor;

    [Header("ハイライト")]
    [SerializeField, Tooltip("カラー")] private Color _highlightColor;
    [SerializeField, Tooltip("カラー")] private Color _highlightFrameColor;
    [SerializeField, Tooltip("クリップ")] private UnityEvent _highlightClip;


    // ---------------------------- PublicMethod
    #region ------ ButtonLayer
    /// <summary>
    /// ノーマル
    /// </summary>
    public void Normal()
    {
        UpdateAnimation(_normalColor, _normalFrameColor);
    }

    /// <summary>
    /// ハイライト
    /// </summary>
    public void Highlight()
    {
        UpdateAnimation(_highlightColor, _highlightFrameColor);
        _highlightClip?.Invoke();
    }

    /// <summary>
    /// プレス
    /// </summary>
    public void Pressed()
    {

    }

    /// <summary>
    /// セレクト
    /// </summary>
    public void Selected()
    {
        UpdateAnimation(_highlightColor, _highlightFrameColor);
        _highlightClip?.Invoke();
    }

    /// <summary>
    /// ディサブル
    /// </summary>
    public void Disabled()
    {

    }

    #endregion

    // ---------------------------- PrivateMethod

    private void UpdateAnimation(Color content, Color frame)
    {
        ChangeColor(_fillImg, content);
        ChangeColor(_fillFrameImg, frame);
        ChangeColor(_handleImg, content);
        ChangeColor(_handleFrameImg, frame);
    }

    private void ChangeColor(Image img, Color toColor)
    {
        DOVirtual.Color(
            img.color, toColor,
            _animeDuration,
            (result) =>
            {
                img.color = result;
            })
            .SetUpdate(true)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
    }
}
