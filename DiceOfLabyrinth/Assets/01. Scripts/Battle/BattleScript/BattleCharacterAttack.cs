using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCharacterAttack : MonoBehaviour
{
    BattleManager battleManager;
    
    IEnumerator enumeratorAttack;
    IEnumerator enumeratorDamage;

    [SerializeField] Vector3 attackPosition;
    [SerializeField] float charAttackMoveTime;
    [SerializeField] float waitSecondEnemyDie;
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
        float pastTime, destTime = charAttackMoveTime;

        List<BattleCharacter> battleCharacters = battleManager.BattleGroup.BattleCharacters;
        GameObject[] characterPrefabs = battleManager.BattleGroup.CharacterPrefabs;

        int monsterDef = battleManager.Enemy.Data.Def;
        int characterAtk;
        int damage;
        float penetration;
        float elementDamage = 1;

        for (int i = 0; i < battleCharacters.Count; i++)
        {
            if (battleCharacters[i].IsDied) continue;

            characterAtk = battleCharacters[i].CurrentATK;
            penetration = battleCharacters[i].Penetration;
            elementDamage = JudgeElementSuperiority(battleCharacters[i].CharacterData, battleManager.Enemy.Data);

            damage = CalculateDamage(characterAtk, monsterDef, penetration, elementDamage, diceWeighting);

            Vector3 firstPosition = characterPrefabs[i].transform.position;

            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(firstPosition, attackPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }

            enumeratorDamage = DealDamage(battleManager.Enemy, damage);
            StartCoroutine(enumeratorDamage);
            battleManager.Enemy.iEnemy.TakeDamage();
            UIManager.Instance.BattleUI.BattleUILog.MakeBattleLog(battleCharacters[i].CharNameKr, battleManager.Enemy.Data.EnemyName, damage, true);

            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(attackPosition, firstPosition, pastTime / destTime);

                pastTime += Time.deltaTime;

                yield return null;
            }
            if (battleManager.Enemy.IsDead) StopAttackCoroutine();

            yield return new WaitForSeconds(waitSecondCharAttack);
        }
        isCharacterAttacking = false;
        BattleManager.Instance.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.AttackEnd);
    }

    IEnumerator DealDamage(IDamagable target, int damage)
    {
        target.TakeDamage(damage);
        BattleEnemy enemy = (BattleEnemy)target;
        float ratio = (float)enemy.CurrentHP / enemy.MaxHP;

        if (battleManager.Enemy.IsDead)
        {
            isCharacterAttacking = false;
            battleManager.BattlePlayerTurnState.ChangeDetailedTurnState(DetailedTurnState.BattleEnd);
            yield return new WaitForSeconds(waitSecondEnemyDie);
            battleManager.EndBattle();
        }
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
        //{공격력 - 방어력 * (1-방어력 관통률)} * (1 + 버프 + 아티팩트 + 속성 + 패시브) * (족보별 계수 * 각인 계수)
        float engravingAddAtk = battleManager.EngravingAdditionalStatus.AdditionalStatus[(int)EffectTypeEnum.AdditionalDamage];
        float additionalElementDamage = battleManager.ArtifactAdditionalStatus.AdditionalStatus[(int)AdditionalStatusEnum.AdditionalElementDamage];
        float artifactAddAtk = battleManager.ArtifactAdditionalStatus.AdditionalStatus[(int)AdditionalStatusEnum.AdditionalDamage];

        float damage = (characterAtk - monsterDef * (1- penetration)) * (1 + artifactAddAtk + elementDamage + additionalElementDamage) * ((int)diceWeighting * engravingAddAtk);
        Debug.Log($"Engrving :  + {engravingAddAtk}\nArtifact :  + {artifactAddAtk}\nElement :  + {additionalElementDamage}");
        damage = Mathf.Clamp(damage, 0, damage);
        return (int)damage;
    }
}
