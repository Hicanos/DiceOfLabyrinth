using TMPro;
using UnityEngine;

public enum HudMode
{
    None,
    Lobby,
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

    public void Refresh()
    {
        UserData userdata = UserDataManager.Instance.userdata;

        if (userdata == null)
            return;

        staminaText.text = userdata.stamina.ToString("N0");
        goldText.text = userdata.gold.ToString("N0");
        jewelText.text = userdata.jewel.ToString("N0");
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
