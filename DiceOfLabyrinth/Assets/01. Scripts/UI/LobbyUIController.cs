using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject summonCharactersPanel;
    [SerializeField] private GameObject inventoryPanel;

    public MessagePopup messagePopup;

    private void OnEnable()
    {
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Lobby);
        TutorialManager.Instance.StartLobbyTutorial();
    }

    public void OnClickSummonButton()
    {
        lobbyPanel.SetActive(false);
        summonCharactersPanel.SetActive(true);

        UIManager.Instance.SetHudMode(HudMode.Summon);
    }

    public void OnClickInventoryButton()
    {
        lobbyPanel.SetActive(false);
        inventoryPanel.SetActive(true);

        UIManager.Instance.SetHudMode(HudMode.Inventory);
    }

    public void OnClickAdventureButton()
    {
        if (SceneManagerEx.Instance != null)
        {
            SceneManagerEx.Instance.LoadScene("SelectAdventureScene");
        }
    }

    public void OnClickStoreButton()
    {
        messagePopup.Open("미구현된 기능입니다.\n" +
            "추후 업데이트될 예정입니다.");
    }

    public void OnClickBackButton()
    {
        lobbyPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        summonCharactersPanel.SetActive(false);

        UIManager.Instance.SetHudMode(HudMode.Lobby);
    }
}