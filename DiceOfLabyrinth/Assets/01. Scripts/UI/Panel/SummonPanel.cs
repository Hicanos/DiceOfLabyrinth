using UnityEngine;
using UnityEngine.UI;

public class SummonPanel : MonoBehaviour
{
    [SerializeField] private MessagePopup messagePopup;

    public void OnClickProbabilityInfo()
    {
        messagePopup.Open("<color=#6a6a6a>[확률 정보]</color>\n" +
            "<color=#ff0000>SSR 캐릭터</color> : 0.5%\n" +
            "<color=#ff0000>SSR 아이템</color> : 1.5%\n" +
            "<color=#0000ff>SR 아이템</color> : 18%\n" +
            "<color=#00ff00>R 아이템</color> : 80%");
    }

    public void OnClickExchange()
    {
        messagePopup.Open("미구현된 기능입니다.\n" +
            "추후 업데이트될 예정입니다.");
    }
}