using UnityEngine;

public class CurrencyExchange : MonoBehaviour
{
    // 골드 - 쥬얼간 교환 (골드 100 = 1 쥬얼)
    // 스태미나 - 쥬얼 교환 (스태미나 50 = 100 쥬얼)
    // 쥬얼 - 골드 교환 (쥬얼 1 = 골드 100)


    public void GoldToJewelButton()
    {

    }

    public void JewelToGoldButton()
    {
    }
    
    public void JewelToStaminaButton()
    {
        
    }



    // 골드를 쥬얼로 교환
    private void ExchangeGoldToJewel(int goldAmount)
    {
        int jewelAmount = goldAmount / 100;
        if (jewelAmount > 0)
        {
            UserDataManager.Instance.UseGold(goldAmount);
            UserDataManager.Instance.AddJewel(jewelAmount);
        }
    }

    // 쥬얼을 골드로 교환
    private void ExchangeJewelToGold(int jewelAmount)
    {
        int goldAmount = jewelAmount * 100;
        if (UserDataManager.Instance.jewel >= jewelAmount)
        {
            UserDataManager.Instance.UseJewel(jewelAmount);
            UserDataManager.Instance.AddGold(goldAmount);
        }
    }


    // 쥬얼을 스태미나로 교환(스태미나 50 = 100 쥬얼, 반드시 50단위로 충전됨)
    private void ExchangeJewelToStamina(int jewelAmount)
    {
        int staminaAmount = jewelAmount / 2;
        if (staminaAmount > 0 && UserDataManager.Instance.jewel >= jewelAmount)
        {
            UserDataManager.Instance.UseJewel(jewelAmount);
            UserDataManager.Instance.AddStamina(staminaAmount);
            if (UserDataManager.Instance.currentStamina > UserDataManager.Instance.maxStamina)
            {
                UserDataManager.Instance.currentStamina = UserDataManager.Instance.maxStamina;
            }
        }
    }

}
