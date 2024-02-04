using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using static BaseEnum;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    // ---------------------------- SerializeField
    [Header("入力パラメータ")]
    [SerializeField, Tooltip("プレイヤー入力")] private PlayerInput _input;

    [Header("選択ボタン")]
    [SerializeField, Tooltip("初期選択")] private Button _firstSelect;
    [SerializeField, Tooltip("初期選択")] private Button _clearSelect;
    [SerializeField, Tooltip("初期選択")] private Button _gameOverSelect;


    [Header("スイッチキャンバス")]
    [SerializeField, Tooltip("フレーム")] private CanvasGroup _frame;
    [SerializeField, Tooltip("フェード")] private GameObject _fade;
    [SerializeField, Tooltip("デフォルト")] private GameObject _defaultFrame;
    [SerializeField, Tooltip("オプション")] private GameObject _optionFrame;
    [SerializeField, Tooltip("移動時間")] private float _duration;

    [Header("UI")]
    [SerializeField, Tooltip("HP")] private GameObject[] _hpObjects;
    [SerializeField, Tooltip("ガイド")] private GameObject _guide;
    [SerializeField, Tooltip("フェード制限")] private float _fadeLimit;
    [SerializeField, Tooltip("フェード待機時間")] private float _fadeWaitTime;
    [SerializeField, Tooltip("フェードアウト時間")] private float _fadeoutTime;

    [Header("時間")]
    [SerializeField, Tooltip("テキスト")] private TextMeshProUGUI _timeText;
    [SerializeField, Tooltip("制限時間")] private float _limitTime;
    [SerializeField, Tooltip("スコア獲得時間")] private int _getScoreTime;

    [Header("次ステージ")]
    [SerializeField, Tooltip("次ステージ")] private SceneState _nextScene;

    [Header("リザルト")]
    [SerializeField, Tooltip("リザルトフレーム")] private GameObject _resultFrame;
    [SerializeField, Tooltip("初期化位置")] private float _initUIPos;
    [SerializeField, Tooltip("スコア表示")] private GameObject[] _scoreObjects;

    [SerializeField, Tooltip("BGM")] private AudioSource _bgmSource;
    [SerializeField, Tooltip("フェード音量")] private float _fadeBgmVolume;
    [SerializeField, Tooltip("背景")] private Image _resultPanel;
    [SerializeField, Tooltip("背景アルファ")] private float _resultPanelAlpha;
    [SerializeField, Tooltip("ロゴ")] private GameObject _clearLogo;
    [SerializeField, Tooltip("ロゴ")] private GameObject _gameOverLogo;
    [SerializeField, Tooltip("演出時間")] private float _logoDuration;

    [SerializeField, Tooltip("待機時間")] private float _resultWait;

    [SerializeField, Tooltip("UI")] private RectTransform[] _clearUI;
    [SerializeField, Tooltip("UI")] private RectTransform[] _gameOverUI;
    [SerializeField, Tooltip("演出時間")] private float _resultUIDuration;
    [SerializeField, Tooltip("ずらし")] private float _resultUIShifting;

    [SerializeField, Tooltip("スコア")] private TextMeshProUGUI[] _scoreTexts;
    [SerializeField, Tooltip("演出時間")] private float _scoreDuration;

    [Header("オーディオ")]
    [SerializeField, Tooltip("リザルト")] private UnityEvent[] _resultClips;
    [SerializeField, Tooltip("スコア")] private UnityEvent _scoreClip;



    // ---------------------------- Field
    //  シーン遷移
    private SystemBase Base;
    private (GameObject, CanvasGroup) Param;
    private bool _canFade = true;
    private bool _isMoveMenu = false;

    //  ゲームプレイステータス
    private GameState _state;
    private readonly string DEFAULT_MAP = "Player", PAUSE_MAP = "Pause";

    //  コイン
    private float _score;
    private int _currentCoinCount;
    private readonly int MAX_COIN = 3;

    //  時間
    private float _nowTime;
    private readonly int GET_HUNDRED = 100;

    //  HP
    private float _hp;

    //  リザルト
    private readonly List<float> _clearUIPos = new();
    private readonly List<float> _gameOverUIPos = new();


    // ---------------------------- Property
    public GameState State { get { return _state; } }
    public bool IsMoveMenu { get { return _isMoveMenu; } }
    public float CoinScore
    {
        set
        {
            _currentCoinCount++;
            _score += value;
        }
    }


    // ---------------------------- UnityMessage
    private void Awake()
    {
        Instance = this;
    }

    private async void Start()
    {
        Base = SystemBase.Instance;
        Param = (_fade, _frame);
        _nowTime = _limitTime;

        var tasks = new List<UniTask>();
        SetPos(_clearUI, _clearUIPos);
        SetPos(_gameOverUI, _gameOverUIPos);
        void SetPos(RectTransform[] setPos, List<float> getPos)
        {
            foreach (var rect in setPos)
            {
                getPos.Add(rect.anchoredPosition.y);
                tasks.Add(InitUIPos(rect, _initUIPos));
            }
        }
        await Canceled(UniTask.WhenAll(tasks));

        await Canceled(InitUIPos(_clearLogo.GetComponent<RectTransform>(), 0));
        await Canceled(InitUIPos(_gameOverLogo.GetComponent<RectTransform>(), 0));

        async UniTask InitUIPos(RectTransform rect, float pos)
        {
            await rect.DOAnchorPosY(pos, 0);
        }

        _bgmSource.Play();
    }

    private void Update()
    {
        StartFadeOut();
        UpdateTime();
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
    #region ------- Button
    /// <summary>
    /// ポーズ
    /// </summary>
    public void OnPause()
    {
        if (_state == GameState.DEFAULT)
        {
            StateChange(GameState.PAUSE);
        }

    }

    /// <summary>
    /// バック
    /// </summary>
    public void OnBack()
    {
        if (_state == GameState.PAUSE)
        {
            StateChange(GameState.DEFAULT);
        }
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void ApplicationQuit()
    {
        Base.ApplicationQuit(Param);
    }

    /// <summary>
    /// リスタート
    /// </summary>
    public void OnRestart()
    {
        Base.SceneFadeIn(Param,
            () => SceneManager.LoadScene(SceneManager.GetActiveScene().name));
    }

    /// <summary>
    /// タイトル
    /// </summary>
    /// <param name="scene">シーン</param>
    public void OnTitle()
    {
        Base.SceneFadeIn(Param,
            () => SceneManager.LoadScene((int)SceneState.TITLE));
    }

    /// <summary>
    /// 次ステージ
    /// </summary>
    public void OnNext()
    {
        Base.SceneFadeIn(Param,
            () => SceneManager.LoadScene((int)_nextScene));
    }

    #endregion
    /// <summary>
    /// コントロール切換え
    /// </summary>
    public void ControlChange()
    {
        UISelect();
    }

    /// <summary>
    /// HP入力
    /// </summary>
    /// <param name="max"></param>
    /// <param name="current"></param>
    public void HPSet(float max, float current)
    {
        _hp = current;
        for (int i = 0; i < _hpObjects.Length; i++)
        {
            _hpObjects[i].SetActive(i < current && i < max);
        }

        if (current <= 0)
        {
            StateChange(GameState.GAMEOVER);
        }
    }

    /// <summary>
    /// クリア
    /// </summary>
    public void GameClear()
    {
        StateChange(GameState.GAMECLEAR);
    }

    // ---------------------------- PrivateMethod
    /// <summary>
    /// フェードスタート
    /// </summary>
    private void StartFadeOut()
    {
        if (_canFade)
        {
            _canFade = false;
            Base.SceneFadeOut(Param,
                () =>
                {
                    if (_guide != null)
                    {
                        StartCoroutine(FadeGuide(_guide));
                    }
                });
        }
    }

    /// <summary>
    /// 時間更新
    /// </summary>
    private void UpdateTime()
    {
        _nowTime -= Time.deltaTime;
        _timeText.text = $"TIME {Mathf.CeilToInt(_nowTime)}";
        if (_nowTime < 0)
        {
            StateChange(GameState.GAMEOVER);
        }
    }

    /// <summary>
    /// ステータス変更
    /// </summary>
    /// <param name="state">ステータス</param>
    private void StateChange(GameState state)
    {
        _state = state;

        switch (state)
        {
            case GameState.DEFAULT:
                SetState(1, true, DEFAULT_MAP);
                MoveOptionFrame(false, WIDTH);
                break;

            case GameState.PAUSE:
                SetState(0, false, PAUSE_MAP);
                MoveOptionFrame(true, 0);
                break;

            case GameState.GAMECLEAR:
                SetState(0, false, PAUSE_MAP);
                GameClearStaging();
                break;

            case GameState.GAMEOVER:
                SetState(0, false, PAUSE_MAP);
                GameOverStaging();
                break;
        }

        void SetState
        (int time
        , bool isDefault
        , string actionMap)
        {
            Time.timeScale = time;
            _defaultFrame.SetActive(isDefault);
            _input.SwitchCurrentActionMap(actionMap);
            UISelect();
        }
    }

    /// <summary>
    /// 初期選択
    /// </summary>
    private void UISelect()
    {
        switch (_state)
        {
            case GameState.PAUSE:
                Base.InitUISelect(_firstSelect);
                break;

            case GameState.GAMECLEAR:
                Base.InitUISelect(_clearSelect);
                break;

            case GameState.GAMEOVER:
                Base.InitUISelect(_gameOverSelect);
                break;
        }
    }

    /// <summary>
    /// オプション画面移動
    /// </summary>
    /// <param name="isPause"></param>
    /// <param name="target"></param>
    private async void MoveOptionFrame
        (bool isPause
        , float target)
    {
        _isMoveMenu = true;
        _optionFrame.GetComponent<CanvasGroup>().blocksRaycasts = false;
        if (!_optionFrame.activeSelf)
        {
            _optionFrame.SetActive(true);
        }

        await _optionFrame.GetComponent<RectTransform>()
            .DOLocalMoveX(target, _duration)
            .SetLink(_optionFrame)
            .SetUpdate(true)
            .SetEase(Ease.OutBack);

        _optionFrame.SetActive(isPause);
        _optionFrame.GetComponent<CanvasGroup>().blocksRaycasts = isPause;
        _isMoveMenu = false;
    }

    /// <summary>
    /// クリア演出
    /// </summary>
    private async void GameClearStaging()
    {
        await Canceled(ResultStaging(_clearLogo, _clearUI, _clearUIPos));
    }

    /// <summary>
    /// ゲームオーバー演出
    /// </summary>
    private async void GameOverStaging()
    {
        await Canceled(ResultStaging(_gameOverLogo, _gameOverUI, _gameOverUIPos));
    }

    /// <summary>
    /// リザルト演出
    /// </summary>
    /// <param name="logo"></param>
    /// <param name="ui"></param>
    /// <returns></returns>
    private async UniTask ResultStaging
        (GameObject logo
        , RectTransform[] ui
        , List<float> uiPos)
    {
        //  設定
        _guide.SetActive(false);
        _resultFrame.SetActive(true);
        if (_state == GameState.GAMECLEAR)
        {
            for (int i = 0; i < _scoreObjects.Length; i++)
            {
                _scoreObjects[i].SetActive(i < _currentCoinCount);
            }
        }
        BlocksRayCasts(false);

        if (_state == GameState.GAMECLEAR)
        {
            foreach (var clip in _resultClips)
            {
                clip?.Invoke();
            }
        }

        //  ロゴ演出
        var logoTasks = new List<UniTask>
        {
            PanelFade()
            ,LogoFade()
            ,Scale()
            ,AudioFade(_bgmSource,_fadeBgmVolume,_logoDuration)
        };
        async UniTask PanelFade()
        {
            await _resultPanel.DOFade
                (_resultPanelAlpha, _logoDuration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetLink(_resultPanel.gameObject);
        }
        async UniTask LogoFade()
        {
            await logo.GetComponent<TextMeshProUGUI>().DOFade
                (1, _logoDuration)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetLink(logo);
        }
        async UniTask Scale()
        {
            await logo.GetComponent<RectTransform>().DOScale
                (1, _logoDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true)
                .SetLink(logo);
        }
        await Canceled(UniTask.WhenAll(logoTasks));

        //  待機処理
        await Canceled(UniTask.Delay(TimeSpan.FromSeconds(_resultWait), true));

        //  UI移動処理
        var uiTasks = new List<UniTask>();
        for (int i = 0; i < ui.Length; i++)
        {
            uiTasks.Add(Move(i));
        }
        async UniTask Move(int num)
        {
            await ui[num].DOAnchorPosY
                (uiPos[num], _resultUIDuration + _resultUIShifting * num)
                .SetEase(Ease.OutBack)
                .SetLink(ui[num].gameObject)
                .SetUpdate(true);
        }
        await Canceled(UniTask.WhenAll(uiTasks));

        //  スコア表示
        if (_state == GameState.GAMECLEAR)
        {
            if (_currentCoinCount == MAX_COIN)
            {
                _score += GET_HUNDRED;
            }

            _scoreClip?.Invoke();
            await Canceled(Count(_scoreTexts[0], (int)_score));

            _scoreClip?.Invoke();
            var timeScore = (int)(Math.Round(_nowTime / _getScoreTime + 1) * GET_HUNDRED);
            await Canceled(Count(_scoreTexts[1], timeScore));

            _scoreClip?.Invoke();
            var hpScore = (int)(_hp * GET_HUNDRED);
            await Canceled(Count(_scoreTexts[2], hpScore));

            _scoreClip?.Invoke();
            var totalScore = (int)_score + timeScore + hpScore;
            await Canceled(Count(_scoreTexts[3], totalScore));

            async UniTask Count(TextMeshProUGUI text, int score)
            {
                await DOVirtual.Int(0, score, _scoreDuration,
                    (value) =>
                    {
                        text.text = value.ToString();
                    })
                    .SetEase(Ease.OutBack)
                    .SetLink(text.gameObject)
                    .SetUpdate(true);
            }
        }

        await Canceled(AudioFade(_bgmSource, 1, _logoDuration * 2));

        BlocksRayCasts(true);
    }

    /// <summary>
    /// UI接触判定変更
    /// </summary>
    /// <param name="rayCast"></param>
    private void BlocksRayCasts(bool rayCast)
    {
        _frame.blocksRaycasts = rayCast;
    }

    /// <summary>
    /// ガイドフェード制御
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeGuide(GameObject obj)
    {
        yield return new WaitForSeconds(_fadeWaitTime);
        float time = _fadeoutTime;
        while (time > 0)
        {
            time -= Time.deltaTime;
            var alpha = time / _fadeoutTime;
            if (alpha > _fadeLimit)
            {
                obj.GetComponent<CanvasGroup>().alpha = alpha;
            }

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// オーディオフェード
    /// </summary>
    /// <param name="source"></param>
    /// <param name="endValue"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private async UniTask AudioFade
        (AudioSource source
        , float endValue
        , float duration)
    {
        await DOVirtual.Float
            (source.volume, endValue, duration
            , (value) =>
            {
                source.volume = value;
            })
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .SetLink(source.gameObject);

    }

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