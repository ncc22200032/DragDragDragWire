using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    // ---------------------------- SerializeField
    [Header("基礎パラメータ")]
    [SerializeField, Tooltip("テキスト")] private TextMeshProUGUI _text;
    [SerializeField, Tooltip("テキスト移動位置")] private float _pressedTextPos;
    [SerializeField, Tooltip("イメージ")] private Image _image;

    [Header("ノーマル")]
    [SerializeField] private Sprite _normalImage;

    [Header("ハイライト")]
    [SerializeField] private Sprite _highlightImage;
    [SerializeField] private UnityEvent _highlightClip;

    [Header("プレス")]
    [SerializeField] private Sprite _pressedImage;
    [SerializeField] private UnityEvent _pressedClip;

    // ---------------------------- Field
    private bool _isPlayPressClip;


    // ---------------------------- PublicMethod
    #region ------ ButtonLayer
    /// <summary>
    /// ノーマル
    /// </summary>
    public void Normal()
    {
        UpdateAnimation(
            Color.white, Vector3.zero,
            null,
            _normalImage);

        _isPlayPressClip = false;
    }

    /// <summary>
    /// ハイライト
    /// </summary>
    public void Highlight()
    {
        UpdateAnimation(
            Color.black, Vector3.zero,
            _highlightClip,
            _highlightImage);
    }

    /// <summary>
    /// プレス
    /// </summary>
    public void Pressed()
    {
        UpdateAnimation(
            Color.white, new Vector3(0, _pressedTextPos, 0),
            _pressedClip,
            _pressedImage);

        _isPlayPressClip = true;
    }

    /// <summary>
    /// セレクト
    /// </summary>
    public void Selected()
    {
        UpdateAnimation(
            Color.black, Vector3.zero,
            _highlightClip,
            _highlightImage);
    }

    /// <summary>
    /// ディサブル
    /// </summary>
    public void Disabled()
    {

    }

    #endregion






    // ---------------------------- PrivateMethod

    /// <summary>
    /// アニメーション更新
    /// </summary>
    /// <param name="textColor"></param>
    /// <param name="textPos"></param>
    /// <param name="clip"></param>
    /// <param name="image"></param>
    private void UpdateAnimation(
        Color textColor, Vector3 textPos,
        UnityEvent clip,
        Sprite image)
    {
        _text.color = textColor;
        _text.rectTransform.localPosition = textPos;
        if (!_isPlayPressClip)
        {
            clip?.Invoke();
        }
        _image.sprite = image;
    }
}
