using UnityEngine;

public class EXPpotion : MonoBehaviour, IItem
{    
    // EXP 포션은 선택된 캐릭터의 경험치를 증가시킴.
    // 캐릭터의 아이디를 받고 해당 캐릭터의 경험치 상승 메서드 사용

    // 해당 코드는 로비UI에서 선택된 캐릭터에게 호출할 것. 일단 이정도로 OK
    // 차후 Item Manager에서 보유중인 아이템 개수도 차감할 것.
    public void UseItem(LobbyCharacter targetCharacter, int expAmount)
    {
        if (targetCharacter != null)
        {
            targetCharacter.AddExp(expAmount);
            Debug.Log($"EXP 포션 사용: {targetCharacter.CharacterData.nameKr}의 경험치가 {expAmount}만큼 증가했습니다.");
        }
        else
        {
            Debug.LogWarning("대상 캐릭터가 없습니다.");
        }
    }
}
