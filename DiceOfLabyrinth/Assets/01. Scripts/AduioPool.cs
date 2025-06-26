using UnityEngine;
using System.Collections.Generic;

public enum BackGroundMusic
{
    temp1,
    temp2
}

public enum UISFX
{
    temp1,
    temp2
}

public class AudioPool : MonoBehaviour
{
    public List<AudioClip> BackGroundMusicList = new List<AudioClip>();    
    public List<AudioClip> UISFXList = new List<AudioClip>();
}
