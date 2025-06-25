using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button dungeon_01_Button;

    private void Start()
    {
        backButton.onClick.AddListener(SceneManagerEx.Instance.OnBackClicked);
        dungeon_01_Button.onClick.AddListener(SceneManagerEx.Instance.OnDungeon1Clicked);
    }
}