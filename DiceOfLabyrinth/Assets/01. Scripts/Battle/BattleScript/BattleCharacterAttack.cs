using UnityEngine;
using System.Collections;

public class BattleCharacterAttack : MonoBehaviour
{
    BattleManager battleManager;
    
    IEnumerator enumeratorAttack;
    IEnumerator enumeratorDamage;

    [SerializeField] Vector3 attackPosition;
    [SerializeField] float charAttackMoveTime;    
    [SerializeField] float waitSecondCharAttack;

    public bool isCharacterAttacking = false;
    /// <summary>
    /// 캐릭터가 공격할 때 실행하는 코루틴을 실행하는 메서드입니다.
    /// </summary>    
    public void CharacterAttack(float diceWeighting)
    {
        isCharacterAttacking = true;
        battleManager = BattleManager.Instance;

        enumeratorAttack = CharacterAttackCoroutine(diceWeighting);
        StartCoroutine(enumeratorAttack);
    }

    private void StopAttackCoroutine()
    {
        StopCoroutine(enumeratorAttack);
    }

    private void StopDamageCoroutine()
    {
        StopCoroutine(enumeratorDamage);
    }

    public IEnumerator CharacterAttackCoroutine(float diceWeighting)
    {
        //float pastTime;
        float destTime = charAttackMoveTime;

        BattleCharacterInBattle[] battleCharacters = battleManager.PartyData.Characters;
        GameObject characterPrefab;
        SpawnedCharacter spawnedCharacter;

        int monsterDef;
        int characterAtk;
        int damage;
        float penetration;
        float elementDamage = 1;

        for (int i = 0; i < battleCharacters.Length; i++)
        {
            if (battleCharacters[i].IsDead) continue;
            if (battleManager.PartyData.DeadIndex.Contains(i)) continue;
            characterPrefab = battleCharacters[i].Prefab;
            spawnedCharacter = characterPrefab.GetComponent<SpawnedCharacter>();

            characterAtk = battleCharacters[i].CurrentATK;
            penetration = battleCharacters[i].CurrentPenetration;
            elementDamage = JudgeElementSuperiority(battleCharacters[i].character.CharacterData, battleManager.Enemy.Data);

            monsterDef = battleManager.Enemy.CurrentDef;
            damage = CalculateDamage(characterAtk, monsterDef, penetration, elementDamage, diceWeighting);

            for (int j = 0; j < battleCharacters[i].AttackCount; j++)
            {
                
                spawnedCharacter.Attack();

                WaitForSeconds waitForSeconds = new WaitForSeconds(destTime);
                yield return waitForSeconds;

                battleManager.Enemy.TakeDamage(damage);
                battleManager.Enemy.iEnemy.TakeDamage();
                UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(battleCharacters[i].CharNameKr, battleManager.Enemy.Data.EnemyName, damage, true);

                yield return waitForSeconds;

                battleCharacters[i].character.UsingSkill = false;

                if (battleManager.Enemy.IsDead)
                {
                    isCharacterAttacking = false;
                    StopAttackCoroutine();
                }
                else if (battleCharacters[i].character.GetBonusAttack)
                {
                    // 추가타 구현
                    battleCharacters[i].character.GetBonusAttack = false;
                    // 스킬 사용여부 초기화는 플레이서 턴 시작 시 자동 제거됨

                    spawnedCharacter.SkillAttack();
                    battleCharacters[i].character.UsingSkill = false;
                    damage = CalculateDamage(characterAtk, monsterDef, penetration, elementDamage, diceWeighting);
                    battleManager.Enemy.TakeDamage(damage);
                    battleManager.Enemy.iEnemy.TakeDamage();
                    UIManager.Instance.BattleUI.BattleUILog.WriteBattleLog(battleCharacters[i].CharNameKr, battleManager.Enemy.Data.EnemyName, damage, true);
                   
                    if (battleManager.Enemy.IsDead)
                    {
                        isCharacterAttacking = false;
                        StopAttackCoroutine();
                    }
                }
                yield return new WaitForSeconds(waitSecondCharAttack);
            }
        }
        isCharacterAttacking = false;
        BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.AttackEnd);
    }    

    private float JudgeElementSuperiority(CharacterSO characterData, EnemyData enemyData)
    {
        float elementDamage = 1;

        if (characterData.elementType == DesignEnums.ElementTypes.Water)
        {
            if (enemyData.Attribute == DesignEnums.ElementTypes.Fire)
            {
                elementDamage = characterData.elementDMG;
            }
        }
        else
        {
            if ((int)characterData.elementType > (int)enemyData.Attribute)
            {
                elementDamage = characterData.elementDMG;
            }
        }
        return elementDamage;
    }

    private int CalculateDamage(int characterAtk, int monsterDef, float penetration, float elementDamage, float diceWeighting)
    {
        float engravingAddAtk = battleManager.EngravingAdditionalStatus.AdditionalDamage;
        float additionalElementDamage = battleManager.ArtifactAdditionalStatus.AdditionalElementDamage;
        float artifactAddAtk = battleManager.ArtifactAdditionalStatus.AdditionalDamage;

        //공격력 * [100/{방어력 * (1-방어력 관통률) +100}] * (1 + 버프 + 아티팩트 + 속성 + 패시브) * 족보별 계수 * 각인 계수
        float damage = characterAtk * (100/ (monsterDef * (1- penetration) + 100)) * (1 + artifactAddAtk + elementDamage + additionalElementDamage) * ((int)diceWeighting * engravingAddAtk);
        Debug.Log($"{characterAtk} * (100 / {monsterDef} * (1 - {penetration} + 100)) * (1 + {artifactAddAtk} + {elementDamage} + {additionalElementDamage}) * ({(int)diceWeighting} * {engravingAddAtk})\nEngrving :  + {engravingAddAtk}\nArtifact :  + {artifactAddAtk}\nElement :  + {additionalElementDamage}");
        damage = Mathf.Clamp(damage, 0, damage);

        if (TutorialManager.Instance.isGameTutorialCompleted == false) damage = 20;
        return (int)damage;
    }
}
