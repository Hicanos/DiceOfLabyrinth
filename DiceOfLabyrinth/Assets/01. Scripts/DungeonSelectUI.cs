using UnityEngine;
using UnityEngine.UI;

public class DungeonSelectUI : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Start()
    {
        backButton.onClick.AddListener(SceneManagerEx.Instance.OnBackClicked);
    }
}