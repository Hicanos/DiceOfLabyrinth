using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    float loadTime = 0;

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        string nextScene = SceneManagerEx.Instance.nextSceneName;
        AsyncOperation async = SceneManager.LoadSceneAsync(nextScene);
        async.allowSceneActivation = false; // 로딩이 완료되는대로 씬을 활성화할것인지

        while (!async.isDone) // isDone는 로딩이 완료되었는지 확인하는 변수
        {
            loadTime += Time.deltaTime; // 시간을 더해줌

            // 로딩이 얼마나 완료되었는지 0~1의 값으로 보여줌. 1이 되면 씬이 활성화 되기 때문에,
            // 지금은 최소 1초를 기다리도록 설정해서 씬이 다 로드가 됐다면 대기시간 동안은 0.9가 출력될 것임.
            //Debug.Log($"{async.progress}"); 

            if (loadTime > 1f) // 최소 1초는 기다리도록 설정. 지금은 씬이 가볍기 때문에 설정을 풀면 씬을 바로 넘어갈듯.
            {
                async.allowSceneActivation = true; // 씬 활성화
            }

            yield return null;
        }
    }
}
