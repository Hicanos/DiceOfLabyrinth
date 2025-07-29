using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject summonCharactersPanel;
    private void OnEnable()
    {
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Lobby);
    }

    public void OnClickSummonButton()
    {
        lobbyPanel.SetActive(false);
        summonCharactersPanel.SetActive(true);
    }
}