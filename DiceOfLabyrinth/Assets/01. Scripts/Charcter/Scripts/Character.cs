using UnityEngine;
using UnityEngine.TextCore.Text;
/// <summary>
/// 생성된 CharacterSO데이터를 사용하여
/// 캐릭터의 기본 동작을 구현하는 MonoBehaviour
/// 상속하여 각 캐릭터의 동작을 정의할 수 있음
/// </summary>
public class Character : MonoBehaviour
{    
    [Header("기본 데이터")]
    [SerializeField] protected CharacterSO characterData;

    [Header("동적 데이터")]
    public int Level = 1; // 레벨

    // 전투 중에 사용되는 동적 데이터
    public int CurrentHP; // 현재 HP
    public int CurrentExp; // 현재 경험치
    public int CurrentATK; // 현재 공격력
    public int CurrentDEF; // 현재 방어력
    public int CurrentCritChance; // 현재 크리티컬 확률
    public int CurrentCritDamage; // 현재 크리티컬 데미지

    /// <summary>
    /// 캐릭터 초기화 (초기 설정)
    /// </summary>
    public virtual void Initialize(CharacterSO so, int level = 1)
    {
        characterData = so;
        this.Level = level;
        CurrentHP = GetMaxHP();
    }

    /// <summary>
    /// 레벨에 따라 캐릭터의 HP 계산
    /// </summary>
    
    public int GetMaxHP()
    {
        return characterData.baseHP + characterData.plusHP * (Level - 1);
    }

    public int GetATK()
    {
        return characterData.baseATK + characterData.plusATK * (Level - 1);
    }

    public int GetDEF()
    {
        return characterData.baseDEF + characterData.plusDEF * (Level - 1);
    }

    public void LevelUp()
    {
        //만약 경험치가 이후 레벨업에 필요한 경험치보다 크다면 처리

    }

}
