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
    [SerializeField, Tooltip("フェード効果音")] private UnityEvent _sceneSwitchClip;

    // ---------------------------- Field
    private static Scheme _currentScheme = Scheme.KeyboardMouse;

    private bool _isFade = false;
    private readonly float FADE_TIME = 2;

    // ---------------------------- Property
    /// <summary>
    /// フェード再生状態取得
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
    /// 入力切換え
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
    /// デバッグスイッチ
    /// </summary>
    /// <returns></returns>
    public bool DebugSwitch()
    {
        return _debug == Switch.ON;
    }

    /// <summary>
    /// ゲーム終了
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
                UnityEditor.EditorApplication.isPlaying = false; //ゲームシーン終了
#else
        Application.Quit(); //build後にゲームプレイ終了が適用
#endif
            });
    }

    /// <summary>
    /// UI選択初期化
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
    /// UI選択初期化
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
    /// フェードアウト
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
    /// フェードアウト
    /// </summary>
    /// <param name="Obj"></param>
    public void SceneFadeOut(
        (GameObject, CanvasGroup) Obj)
    {
        SceneFade(ImageType.FADEOUT, Obj);
    }


    /// <summary>
    /// フェードイン
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
    /// フェードイン
    /// </summary>
    /// <param name="Obj"></param>
    public void SceneFadeIn(
        (GameObject, CanvasGroup) Obj)
    {
        SceneFade(ImageType.FADEIN, Obj);
    }

    /// <summary>
    /// フェード
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
    /// フェード
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
    /// フェード処理
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
        //  マスク変更
        Obj.fade.GetComponent<FadeImage>().UpdateMask(type);
        //  操作制限
        Obj.canvas.blocksRaycasts = false;
        Time.timeScale = 0;
        //  効果音
        _sceneSwitchClip?.Invoke();
        //  フェード
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
    /// デバッグ
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
