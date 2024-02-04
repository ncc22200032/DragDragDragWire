using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using DG.Tweening;
using Cysharp.Threading.Tasks;
using static BaseEnum;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    // ---------------------------- Enum
    private enum Frame
    {
        TITLE, STAGE, OPTION
    }

    // ---------------------------- SerializeField
    [Header("入力パラメータ")]
    [SerializeField, Tooltip("プレイヤー入力")] private PlayerInput _input;

    [Header("スイッチフレーム")]
    [SerializeField, Tooltip("フレーム")] private GameObject _frame;
    [SerializeField, Tooltip("フェード")] private GameObject _fade;
    [SerializeField, Tooltip("移動時間")] private float _duration;
    [SerializeField, Tooltip("サブフレーム")] private GameObject[] _frames;

    [Header("選択ボタン")]
    [SerializeField, Tooltip("タイトル")] private Button _titleFirstSelect;
    [SerializeField, Tooltip("ステージ")] private Button _stageFirstSelect;
    [SerializeField, Tooltip("オプション")] private Button _optionFirstSelect;

    [Header("UIアニメーション")]
    [SerializeField, Tooltip("ロゴ")] private GameObject[] _animeUI;
    [SerializeField, Tooltip("大きさ")] private float _loopValue;
    [SerializeField, Tooltip("演出時間")] private float _loopDuration;

    [Header("オーディオ")]
    [SerializeField, Tooltip("BGM")] private AudioSource _bgmSource;


    // ---------------------------- Field
    private Frame _currentState;
    private SystemBase Base;
    private (GameObject, CanvasGroup) Param;


    // ---------------------------- Property



    // ---------------------------- UnityMessage
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Base = SystemBase.Instance;
        Param = (_fade, _frame.GetComponent<CanvasGroup>());

        Base.SceneFadeOut(Param,
            () =>
            {
                Base.InitUISelect(_titleFirstSelect);
                _bgmSource.Play();
                foreach (var obj in _animeUI)
                {
                    UIAnime(obj);
                }
            });

    }

    // ---------------------------- PublicMethod
    /// <summary>
    /// 入力方法変更
    /// </summary>
    public void OnControlsChanged()
    {
        var currentString = _input.currentControlScheme;
        if (currentString == Scheme.KeyboardMouse.ToString())
        {
            Base?.ControlChange(Scheme.KeyboardMouse, null);
        }
        else if (currentString == Scheme.Gamepad.ToString())
        {
            Base?.ControlChange(Scheme.Gamepad, () => { InitButton(_currentState); });
        }
    }

    #region ------- Button

    /// <summary>
    /// スタート
    /// </summary>
    public void OnStart()
    {
        FrameMove(0, Frame.STAGE);
    }

    /// <summary>
    /// バックタイトル
    /// </summary>
    public void OnBackTitle()
    {
        FrameMove(WIDTH, Frame.TITLE);
    }

    /// <summary>
    /// 設定
    /// </summary>
    public void OnOption()
    {
        FrameMove(-WIDTH, Frame.OPTION);
    }

    /// <summary>
    /// バックステージ
    /// </summary>
    public void OnBackStage()
    {
        FrameMove(0, Frame.STAGE);
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void ApplicationQuit()
    {
        Base.ApplicationQuit(Param);
    }

    #endregion ----

    #region ------- LoadingStage
    /// <summary>
    /// ステージ１読み込み
    /// </summary>
    public void Load_Stage01()
    {
        Base.SceneFadeIn(Param,
            () => SceneChange(SceneState.STAGE01));
    }

    /// <summary>
    /// ステージ２読み込み
    /// </summary>
    public void Load_Stage02()
    {
        Base.SceneFadeIn(Param,
            () => SceneChange(SceneState.STAGE02));
    }

    /// <summary>
    /// ステージ３読み込み
    /// </summary>
    public void Load_Stage03()
    {
        Base.SceneFadeIn(Param,
            () => SceneChange(SceneState.STAGE03));
    }

    #endregion ----


    // ---------------------------- PrivateMethod
    /// <summary>
    /// シーン変更
    /// </summary>
    /// <param name="scene">シーン</param>
    private void SceneChange(SceneState target)
    {
        SceneManager.LoadScene((int)target);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// ロゴアニメーション
    /// </summary>
    private void UIAnime(GameObject obj)
    {
        obj.transform.DOScale
            (_loopValue, _loopDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(obj);
    }

    /// <summary>
    /// フレーム移動
    /// </summary>
    /// <param name="target"></param>
    /// <param name="active"></param>
    private async void FrameMove(float target, Frame frame)
    {
        //  状態保存
        _currentState = frame;

        //  入力制限
        for (int i = 0; i < _frames.Length; i++)
        {
            _frames[i].SetActive(true);
            _frames[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        //  フレーム移動
        await _frame.GetComponent<RectTransform>()
         .DOLocalMoveX(target, _duration)
         .SetEase(Ease.OutBack)
         .SetLink(_frame);

        //  入力制限解除
        for (int i = 0; i < _frames.Length; i++)
        {
            var isActive = (i == (int)frame);
            _frames[i].GetComponent<CanvasGroup>().blocksRaycasts = isActive;
            _frames[i].SetActive(isActive);
        }

        //  初期ボタン選択
        InitButton(frame);
    }

    /// <summary>
    /// ボタン選択初期化
    /// </summary>
    /// <param name="frame"></param>
    private void InitButton(Frame frame)
    {
        switch (frame)
        {
            case Frame.TITLE:
                Base.InitUISelect(_titleFirstSelect);

                break;

            case Frame.STAGE:
                Base.InitUISelect(_stageFirstSelect);

                break;

            case Frame.OPTION:
                Base.InitUISelect(_optionFirstSelect);

                break;
        }
    }
}
