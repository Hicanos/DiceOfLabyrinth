using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class StageManager : MonoBehaviour
{
    public StageData stageData; // �������� �����͸� ��� ����
    public static StageManager Instance { get; private set; }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ������Ʈ�� �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� �����ϸ� �ߺ� ���� ����
        }

        // Json ���Ͽ��� Ŭ����� �������� �����͸� �ε��ϴ� �޼��尡 ��������� ���⼭ ȣ���� �����Դϴ�.
    }

    public void StartStage(int stageIndex)
    {
        // �������� ���� ������ �����մϴ�.
        SceneManager.LoadScene("BattleScene");
        stageData.StageIndex[stageIndex].IsLocked = false; // �������� ��� ����
    }

    public void EndStage(int stageIndex, bool isSuccess)
    {
        // �������� ���� ������ �����մϴ�.
        // isSuccess�� ���� Ŭ���� ���θ� ó���ϰ�, Json ���Ͽ� �����͸� �����ϴ� �޼��带 ȣ���� �����Դϴ�.
        if (isSuccess)
        {
            Debug.Log($"Stage {stageIndex} cleared!");
            // Ŭ����� �������� ������ �����ϴ� ������ �߰��� �����Դϴ�.
        }
        else
        {
            Debug.Log($"Stage {stageIndex} failed.");
            // ���� �� ó�� ������ �߰��� �����Դϴ�.
        }
        SceneManager.LoadScene("MainMenu"); // ���� �޴��� ���ư���
    }

}
