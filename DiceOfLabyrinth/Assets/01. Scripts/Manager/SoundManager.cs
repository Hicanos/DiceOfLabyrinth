using System;
using System.Collections.Generic;
using UnityEngine;
using static SoundManager;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;// 배경음악 재생용
    [SerializeField] private AudioSource sfxSource;// SFX 재생용

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [SerializeField] private List<SoundEntry> soundEntries;
    public List<SoundEntry> SoundEntries => soundEntries;

    private Dictionary<SoundType, AudioClip> soundDict;
    public enum SoundType { 
        UIClick, // UI 관련 사운드
        BGM_Lobby,// 로비 배경음악
        BGM_Dungeon, // 던전 배경음악
        BGM_NormalEliteBattle, // 엘리트 전투 배경음악
        BGM_GuardianBattle, // 가디언 전투 배경음악
        BGM_LordBattle, // 보스 전투 배경음악
        BGM_Title,// 타이틀 배경음악
        SFX_Victory,// 전투 승리 배경음악
        SFX_Defeat, // 전투 패배 배경음악
        SFX_Hit_Sword, // 타격음
        SFX_Swing, // 공격음
        SFX_Swing2, // 공격음2
    }

    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
        }
        soundDict = new Dictionary<SoundType, AudioClip>();
        foreach (var entry in soundEntries)
        {
            if (!soundDict.ContainsKey(entry.type) && entry.clip != null)
            {
                soundDict.Add(entry.type, entry.clip);
            }
        }
    }
    public void PlayBGM(SoundType type) // 배경음악 재생
    {
        if (soundDict.TryGetValue(type, out var clip))
        {
            // 같은 클립이면 재생 생략
            if (bgmSource.clip == clip && bgmSource.isPlaying)
                return;
            bgmSource.clip = clip;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
        else
        {
            bgmSource.Stop(); // 해당 타입의 클립이 없으면 현재 재생 중인 BGM을 중지
        }
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(SoundType type) // SFX 재생
    {
        if (soundDict.TryGetValue(type, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    public void SetBGMVolume(float volume) // 배경음악 볼륨 설정
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    public void SetSFXVolume(float volume) // SFX 볼륨 설정
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}

[System.Serializable]
public class SoundEntry
{
    [SerializeField] private string soundName;
    public SoundType type;
    public AudioClip clip;
}