using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using DG.Tweening;

public class SliderAnimation : MonoBehaviour
{
    // ---------------------------- SerializeField
    [Header("�C���[�W")]
    [SerializeField, Tooltip("�Q�[�W�o�b�N")] private Image _fillFrameImg;
    [SerializeField, Tooltip("�Q�[�W")] private Image _fillImg;
    [SerializeField, Tooltip("�n���h���o�b�N")] private Image _handleFrameImg;
    [SerializeField, Tooltip("�n���h��")] private Image _handleImg;

    [Header("�A�j���[�V�����p�����[�^")]
    [SerializeField, Tooltip("�Đ�����")] private float _animeDuration;

    [Header("�m�[�}��")]
    [SerializeField, Tooltip("�J���[")] private Color _normalColor;
    [SerializeField, Tooltip("�J���[")] private Color _normalFrameColor;

    [Header("�n�C���C�g")]
    [SerializeField, Tooltip("�J���[")] private Color _highlightColor;
    [SerializeField, Tooltip("�J���[")] private Color _highlightFrameColor;
    [SerializeField, Tooltip("�N���b�v")] private UnityEvent _highlightClip;


    // ---------------------------- PublicMethod
    #region ------ ButtonLayer
    /// <summary>
    /// �m�[�}��
    /// </summary>
    public void Normal()
    {
        UpdateAnimation(_normalColor, _normalFrameColor);
    }

    /// <summary>
    /// �n�C���C�g
    /// </summary>
    public void Highlight()
    {
        UpdateAnimation(_highlightColor, _highlightFrameColor);
        _highlightClip?.Invoke();
    }

    /// <summary>
    /// �v���X
    /// </summary>
    public void Pressed()
    {

    }

    /// <summary>
    /// �Z���N�g
    /// </summary>
    public void Selected()
    {
        UpdateAnimation(_highlightColor, _highlightFrameColor);
        _highlightClip?.Invoke();
    }

    /// <summary>
    /// �f�B�T�u��
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
