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
    [Header("���̓p�����[�^")]
    [SerializeField, Tooltip("�v���C���[����")] private PlayerInput _input;

    [Header("�X�C�b�`�t���[��")]
    [SerializeField, Tooltip("�t���[��")] private GameObject _frame;
    [SerializeField, Tooltip("�t�F�[�h")] private GameObject _fade;
    [SerializeField, Tooltip("�ړ�����")] private float _duration;
    [SerializeField, Tooltip("�T�u�t���[��")] private GameObject[] _frames;

    [Header("�I���{�^��")]
    [SerializeField, Tooltip("�^�C�g��")] private Button _titleFirstSelect;
    [SerializeField, Tooltip("�X�e�[�W")] private Button _stageFirstSelect;
    [SerializeField, Tooltip("�I�v�V����")] private Button _optionFirstSelect;

    [Header("UI�A�j���[�V����")]
    [SerializeField, Tooltip("���S")] private GameObject[] _animeUI;
    [SerializeField, Tooltip("�傫��")] private float _loopValue;
    [SerializeField, Tooltip("���o����")] private float _loopDuration;

    [Header("�I�[�f�B�I")]
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
    /// ���͕��@�ύX
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
    /// �X�^�[�g
    /// </summary>
    public void OnStart()
    {
        FrameMove(0, Frame.STAGE);
    }

    /// <summary>
    /// �o�b�N�^�C�g��
    /// </summary>
    public void OnBackTitle()
    {
        FrameMove(WIDTH, Frame.TITLE);
    }

    /// <summary>
    /// �ݒ�
    /// </summary>
    public void OnOption()
    {
        FrameMove(-WIDTH, Frame.OPTION);
    }

    /// <summary>
    /// �o�b�N�X�e�[�W
    /// </summary>
    public void OnBackStage()
    {
        FrameMove(0, Frame.STAGE);
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void ApplicationQuit()
    {
        Base.ApplicationQuit(Param);
    }

    #endregion ----

    #region ------- LoadingStage
    /// <summary>
    /// �X�e�[�W�P�ǂݍ���
    /// </summary>
    public void Load_Stage01()
    {
        Base.SceneFadeIn(Param,
            () => SceneChange(SceneState.STAGE01));
    }

    /// <summary>
    /// �X�e�[�W�Q�ǂݍ���
    /// </summary>
    public void Load_Stage02()
    {
        Base.SceneFadeIn(Param,
            () => SceneChange(SceneState.STAGE02));
    }

    /// <summary>
    /// �X�e�[�W�R�ǂݍ���
    /// </summary>
    public void Load_Stage03()
    {
        Base.SceneFadeIn(Param,
            () => SceneChange(SceneState.STAGE03));
    }

    #endregion ----


    // ---------------------------- PrivateMethod
    /// <summary>
    /// �V�[���ύX
    /// </summary>
    /// <param name="scene">�V�[��</param>
    private void SceneChange(SceneState target)
    {
        SceneManager.LoadScene((int)target);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// ���S�A�j���[�V����
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
    /// �t���[���ړ�
    /// </summary>
    /// <param name="target"></param>
    /// <param name="active"></param>
    private async void FrameMove(float target, Frame frame)
    {
        //  ��ԕۑ�
        _currentState = frame;

        //  ���͐���
        for (int i = 0; i < _frames.Length; i++)
        {
            _frames[i].SetActive(true);
            _frames[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        //  �t���[���ړ�
        await _frame.GetComponent<RectTransform>()
         .DOLocalMoveX(target, _duration)
         .SetEase(Ease.OutBack)
         .SetLink(_frame);

        //  ���͐�������
        for (int i = 0; i < _frames.Length; i++)
        {
            var isActive = (i == (int)frame);
            _frames[i].GetComponent<CanvasGroup>().blocksRaycasts = isActive;
            _frames[i].SetActive(isActive);
        }

        //  �����{�^���I��
        InitButton(frame);
    }

    /// <summary>
    /// �{�^���I��������
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
