using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private Button adventureButton;

    private void Start()
    {
        adventureButton.onClick.AddListener(() => SceneManagerEx.Instance.OnAdventureClicked());
    }
}