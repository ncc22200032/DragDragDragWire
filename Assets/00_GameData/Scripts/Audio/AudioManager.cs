using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// 共通音量設定
    /// </summary>
    static class SoundSetting
    {
        public static bool isInit = false;

        public static float[] Volume { get; set; } = new float[Enum.GetValues(typeof(Group)).Length];
    }

    // ---------------------------- Enum
    private enum Group
    {
        MASTER, BGM, SE
    }

    // ---------------------------- SerializeField
    [SerializeField, Tooltip("ミキサー")] private AudioMixer mixer;
    [SerializeField, Tooltip("スライダー")] private Slider[] sliders;

    // ---------------------------- Field
    //  オーディオミキサーグループ　パラメーター文字列
    private static readonly string[] GROUP_PARAMS = { "Master", "BGM", "SE" };




    // ---------------------------- UnityMessage
    private void Awake()
    {
        InitAudioFirstOnce();    //  ゲーム開始時一度だけ初期化
    }


    private void Start()
    {
        InitAudio();    //  シーン開始時初期化
    }




    // ---------------------------- PublicMethod
    #region ------ ChangeSliderVolume
    /// <summary>
    /// マスターボリューム変更
    /// </summary>
    /// <param name="newSliderValue"></param>
    public void ChangeValue_Master(float newSliderValue)
    {
        ChangeValue((int)Group.MASTER, newSliderValue);
    }

    /// <summary>
    /// BGMボリューム変更
    /// </summary>
    /// <param name="newSliderValue"></param>
    public void ChangeValue_BGM(float newSliderValue)
    {
        ChangeValue((int)Group.BGM, newSliderValue);
    }

    /// <summary>
    /// SEボリューム変更
    /// </summary>
    /// <param name="newSliderValue"></param>
    public void ChangeValue_SE(float newSliderValue)
    {
        ChangeValue((int)Group.SE, newSliderValue);
    }

    #endregion ---




    // ---------------------------- PrivateMethod
    #region ------ Init
    /// <summary>
    /// 起動時初期化
    /// </summary>
    private void InitAudioFirstOnce()
    {
        if (!SoundSetting.isInit)
        {
            for (int i = 0; i < SoundSetting.Volume.Length; i++)
            {
                mixer.GetFloat(GROUP_PARAMS[i], out float value);    //  ミキサーの音量を取得
                SoundSetting.Volume[i] = ConvertDbToVolume(value); //  ミキサーの音量を同期
            }
            SoundSetting.isInit = true;
        }
    }

    /// <summary>
    /// シーン開始時初期化
    /// </summary>
    private void InitAudio()
    {
        for (int i = 0; i < SoundSetting.Volume.Length; i++)
        {
            mixer.SetFloat(GROUP_PARAMS[i], ConvertVolumeToDb(SoundSetting.Volume[i]));  //  ミキサー
            sliders[i].value = SoundSetting.Volume[i];  //  スライダー
        }
    }

    #endregion ---

    #region ------ AudioAdjustment
    /// <summary>
    /// ボリューム変更
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    private void ChangeValue(int array, float value)
    {
        SoundSetting.Volume[array] = value;
        mixer.SetFloat(GROUP_PARAMS[array], ConvertVolumeToDb(value));
    }

    /// <summary>
    /// ミキサーの値をスライダー用に調整(Db -> Volume)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float ConvertDbToVolume(float value)
    {
        return Mathf.Clamp(((float)Math.Pow(10, value / 20)), 0f, 1f);
    }

    /// <summary>
    /// スライダーの値をミキサー用に調整(Volume -> Db)
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    private float ConvertVolumeToDb(float volume)
    {
        return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)) * 20f, -80f, 20f);
    }

    #endregion ---
}
