using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject pedigreePopup;
    [SerializeField] private GameObject settingPopup;

    [Header("Buttons")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button pedigreeButton;
    [SerializeField] private Button exitButton;

    public void OnClickContinue()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickExit()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        StageManager.Instance.EndChapterEarly(StageManager.Instance.stageSaveData.currentChapterIndex);
    }

    public void OnClickSetting()
    {
        if (settingPopup != null)
            settingPopup.SetActive(true);
    }

    public void OnClickPedigree()
    {
        if (pedigreePopup != null)
            pedigreePopup.SetActive(true);
    }
}
