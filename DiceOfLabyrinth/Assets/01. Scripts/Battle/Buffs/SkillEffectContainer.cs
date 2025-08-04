




//-플레이어
//스텟증감(공격력)
//스텟증감(방어력)
//스텟증감(최대 체력)
//스텟증감(치명타확률)
//스텟증감(치명타 데미지)
//스텟증감(방어력 관통)
//마석 획득량
//최대 코스트 증감
//코스트 획득
//최종 데미지 증가
//주사위 굴리기 횟수 증감
//추가 데미지(아티펙트)
//추가 데미지(각인)
//적에게 피해

//-에너미
//스텟증감(공격력)
//스텟증감(방어력)
//스텟증감(최대 체력)
//스텟증감(치명타확률)
//스텟증감(치명타 데미지)
//스텟증감(방어력 관통)

//-공통
//화상
//보호막
//추가 공격
//디버프 제거
//체력 회복
//속성 추가 데미지
//부활

public class SkillEffectContainer
{
    private float additionalManaStoneRatio;
    private int additionalMaxCost;
    private int additionalCostGain;
    private int additionalRoll;
    private float additionalFinalDamage;
    private float additionalDamageArtifact;
    private float additionalDamageEngraving;

    public void EffectAdditionalManaStone(float value)
    { additionalManaStoneRatio += value; }
    public void EffectAdditionalMaxCost(float value)
    { additionalMaxCost += (int)value; }
    public void EffectAdditionalCostGain(float value)
    { additionalCostGain += (int)value; }
    public void EffectAdditionalRoll(float value)
    { additionalRoll += (int)value; }
    public void EffectAdditionalFinalDamage(float value)
    { additionalFinalDamage += value; }
    public void EffectAdditionalDamageArtifact(float value)
    { additionalDamageArtifact += value; }
    public void EffectAdditionalDamageEngraving(float value)
    { additionalDamageEngraving  += value; }


    public void EffectDealDamage(float value)
    {
        BattleManager.Instance.Enemy.TakeDamage((int)value);
    }

    public void EffectBurn(int targetIndex, int giversAtk, float buffValue, float skillValue)
    {
        float damage = giversAtk * buffValue * skillValue;

        //BattleManager.Instance.PartyData.BattleCharacters[targetIndex].TakeDamage((int)damage);
    }
}
