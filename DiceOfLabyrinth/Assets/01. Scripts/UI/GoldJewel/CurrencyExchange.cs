using UnityEngine;

public class CurrencyExchange : MonoBehaviour
{
    // 골드 - 쥬얼간 교환 (골드 100 = 1 쥬얼)
    // 스태미나 - 쥬얼 교환 (스태미나 50 = 100 쥬얼)
    // 쥬얼 - 골드 교환 (쥬얼 1 = 골드 100)

    [Header("Popup")]
    [SerializeField] private MessagePopup messagePopup;

    [Header("Rates")]
    [SerializeField] private int goldPerJewel = 100;
    [SerializeField] private int staminaPerPack = 50;
    [SerializeField] private int jewelCostPerStaminaPack = 100;


    public void GoldToJewelButton() // 쥬얼 버튼
    {
        if (UserDataManager.Instance == null || messagePopup == null)
        {
            return;
        }

        string message = $"골드 {goldPerJewel}을(를) 쥬얼 1개로 교환할까요?\n";
        messagePopup.Open(
            message,
            onYes: () =>
            {
                if (UserDataManager.Instance.gold < 1)
                {
                    messagePopup.Open("골드가 부족합니다.");
                    return;
                }
                ExchangeGoldToJewel(goldPerJewel);
                messagePopup.Open($"쥬얼 1개를 획득했습니다.");
            }
        );
    }

    public void JewelToGoldButton() // 골드 버튼
    {
        if (UserDataManager.Instance == null)
        {
            return;
        }

        if (UserDataManager.Instance.jewel < 1)
        {
            if (messagePopup != null)
            {
                messagePopup.Open("쥬얼이 부족합니다.");
                return;
            }
        }
        ExchangeJewelToGold(1);
        if (messagePopup != null)
        {
            messagePopup.Open($"골드 {goldPerJewel}을(를) 획득했습니다.");
        }
    }
    
    public void JewelToStaminaButton() // 스태미나 버튼
    {
        if (UserDataManager.Instance == null)
        {
            return;
        }

        if (UserDataManager.Instance.jewel < jewelCostPerStaminaPack)
        {
            if (messagePopup != null)
            {
                messagePopup.Open("쥬얼이 부족합니다.");
                return;
            }
        }
        ExchangeJewelToStamina(jewelCostPerStaminaPack);
        if (messagePopup != null)
        {
            messagePopup.Open($"스태미나 {staminaPerPack}이(가) 충전되었습니다.");
        }
    }



    // 골드를 쥬얼로 교환
    private void ExchangeGoldToJewel(int goldAmount)
    {
        if (goldAmount <= 0)
        {
            return;
        }

        int jewelAmount = goldAmount / goldPerJewel;
        if (jewelAmount <= 0)
        {
            return;
        }

        bool paid = UserDataManager.Instance.UseGold(goldAmount);
        if (!paid)
        {
            return;
        }
        UserDataManager.Instance.AddJewel(jewelAmount);
    }

    // 쥬얼을 골드로 교환
    private void ExchangeJewelToGold(int jewelAmount)
    {
        if (jewelAmount <= 0)
        {
            return;
        }

        int goldAmount = jewelAmount * goldPerJewel;

        bool paid = UserDataManager.Instance.UseJewel(jewelAmount);
        if (!paid)
        {
            return;
        }
        UserDataManager.Instance.AddGold(goldAmount);
    }


    // 쥬얼을 스태미나로 교환(스태미나 50 = 100 쥬얼, 반드시 50단위로 충전됨)
    private void ExchangeJewelToStamina(int jewelAmount)
    {
        if (jewelAmount < jewelCostPerStaminaPack)
        {
            return;
        }

        int packs = jewelAmount / jewelCostPerStaminaPack;
        if (packs <= 0)
        {
            return;
        }

        int staminaAmount = packs * staminaPerPack;

        bool paid = UserDataManager.Instance.UseJewel(packs * jewelCostPerStaminaPack);
        if (!paid)
        {
            return;
        }

        UserDataManager.Instance.AddStamina(staminaAmount);
    }
}