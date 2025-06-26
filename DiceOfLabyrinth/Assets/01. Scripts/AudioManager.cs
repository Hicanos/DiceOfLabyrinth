using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioPool pool;
    public AudioSource mainAudioSource;
    public AudioSource subAudioSource;
    public AudioSource tempAudioSource;

    public void PlayOneShot(UISFX clip)
    {
        tempAudioSource.PlayOneShot(pool.BackGroundMusicList[(int)clip]);
    }

    public void PlayBackGroundMusic(BackGroundMusic clip)
    {
        StopAllCoroutines();
        IEnumerator checkAudio;

        mainAudioSource.clip = pool.BackGroundMusicList[(int)clip];
        mainAudioSource.Play();

        checkAudio = CheckAudio(mainAudioSource);
        StartCoroutine(checkAudio);
    }

    IEnumerator CheckAudio(AudioSource audio)
    {
        yield return new WaitForSeconds(20);

        while (true)
        {
            if (audio.isPlaying)
            {
                yield return new WaitForSeconds(5);
            }
            else
            {
                //audio.clip = 새 오디오 클립 받아오기
                audio.Play();
            }
        }
    }
}
