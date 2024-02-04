using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using static BaseEnum;
using static FadeImage;

public class SystemBase : MonoBehaviour
{
    static public SystemBase Instance;
    // ---------------------------- SerializeField
    [SerializeField] private Switch _debug;
    [SerializeField, Tooltip("�t�F�[�h���ʉ�")] private UnityEvent _sceneSwitchClip;

    // ---------------------------- Field
    private static Scheme _currentScheme = Scheme.KeyboardMouse;

    private bool _isFade = false;
    private readonly float FADE_TIME = 2;

    // ---------------------------- Property
    /// <summary>
    /// �t�F�[�h�Đ���Ԏ擾
    /// </summary>
    public bool IsFade { get { return _isFade; } }


    // ---------------------------- UnityMessage
    private void Awake()
    {
        Instance = this;
        DG.Tweening.DOTween.SetTweensCapacity(tweenersCapacity: 20000, sequencesCapacity: 200);
    }

    private void Update()
    {
        if (_debug == Switch.ON)
        {
            BaseDebug();
        }
    }

    private void OnGUI()
    {
        if (SystemBase.Instance.DebugSwitch())
        {
            var guiPos = new Rect[30];
            for (int i = 0; i < guiPos.Length; i++)
            {
                guiPos[i] = new Rect(10, 1080 - i * 30, 300, 30);
            }
            var style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 25;

        }
    }



    // ---------------------------- PublicMethod
    /// <summary>
    /// ���͐؊���
    /// </summary>
    /// <param name="scheme"></param>
    /// <param name="act"></param>
    public void ControlChange(Scheme scheme, Action act)
    {
        switch (scheme)
        {
            case Scheme.KeyboardMouse:
                Cursor.visible = true;
                _currentScheme = Scheme.KeyboardMouse;
                EventSystem.current.SetSelectedGameObject(null);
                act?.Invoke();

                break;

            case Scheme.Gamepad:
                Cursor.visible = false;
                _currentScheme = Scheme.Gamepad;
                act?.Invoke();

                break;
        }
    }

    /// <summary>
    /// �f�o�b�O�X�C�b�`
    /// </summary>
    /// <returns></returns>
    public bool DebugSwitch()
    {
        return _debug == Switch.ON;
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void ApplicationQuit(
        (GameObject fade, CanvasGroup canvas) Obj)
    {
        Obj.canvas.blocksRaycasts = false;
        Obj.fade.GetComponent<FadeImage>().UpdateMask(ImageType.FADEIN);
        Obj.fade.GetComponent<Fade>().FadeIn(
            FADE_TIME,
            () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; //�Q�[���V�[���I��
#else
        Application.Quit(); //build��ɃQ�[���v���C�I�����K�p
#endif
            });
    }

    /// <summary>
    /// UI�I��������
    /// </summary>
    /// <param name="button"></param>
    public void InitUISelect(Button button)
    {
        if (_currentScheme == Scheme.Gamepad)
        {
            button.Select();
        }
    }

    /// <summary>
    /// UI�I��������
    /// </summary>
    /// <param name="slider"></param>
    public void InitUISelect(Slider slider)
    {
        if (_currentScheme == Scheme.Gamepad)
        {
            slider.Select();
        }
    }

    #region ------ Fade
    /// <summary>
    /// �t�F�[�h�A�E�g
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="act"></param>
    public void SceneFadeOut(
        (GameObject, CanvasGroup) Obj,
        Action act)
    {
        SceneFade(ImageType.FADEOUT, Obj, act);
    }

    /// <summary>
    /// �t�F�[�h�A�E�g
    /// </summary>
    /// <param name="Obj"></param>
    public void SceneFadeOut(
        (GameObject, CanvasGroup) Obj)
    {
        SceneFade(ImageType.FADEOUT, Obj);
    }


    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>
    /// <param name="Obj"></param>
    /// <param name="act"></param>
    public void SceneFadeIn(
        (GameObject, CanvasGroup) Obj,
        Action act)
    {
        SceneFade(ImageType.FADEIN, Obj, act);
    }

    /// <summary>
    /// �t�F�[�h�C��
    /// </summary>
    /// <param name="Obj"></param>
    public void SceneFadeIn(
        (GameObject, CanvasGroup) Obj)
    {
        SceneFade(ImageType.FADEIN, Obj);
    }

    /// <summary>
    /// �t�F�[�h
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Obj"></param>
    /// <param name="act"></param>
    private void SceneFade(
        ImageType type,
        (GameObject fade, CanvasGroup canvas) Obj,
        Action act)
    {
        FadeProcess(type, Obj,
            () =>
            {
                Time.timeScale = 1;
                Obj.canvas.blocksRaycasts = true;
                act();
                _isFade = false;
            });
    }

    /// <summary>
    /// �t�F�[�h
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Obj"></param>
    private void SceneFade(
        ImageType type,
        (GameObject fade, CanvasGroup canvas) Obj)
    {
        FadeProcess(type, Obj,
            () =>
            {
                Time.timeScale = 1;
                Obj.canvas.blocksRaycasts = true;
                _isFade = false;
            });
    }

    /// <summary>
    /// �t�F�[�h����
    /// </summary>
    /// <param name="type"></param>
    /// <param name="Obj"></param>
    /// <param name="act"></param>
    private void FadeProcess(
        ImageType type,
        (GameObject fade, CanvasGroup canvas) Obj
        , Action act)
    {
        _isFade = true;
        //  �}�X�N�ύX
        Obj.fade.GetComponent<FadeImage>().UpdateMask(type);
        //  ���쐧��
        Obj.canvas.blocksRaycasts = false;
        Time.timeScale = 0;
        //  ���ʉ�
        _sceneSwitchClip?.Invoke();
        //  �t�F�[�h
        var fadeScr = Obj.fade.GetComponent<Fade>();
        switch (type)
        {
            case ImageType.FADEOUT:
                fadeScr.FadeOut(FADE_TIME, act);
                break;

            case ImageType.FADEIN:
                fadeScr.FadeIn(FADE_TIME, act);
                break;

        }
    }

    #endregion



    // ---------------------------- PrivateMethod
    #region ------ Input
    /// <summary>
    /// �f�o�b�O
    /// </summary>
    private void BaseDebug()
    {
        var UI = UIManager.Instance;
        var Player = PlayerController.Instance;

        var current = Keyboard.current;
        var oneKey = current.digit1Key;
        var twoKey = current.digit2Key;
        var threeKey = current.digit3Key;
        var fourKey = current.digit4Key;
        var fiveKey = current.digit5Key;
        var sixKey = current.digit6Key;

        if (oneKey.wasPressedThisFrame)
        {
            UI.ApplicationQuit();
        }
        if (twoKey.wasPressedThisFrame)
        {

        }
        if (threeKey.wasPressedThisFrame)
        {
            UI.OnRestart();
        }
        if (fourKey.wasPressedThisFrame)
        {

        }
        if (fiveKey.wasPressedThisFrame)
        {

        }
        if (sixKey.wasPressedThisFrame)
        {

        }

        var uKey = current.uKey;
        var jKey = current.jKey;
        var pKey = current.pKey;

        if (uKey.wasPressedThisFrame)
        {
            Player.SetWireDis(5);
        }
        if (jKey.wasPressedThisFrame)
        {
            Player.SetWireDis(-5);
        }
        if (pKey.wasPressedThisFrame)
        {

        }
    }

    #endregion


}
