using UnityEngine;

public class EXPpotion : MonoBehaviour, IItem
{
    // 아이템 SO 정보 가져오기-해당 아이템의 데이터는 

    public void UseItem()
    {
        // EXP 포션은 사용한 대상 캐릭터의 경험치를 증가시킴
        
    }

    // EXP 포션은 선택된 캐릭터의 경험치를 증가시킴.
    // 캐릭터의 아이디를 받고 해당 캐릭터의 경험치 상승 메서드 사용

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

    //선택된 캐릭터의 데이터를 받는 메서드



}
