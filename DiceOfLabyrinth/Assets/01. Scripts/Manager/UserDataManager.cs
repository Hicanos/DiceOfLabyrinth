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
    public int jewel = 0;

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
