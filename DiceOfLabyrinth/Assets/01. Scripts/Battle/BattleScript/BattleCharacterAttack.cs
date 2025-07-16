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
            enumeratorDamage = DealDamage(battleManager.Enemy, damage);
            StartCoroutine(enumeratorDamage);
            battleManager.Enemy.iEnemy.TakeDamage();            

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
        BattleManager.Instance.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.ConfirmEnd);
    }

    IEnumerator DealDamage(IDamagable target, int damage)
    {
        target.TakeDamage(damage);
        BattleEnemy enemy = (BattleEnemy)target;
        float ratio = (float)enemy.CurrentHP / enemy.MaxHP;

        battleManager.UIValueChanger.ChangeEnemyHpUI(HPEnumEnemy.enemy);

        if (battleManager.Enemy.IsDead)
        {
            isCharacterAttacking = false;
            battleManager.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
            yield return new WaitForSeconds(waitSecondEnemyDie);
            battleManager.EndBattle();
        }
    }
}
