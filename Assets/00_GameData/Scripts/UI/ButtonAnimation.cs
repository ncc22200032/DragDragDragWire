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
    [Header("��b�p�����[�^")]
    [SerializeField, Tooltip("�e�L�X�g")] private TextMeshProUGUI _text;
    [SerializeField, Tooltip("�e�L�X�g�ړ��ʒu")] private float _pressedTextPos;
    [SerializeField, Tooltip("�C���[�W")] private Image _image;

    [Header("�m�[�}��")]
    [SerializeField] private Sprite _normalImage;

    [Header("�n�C���C�g")]
    [SerializeField] private Sprite _highlightImage;
    [SerializeField] private UnityEvent _highlightClip;

    [Header("�v���X")]
    [SerializeField] private Sprite _pressedImage;
    [SerializeField] private UnityEvent _pressedClip;

    // ---------------------------- Field
    private bool _isPlayPressClip;


    // ---------------------------- PublicMethod
    #region ------ ButtonLayer
    /// <summary>
    /// �m�[�}��
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
    /// �n�C���C�g
    /// </summary>
    public void Highlight()
    {
        UpdateAnimation(
            Color.black, Vector3.zero,
            _highlightClip,
            _highlightImage);
    }

    /// <summary>
    /// �v���X
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
    /// �Z���N�g
    /// </summary>
    public void Selected()
    {
        UpdateAnimation(
            Color.black, Vector3.zero,
            _highlightClip,
            _highlightImage);
    }

    /// <summary>
    /// �f�B�T�u��
    /// </summary>
    public void Disabled()
    {

    }

    #endregion






    // ---------------------------- PrivateMethod

    /// <summary>
    /// �A�j���[�V�����X�V
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
