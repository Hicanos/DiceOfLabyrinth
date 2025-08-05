using System;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }

    // 계정 정보
    public string nickname = "User";
    public int level = 1;
    public int exp = 0;

    // 재화
    public int currentStamina = 50;
    public int maxStamina = 50;
    public int gold = 0;
    public int jewel = 10000;
    public DateTime lastQuit;


    // 스테미나 회복
    [SerializeField] private int staminaRecoveryInterval = 300; // 스테미나 회복 간격 (5분마다 1 회복)
    public float remainingRecoveryTime = 0f; // 다음 회복까지 남은 시간
    private float recoveryTimer = 0f; // 게임 실행 중 경과 시간

    // 앱 종료 시점 저장용 키
    private const string LastQuitTimeKey = "LastQuitTimeKey";

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


    private void Update()
    {
        // 게임 실행 중 스테미나 자동 회복
        if (currentStamina < maxStamina)
        {
            recoveryTimer += Time.deltaTime; // 경과 시간 누적

            // 회복 할 시간이 지났는지 체크
            if (recoveryTimer >= staminaRecoveryInterval)
            {
                int recovered = (int)(recoveryTimer / staminaRecoveryInterval);

                // 회복 할 값이 있는 경우
                if (recovered > 0)
                currentStamina = Mathf.Min(currentStamina + recovered, maxStamina); // 1 회복, 최대치 제한
                recoveryTimer %= staminaRecoveryInterval; // 남은 시간만 남기고 초기화
                UIManager.Instance.publicUIController.Refresh();
            }

            // 종료 시 저장할 남은 시간
            remainingRecoveryTime = recoveryTimer;
        }
    }

    /// <summary>
    /// DataSaver의 저장된 종료 시점(LastQuitTime)으로부터 경과 시간 + 이전 잔여 시간만큼 스테미나 회복
    /// </summary>
    public void RecoverStaminaFromLastQuit()
    {
        // 저장된 데이터가 없으면 지금 시간을 기준으로 초기화
        if (DataSaver.Instance?.SaveData?.userData == null)
        {
            lastQuit = DateTime.Now;
            remainingRecoveryTime = 0f;
            return;
        }

        // 마지막 종료 시간 불러오기
        lastQuit = DataSaver.Instance.SaveData.userData.LastQuitTime;

        // 종료 이후 경과 시간
        TimeSpan diff = DateTime.Now - lastQuit;

        // 총 경과 시간 = 종료 후 경과 시간 + 저장된 남은 시간
        float totalElapsed = (float)diff.TotalSeconds + remainingRecoveryTime;

        // 회복할 양 계산
        int recovered = (int)(totalElapsed / staminaRecoveryInterval);
        float newRemaining = totalElapsed % staminaRecoveryInterval;

        if (recovered > 0)
        {
            currentStamina = Math.Min(currentStamina + recovered, maxStamina);
            UIManager.Instance.publicUIController.Refresh();
        }

        // 새로운 잔여 시간 저장
        recoveryTimer = newRemaining;
        remainingRecoveryTime = newRemaining;
    }

    // 경험치
    public void AddExp(int amount)
    {
        exp += amount;
        UIManager.Instance.publicUIController.Refresh();
    }

    // 스테미나
    public bool UseStamina(int amount)
    {
        if (currentStamina < amount)
            return false;

        currentStamina -= amount;
        UIManager.Instance.publicUIController.Refresh();
        return true;
    }

    public void AddStamina(int amount)
    {
        currentStamina += amount;
        UIManager.Instance.publicUIController.Refresh();
    }

    // 쥬얼
    public bool UseJewel(int amount)
    {
        if (jewel < amount)
            return false;

        jewel -= amount;
        UIManager.Instance.publicUIController.Refresh();
        return true;
    }

    public void AddJewel(int amount)
    {
        jewel += amount;
        UIManager.Instance.publicUIController.Refresh();
    }

    // 골드
    public bool UseGold(int amount)
    {
        if (gold < amount)
            return false;

        gold -= amount;
        UIManager.Instance.publicUIController.Refresh();
        return true;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.Instance.publicUIController.Refresh();
    }
}