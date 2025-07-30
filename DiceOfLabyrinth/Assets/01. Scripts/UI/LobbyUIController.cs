using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject summonCharactersPanel;
    private void OnEnable()
    {
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Lobby);
        TutorialManager.Instance.StartLobbyTutorial();
    }

    public void OnClickSummonButton()
    {
        lobbyPanel.SetActive(false);
        summonCharactersPanel.SetActive(true);
    }

    public void OnClickAdventureButton()
    {
        if (SceneManagerEx.Instance != null)
        {
            SceneManagerEx.Instance.LoadScene("SelectAdventureScene");
        }
    }
}