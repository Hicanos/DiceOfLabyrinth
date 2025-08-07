using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUISoundController : MonoBehaviour
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text masterVolumeText;
    [SerializeField] private TMP_Text bgmVolumeText;
    [SerializeField] private TMP_Text sfxVolumeText;

    private void Start()
    {
        // 슬라이더 값 초기화 (SoundManager의 현재 값으로)
        masterSlider.value = SoundManager.Instance.masterVolume;
        bgmSlider.value = SoundManager.Instance.bgmVolume;
        sfxSlider.value = SoundManager.Instance.sfxVolume;
        // 텍스트 초기화
        masterVolumeText.text = $"{masterSlider.value * 100:F0}%";
        bgmVolumeText.text = $"{bgmSlider.value * 100:F0}%";
        sfxVolumeText.text = $"{sfxSlider.value * 100:F0}%";

        // 슬라이더 이벤트 등록
        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
        masterVolumeText.text = $"{value * 100:F0}%";
    }

    private void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
        bgmVolumeText.text = $"{value * 100:F0}%";
    }

    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
        sfxVolumeText.text = $"{value * 100:F0}%";
    }
}