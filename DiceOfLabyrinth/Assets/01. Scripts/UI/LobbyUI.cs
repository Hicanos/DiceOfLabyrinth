using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button adventureButton;

    private void Start()
    {
        adventureButton.onClick.AddListener(() => SceneManagerEx.Instance.LoadScene("DungeonSelectScene"));
    }
}