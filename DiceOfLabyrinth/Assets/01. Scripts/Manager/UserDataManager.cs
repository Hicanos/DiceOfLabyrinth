using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }

    [field: SerializeField] public UserData userdata { get; private set; }

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
        userdata.exp += amount;
        UIManager.Instance.publicUIController.Refresh();
    }

    // 스테미나
    public bool UseStamina(int amount)
    {
        if (userdata.stamina < amount)
            return false;

        userdata.stamina -= amount;
        UIManager.Instance.publicUIController.Refresh();
        return true;
    }

    public void AddStamina(int amount)
    {
        userdata.stamina += amount;
        UIManager.Instance.publicUIController.Refresh();
    }

    // 쥬얼
    public bool UseJewel(int amount)
    {
        if (userdata.jewel < amount)
            return false;

        userdata.jewel -= amount;
        UIManager.Instance.publicUIController.Refresh();
        return true;
    }

    public void AddJewel(int amount)
    {
        userdata.jewel += amount;
        UIManager.Instance.publicUIController.Refresh();
    }

    // 골드
    public bool UseGold(int amount)
    {
        if (userdata.gold < amount)
            return false;

        userdata.gold -= amount;
        UIManager.Instance.publicUIController.Refresh();
        return true;
    }

    public void AddGold(int amount)
    {
        userdata.gold += amount;
        UIManager.Instance.publicUIController.Refresh();
    }
}
