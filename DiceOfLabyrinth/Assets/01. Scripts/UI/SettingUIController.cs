using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    [SerializeField] private Button settingButton;
    [SerializeField] private Button settingBg;

    [SerializeField] private GameObject settingPanel;
    [SerializeField] private GameObject settingPopup;

    private void Start()
    {
        settingButton.onClick.AddListener(OpenSetting);
        settingBg.onClick.AddListener(CloseSetting);
    }

    private void OpenSetting()
    {
        settingPanel.SetActive(true);
    }

    private void CloseSetting()
    {
        settingPanel.SetActive(false);
    }
}
