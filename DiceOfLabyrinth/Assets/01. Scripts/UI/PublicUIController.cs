using TMPro;
using UnityEngine;

public enum HudMode
{
    None,
    Title,
    Lobby,
    Inventory,// 로비씬의 가방
    Summon, // 로비씬의 소환
    Character,
    SelectAdventure,
    Battle,
}

public class PublicUIController : MonoBehaviour
{
    [Header("HUD Elements")]
    [SerializeField] private GameObject stamina;
    [SerializeField] private GameObject gold;
    [SerializeField] private GameObject jewel;
    [SerializeField] private GameObject settingButton;
    [SerializeField] private GameObject homeButton;
    [SerializeField] private GameObject sceneBackButton;

    [Header("Text")]
    [SerializeField] private TMP_Text staminaText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text jewelText;

    [Header("Popup")]
    [SerializeField] private GameObject messagePopup;
    [SerializeField] private GameObject settingPopup;

    private void Start()
    {
        messagePopup.SetActive(false);
    }

    public void Refresh()
    {
        UserDataManager manager = UserDataManager.Instance;

        if (manager == null)
            return;

        staminaText.text = $"{manager.currentStamina}/{manager.maxStamina}";
        goldText.text = manager.gold.ToString("N0");
        jewelText.text = manager.jewel.ToString("N0");
    }

    public void ApplyMode(HudMode mode)
    {
        stamina.SetActive(false);
        gold.SetActive(false);
        jewel.SetActive(false);
        settingButton.SetActive(false);
        homeButton.SetActive(false);
        sceneBackButton.SetActive(false);

        switch (mode)
        {
            case HudMode.Title:
                stamina.SetActive(false);
                gold.SetActive(false);
                jewel.SetActive(false);
                settingButton.SetActive(true);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(false);
                break;

            case HudMode.Lobby:
                stamina.SetActive(true);
                gold.SetActive(true);
                jewel.SetActive(true);
                settingButton.SetActive(true);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(false);
                break;

            case HudMode.Inventory: // 로비씬의 가방
                stamina.SetActive(false);
                gold.SetActive(true);
                jewel.SetActive(true);
                settingButton.SetActive(true);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(false);
                break;

            case HudMode.Summon: // 로비씬의 소환
                stamina.SetActive(false);
                gold.SetActive(true);
                jewel.SetActive(true);
                settingButton.SetActive(true);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(false);
                break;

            case HudMode.Character:
                stamina.SetActive(false);
                gold.SetActive(true);
                jewel.SetActive(true);
                settingButton.SetActive(true);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(true);
                break;

            case HudMode.SelectAdventure:
                stamina.SetActive(true);
                gold.SetActive(true);
                jewel.SetActive(true);
                settingButton.SetActive(true);
                homeButton.SetActive(true);
                sceneBackButton.SetActive(true);
                break;

            case HudMode.Battle:
                stamina.SetActive(false);
                gold.SetActive(false);
                jewel.SetActive(false);
                settingButton.SetActive(false);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(false);
                break;

            case HudMode.None:
            default:
                break;
        }
    }

    public void OnClickBackButton()
    {
        if (CharacterUIController.Instance != null &&
            CharacterUIController.Instance.gameObject != null &&
            CharacterUIController.Instance.characterInfoPopup != null &&
            CharacterUIController.Instance.characterListPopup != null &&
            CharacterUIController.Instance.gameObject.activeSelf &&
            CharacterUIController.Instance.characterInfoPopup.activeSelf &&
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "CharacterScene")
        {
            // 캐릭터 정보 팝업이 열려있을 때는 팝업을 닫고 리스트 팝업을 연다
            CharacterUIController.Instance.characterInfoPopup.SetActive(false);
            CharacterUIController.Instance.characterListPopup.SetActive(true);
            return;
        }
        SceneManagerEx.Instance.LoadPreviousScene();
    }
    public void OnClickHomeButton()
    {
        SceneManagerEx.Instance.LoadScene("LobbyScene");
    }

    public void OnClickSettingButton()
    {
        if (settingPopup != null)
            settingPopup.SetActive(true);
    }
}
