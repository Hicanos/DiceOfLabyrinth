using UnityEngine;

public enum HudMode
{
    None,
    Lobby,
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

    [Header("Panels")]
    [SerializeField] private CheckPanel checkPanel;

    [ContextMenu("테스트")]
    void TestOpenCheckPanel()
    {
        if (checkPanel)
        {
            checkPanel.Open("안녕");
        }
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
            case HudMode.Lobby:
                stamina.SetActive(true);
                gold.SetActive(true);
                jewel.SetActive(true);
                settingButton.SetActive(true);
                homeButton.SetActive(false);
                sceneBackButton.SetActive(false);
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
                settingButton.SetActive(true);
                homeButton.SetActive(true);
                sceneBackButton.SetActive(false);
                break;

            case HudMode.None:
            default:
                break;
        }
    }
}
