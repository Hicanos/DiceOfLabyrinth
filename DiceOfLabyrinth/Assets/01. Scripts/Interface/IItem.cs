/// <summary>
/// 아이템 인터페이스
/// </summary>

public interface IItem
{
    void UseItem(LobbyCharacter targetCharacter, int amount); // 대상 캐릭터와 사용량을 받는 메서드
}
