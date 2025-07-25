using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    private void OnEnable()
    {
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Lobby);
    }
}