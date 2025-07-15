using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCharacterAttack : MonoBehaviour
{
    BattleManager battleManager;
    
    IEnumerator enumeratorAttack;    

    /// <summary>
    /// 캐릭터가 공격할 때 실행하는 코루틴을 실행하는 메서드입니다.
    /// </summary>    
    public void CharacterAttack(float diceWeighting)
    {
        battleManager = BattleManager.Instance;
        enumeratorAttack = CharacterAttackCoroutine(diceWeighting);
        StartCoroutine(enumeratorAttack);
    }

    public void StopCharacterAttack()
    {
        StopCoroutine(enumeratorAttack);
    }

    public IEnumerator CharacterAttackCoroutine(float diceWeighting)
    {
        float pastTime, destTime = 0.5f;

        List<BattleCharacter> battleCharacters = battleManager.BattleGroup.BattleCharacters;
        GameObject[] characterPrefabs = battleManager.BattleGroup.CharacterPrefabs;

        int monsterDef = battleManager.Enemy.Data.Def;
        Vector3 attackPosition = new Vector3(3.25f, 0, 0);

        for (int i = 0; i < battleCharacters.Count; i++)
        {
            int characterAtk = battleCharacters[i].CurrentATK;
            int damage = (characterAtk - monsterDef) * (int)diceWeighting;
            damage = Mathf.Clamp(damage, 0, damage);

            Vector3 firstPosition = characterPrefabs[i].transform.position;

            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(firstPosition, attackPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }
            DealDamage(battleManager.Enemy, damage);
            battleManager.Enemy.iEnemy.TakeDamage();
            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(attackPosition, firstPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }

            if (battleManager.Enemy.IsDead)
            {
                battleManager.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
                battleManager.isWon = true;
                battleManager.EndBattle();
                StopCharacterAttack();
            }
            yield return null;
        }
        BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.ConfirmEnd);
    }

    public void DealDamage(IDamagable target, int damage)
    {
        target.TakeDamage(damage);
        BattleEnemy enemy = (BattleEnemy)target;
        float ratio = (float)enemy.CurrentHP / enemy.MaxHP;

        battleManager.UIValueChanger.ChangeEnemyHpUI(HPEnumEnemy.enemy);
    }
}
