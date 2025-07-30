using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject summonCharactersPanel;
    [SerializeField] private GameObject inventoryPanel;

    private void OnEnable()
    {
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Lobby);
    }

    public void OnClickSummonButton()
    {
        lobbyPanel.SetActive(false);
        summonCharactersPanel.SetActive(true);
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

    public void OnClickBackButton()
    {
        lobbyPanel.SetActive(true);
        inventoryPanel.SetActive(false);

        UIManager.Instance.SetHudMode(HudMode.Lobby);
    }

    public void ReturnToLobby()
    {
        lobbyPanel.SetActive(true);
        inventoryPanel.SetActive(false);
        summonCharactersPanel.SetActive(false);
    }
}