using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectUIController : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button selectDungeonBg;
    [SerializeField] private Button chapter_01_Button;
    [SerializeField] private Button dungeon_01_Button;

    [SerializeField] private GameObject selectChapterPanel;
    [SerializeField] private GameObject selectDungeonPanel;

    private void Start()
    {
        selectChapterPanel.SetActive(true);
        selectDungeonPanel.SetActive(false);

        backButton.onClick.AddListener(SceneManagerEx.Instance.OnBackClicked);
        selectDungeonBg.onClick.AddListener(CloseSelectDungeonPanel);
        chapter_01_Button.onClick.AddListener(OpenSelectDungeonPanel);
        dungeon_01_Button.onClick.AddListener(SceneManagerEx.Instance.OnDungeon1Clicked);
    }

    private void OpenSelectDungeonPanel()
    {
        selectDungeonPanel.SetActive(true);
    }

    private void CloseSelectDungeonPanel()
    {
        selectDungeonPanel.SetActive(false);
    }
}