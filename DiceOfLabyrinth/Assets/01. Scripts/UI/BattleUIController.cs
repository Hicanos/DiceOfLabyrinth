using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    [SerializeField] private GameObject stageSelectPanel;
    [SerializeField] private GameObject battlePanel;

    private void Start()
    {
        stageSelectPanel.SetActive(true);
        battlePanel.SetActive(false);

        backButton.onClick.AddListener(SceneManagerEx.Instance.OnBackClicked);
        nextButton.onClick.AddListener(OpenBattlePanel);
    }

    private void OpenBattlePanel()
    {
        stageSelectPanel.SetActive(false);
        battlePanel.SetActive(true);
    }
}