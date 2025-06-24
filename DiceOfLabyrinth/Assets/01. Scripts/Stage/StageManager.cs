using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class StageManager : MonoBehaviour
{
    public StageData stageData; // �������� �����͸� ��� ����

    private int currentStageIndex; // ���� �������� �ε���
    private int currentPhaseIndex; // ���� ������ �ε���
    private int gem; // �������� �������� ���̴� ��ȭ, ���������� ����� �ʱ�ȭ�˴ϴ�.
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

        // �������� �����Ͱ� �Ҵ���� ���� ��� ���� �޽����� ����մϴ�.
        if (stageData == null)
        {
            Debug.LogError("StageData is not assigned in the inspector!");
        }

        // Json ���Ͽ��� Ŭ����� �������� �����͸� �ε��ϴ� �޼��尡 ��������� ���⼭ ȣ���� �����Դϴ�.
    }

    public void StartStage(int stageIndex)
    {
        // �������� ���� ������ �����մϴ�.
        if (stageData.StageIndex[stageIndex].IsCompleted)
        {
            Debug.Log($"Stage {stageIndex} is already completed.");
            // �̹� �Ϸ�� ���������� �絵�� ���� ���θ� ���� UI�� ǥ���� �����Դϴ�.
            //if() // �絵���� �����ϴ� ��� ����, UI �Է¿� ���� �Ҹ��� �޼��尡 ��������� �߰��� �����Դϴ�.
            //{
            //    return;
            //}
        }
        else if (stageData.StageIndex[stageIndex].IsLocked)
        {
            Debug.Log($"Stage {stageIndex} is locked. Please complete previous stages.");
            // ��ݵ� ���������� ������ �� ���ٴ� UI �޽����� ǥ���� �����Դϴ�.
            return;
        }
        // �÷��̾� �����Ϳ� ���� �ڽ�Ʈ�� ��������� �ڽ�Ʈ �񱳸� �߰��� �����Դϴ�.
        //else if (stageData.StageIndex[stageIndex].StageCost > �÷��̾��� ���� �ڽ�Ʈ)
        //{
        //    // �ڿ��� �����ϴٴ� UI �޽����� ǥ���� �����Դϴ�.
        //    return;
        //}
        SceneManager.LoadScene("BattleScene");//SceneManagerEX.cs�� ��������� ������ �����Դϴ�.
        currentStageIndex = stageIndex; // ���� �������� �ε��� ����
        currentPhaseIndex = 0; // ���� ������ �ε��� �ʱ�ȭ
        gem = 0;
        StandbyPhase();
    }

    public void EndStage(int stageIndex, bool isSuccess)
    {
        // �������� ���� ������ �����մϴ�.
        // isSuccess�� ���� Ŭ���� ���θ� ó���ϰ�, Json ���Ͽ� �����͸� �����ϴ� �޼��带 ȣ���� �����Դϴ�.
        if (isSuccess)
        {
            Debug.Log($"Stage {stageIndex} cleared!");
            // Ŭ����� �������� ������ �����ϴ� ������ �߰��� �����Դϴ�.
            stageData.StageIndex[stageIndex].IsCompleted = true; // �������� �Ϸ� ���� ������Ʈ
            stageData.StageIndex[stageIndex+1].IsLocked = false; // ���� �������� ��� ����
            //���� ���� �߰� �����Դϴ�. ��: ����ġ, ���, ���� ��, �÷��̾� �����Ͱ� ��������� += �� �����Դϴ�.

        }
        else
        {
            Debug.Log($"Stage {stageIndex} failed.");
            // ���� �� ó�� ������ �߰��� �����Դϴ�.
        }
        SceneManager.LoadScene("MainMenu"); // ���� �޴��� ���ư���, // SceneManagerEX.cs�� ��������� ������ �����Դϴ�.
    }

    private void StandbyPhase()
    {
        //���� ������ ������ �ɷ�ġ ���� ������ �����մϴ�.
        BattlePhase(0); // ù ��° ������� �̵�
    }

    public void BattlePhase(int phaseIndex)
    {
        // ���� ������ ���� ������ �����մϴ�.
        if (phaseIndex < stageData.StageIndex[currentStageIndex].Phases.Length)
        {
            currentPhaseIndex = phaseIndex;
            PhaseData phaseData = stageData.StageIndex[currentStageIndex].Phases[currentPhaseIndex];
            Debug.Log($"Starting Battle Phase {phaseIndex} with {phaseData.Enemies.Count} enemies.");
            // �� ���� ������ �߰��� �����Դϴ�.
            // ��: SpawnEnemies(phaseData.Enemies);
        }
        else
        {
            Debug.LogError("Invalid phase index.");
        }

    }

    private void ShopPhase()
    {
        //���������� ����

        BattlePhase(4);
    }
    private void PhaseSuccess(bool isSuccess)
    {
        // ������ ���� ���ο� ���� ���� ������� �̵��ϰų� �������� ���� ������ ȣ���մϴ�.
        if (isSuccess)
        {
            Debug.Log($"Phase {currentPhaseIndex} cleared!");
            if (currentPhaseIndex < stageData.StageIndex[currentStageIndex].Phases.Length - 2)
            {
                BattlePhase(currentPhaseIndex + 1); // ���� ������� �̵�
            }
            else if (currentPhaseIndex == stageData.StageIndex[currentStageIndex].Phases.Length - 2)
            {
                ShopPhase(); //
            }
            else // 
            {
                EndStage(currentStageIndex, true); // �������� Ŭ����
            }
        }
        else
        {
            Debug.Log($"Phase {currentPhaseIndex} failed.");
            EndStage(currentStageIndex, false); // �������� ����
        }
    }
}
