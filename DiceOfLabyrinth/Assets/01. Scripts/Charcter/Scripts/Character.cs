using UnityEngine;
using UnityEngine.TextCore.Text;
/// <summary>
/// 공통 베이스
/// </summary>
public class Character : MonoBehaviour
{
    // 캐릭터 데이터
    [SerializeField] protected CharacterSO characterData;
    public int Level = 1;

    public virtual void Initialize(CharacterSO so, int level = 1)
    {
        characterData = so;
        Level = level;
    }
    // 공통 메서드 (GetMaxHP 등)
    /// <summary>
    /// 최대 HP 계산 (외부 강화 미포함)
    /// </summary>
    public virtual int GetMaxHP()
    {
        return characterData.baseHP + characterData.plusHP * (Level - 1);
    }

    /// <summary>
    /// 공격력 계산 (외부 강화 미포함)
    /// </summary>
    public virtual int GetATK()
    {
        return characterData.baseATK + characterData.plusATK * (Level - 1);
    }

    /// <summary>
    /// 방어력 계산 (외부 강화 미포함)
    /// </summary>
    public virtual int GetDEF()
    {
        return characterData.baseDEF + characterData.plusDEF * (Level - 1);
    }

}
