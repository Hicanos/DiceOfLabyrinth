using System.Collections.Generic;
using System;

[Serializable]
public class UserData
{
    // 계정 정보
    public string nickname = "User";
    public int level = 1;
    public int exp = 0;

    // 재화
    public int stamina = 100;
    public int gold = 0;
    public int jewel = 0;

    // 보유 캐릭터
    public List<OwnedCharacter> ownedCharacter = new();

    // 스테이지 진행 정보

    // 편성
}
