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
            if (recoveryTimer >= staminaRecoveryInterval)
            {
                currentStamina = Mathf.Min(currentStamina + 1, maxStamina); // 1 회복, 최대치 제한
                recoveryTimer = 0f;
                UIManager.Instance.publicUIController.Refresh();
            }
        }
    }


    //private void OnApplicationPause(bool pause)
    //{
    //    // 앱이 백그라운드로 갔을 때 시간 저장
    //    if (pause)
    //    {
    //        SaveQuitTime();
    //    }
    //}

    //private void OnApplicationQuit()
    //{
    //    // 앱 완전 종료 시 시간 저장
    //    SaveQuitTime();
    //}

    ///// <summary>
    ///// 앱 종료 또는 백그라운드 진입 시간 저장
    ///// </summary>
    //private void SaveQuitTime()
    //{
    //    PlayerPrefs.SetString(LastQuitTimeKey, DateTime.Now.ToString());
    //    PlayerPrefs.Save();
    //}

    /// <summary>
    /// DataSaver의 저장된 종료 시점(LastQuitTime)으로부터 경과 시간만큼 스테미나 회복
    /// </summary>
    public void RecoverStaminaFromLastQuit()
    {
        if (DataSaver.Instance?.SaveData?.userData == null)
        {
            lastQuit = DateTime.Now;
            return;
        }
        lastQuit = DataSaver.Instance.SaveData.userData.LastQuitTime;
        TimeSpan diff = DateTime.Now - lastQuit;
        int recovered = (int)(diff.TotalSeconds / staminaRecoveryInterval);
        if (recovered > 0)
        {
            currentStamina = Math.Min(currentStamina + recovered, maxStamina);
            UIManager.Instance.publicUIController.Refresh();
        }
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
