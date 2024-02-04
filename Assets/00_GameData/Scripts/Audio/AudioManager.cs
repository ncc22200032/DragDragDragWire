using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// ���ʉ��ʐݒ�
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
    [SerializeField, Tooltip("�~�L�T�[")] private AudioMixer mixer;
    [SerializeField, Tooltip("�X���C�_�[")] private Slider[] sliders;

    // ---------------------------- Field
    //  �I�[�f�B�I�~�L�T�[�O���[�v�@�p�����[�^�[������
    private static readonly string[] GROUP_PARAMS = { "Master", "BGM", "SE" };




    // ---------------------------- UnityMessage
    private void Awake()
    {
        InitAudioFirstOnce();    //  �Q�[���J�n����x����������
    }


    private void Start()
    {
        InitAudio();    //  �V�[���J�n��������
    }




    // ---------------------------- PublicMethod
    #region ------ ChangeSliderVolume
    /// <summary>
    /// �}�X�^�[�{�����[���ύX
    /// </summary>
    /// <param name="newSliderValue"></param>
    public void ChangeValue_Master(float newSliderValue)
    {
        ChangeValue((int)Group.MASTER, newSliderValue);
    }

    /// <summary>
    /// BGM�{�����[���ύX
    /// </summary>
    /// <param name="newSliderValue"></param>
    public void ChangeValue_BGM(float newSliderValue)
    {
        ChangeValue((int)Group.BGM, newSliderValue);
    }

    /// <summary>
    /// SE�{�����[���ύX
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
    /// �N����������
    /// </summary>
    private void InitAudioFirstOnce()
    {
        if (!SoundSetting.isInit)
        {
            for (int i = 0; i < SoundSetting.Volume.Length; i++)
            {
                mixer.GetFloat(GROUP_PARAMS[i], out float value);    //  �~�L�T�[�̉��ʂ��擾
                SoundSetting.Volume[i] = ConvertDbToVolume(value); //  �~�L�T�[�̉��ʂ𓯊�
            }
            SoundSetting.isInit = true;
        }
    }

    /// <summary>
    /// �V�[���J�n��������
    /// </summary>
    private void InitAudio()
    {
        for (int i = 0; i < SoundSetting.Volume.Length; i++)
        {
            mixer.SetFloat(GROUP_PARAMS[i], ConvertVolumeToDb(SoundSetting.Volume[i]));  //  �~�L�T�[
            sliders[i].value = SoundSetting.Volume[i];  //  �X���C�_�[
        }
    }

    #endregion ---

    #region ------ AudioAdjustment
    /// <summary>
    /// �{�����[���ύX
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    private void ChangeValue(int array, float value)
    {
        SoundSetting.Volume[array] = value;
        mixer.SetFloat(GROUP_PARAMS[array], ConvertVolumeToDb(value));
    }

    /// <summary>
    /// �~�L�T�[�̒l���X���C�_�[�p�ɒ���(Db -> Volume)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float ConvertDbToVolume(float value)
    {
        return Mathf.Clamp(((float)Math.Pow(10, value / 20)), 0f, 1f);
    }

    /// <summary>
    /// �X���C�_�[�̒l���~�L�T�[�p�ɒ���(Volume -> Db)
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    private float ConvertVolumeToDb(float volume)
    {
        return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)) * 20f, -80f, 20f);
    }

    #endregion ---
}
