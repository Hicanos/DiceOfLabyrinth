using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [field: SerializeField]
    public PublicUIController publicUIController { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HudMode hudmode = scene.name switch
        {
            "LobbyScene" => HudMode.Lobby,
            "SelectAdventureScene" => HudMode.SelectAdventure,
            "CharacterScene" => HudMode.Character,
            "BattleScene" => HudMode.Battle,
            "LoadingScene" => HudMode.None,
            _ => HudMode.Lobby
        };

        publicUIController.ApplyMode(hudmode);
        ActivateControllerForScene(scene.name);
    }

    private void ActivateControllerForScene(string sceneName)
    {
        // "ControllerContainer"를 찾음
        Transform container = transform.Find("ControllerContainer");
        if (container == null)
        {
            Debug.LogWarning("ControllerContainer가 존재하지 않습니다.");
            return;
        }

        // 컨테이너 하위의 오브젝트만 온오프
        foreach (Transform child in container)
        {
            bool isActive = child.name == sceneName;
            child.gameObject.SetActive(isActive);
        }
    }
}
