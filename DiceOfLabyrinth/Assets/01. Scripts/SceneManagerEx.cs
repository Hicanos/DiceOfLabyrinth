using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
    public static SceneManagerEx Instance;
    private Stack<string> sceneStack = new Stack<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnAdventureClicked()
    {
        LoadScene("DungeonSelectScene");
    }

    public void OnBackClicked()
    {
        Debug.Log("뒤로 가기");
        LoadPreviousScene();
    }

    public void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        sceneStack.Push(currentScene);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPreviousScene()
    {
        if (sceneStack.Count > 0)
        {
            string previousScene = sceneStack.Pop();
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.Log("이전 씬이 없습니다.");
        }

    }
}
