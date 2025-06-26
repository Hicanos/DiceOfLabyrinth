using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
    public static SceneManagerEx Instance;

    private Stack<string> sceneStack = new Stack<string>();
    public string nextSceneName { get; private set; } // 로딩 후 이동할 씬 이름

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

    public void OnDungeon1Clicked() // 추후 팀 편성UI를 추가한 뒤 탐험 버튼을 누르면 배틀 씬으로 이동하게 변경.
    {
        LoadScene("BattleScene");
    }

    public void OnBackClicked()
    {
        Debug.Log("뒤로 가기");
        LoadPreviousScene();
    }

    public void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name; // 현재 씬 이름을 저장
        sceneStack.Push(currentScene); // 현재 씬을 스택에 추가. 추후에 꺼내면 그게 이전 씬이 됨.
        nextSceneName = sceneName; // 일단 이동할 씬을 저장해놓고, 먼저 로딩 씬으로 이동
        SceneManager.LoadScene("LoadingScene");
    }

    public void LoadPreviousScene()
    {
        if (sceneStack.Count > 0) // 이전 씬이 있다면,
        {
            string previousScene = sceneStack.Pop(); // 이전 씬을 스택에서 제거하고 변수에 저장
            nextSceneName = previousScene; // 일단 이동할 씬을 저장해놓고, 먼저 로딩 씬으로 이동
            SceneManager.LoadScene("LoadingScene");
        }
        else // 이전 씬이 없다면,
        {
            Debug.Log("이전 씬이 없습니다.");
        }
    }
}