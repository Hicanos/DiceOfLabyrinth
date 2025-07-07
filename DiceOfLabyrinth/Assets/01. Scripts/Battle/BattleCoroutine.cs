using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCoroutine : MonoBehaviour
{
    bool isPreparing = false;
    IEnumerator enumeratorSpawn;
    IEnumerator enumeratorAttack;

    GameObject[] characterPrefabs = new GameObject[5];
    BattleManager battleManager;
    [SerializeField] Transform characterContainer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SkipCharacterSpwan();
        }
    }

    public void CharacterSpwan()
    {
        battleManager = BattleManager.Instance;
        enumeratorSpawn = CharacterSpawn();
        StartCoroutine(enumeratorSpawn);
    }

    public void SkipCharacterSpwan()
    {
        if (isPreparing == false) return;
        isPreparing = false;
        StopCoroutine(enumeratorSpawn);

        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        chapterIndex = 0; //임시
        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);

        List<PlayerPositions> positions = StageManager.Instance.chapterData.chapterIndex[chapterIndex].stageData.PlayerFormations[iFormation].PlayerPositions;
        Camera cam = Camera.main;

        for (int i = 0; i < 5; i++)
        {
            characterPrefabs[i].transform.localPosition = positions[i].Position;
            //Vector3 vec2 = cam.WorldToViewportPoint(positions[i].Position + Vector3.up * 50);
            //battleManager.HPBar[i].transform.position = vec2;
            //battleManager.HPBar[i].gameObject.SetActive(true);
        }

        battleManager.BattleStart();
    }

    /// <summary>
    /// 캐릭터가 공격할 때 실행하는 코루틴을 실행하는 메서드입니다.
    /// </summary>    
    public void CharacterAttack(float diceWeighting)
    {
        enumeratorAttack = CharacterAttackCoroutine(diceWeighting);
        StartCoroutine(enumeratorAttack);
    }

    public void StopCharacterAttack()
    {
        StopCoroutine(enumeratorAttack);
    }

    IEnumerator CharacterSpawn()
    {
        isPreparing = true;
        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        chapterIndex = 0; //임시
        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = StageManager.Instance.chapterData.chapterIndex[chapterIndex].stageData.PlayerFormations[iFormation].PlayerPositions;

        float pastTime = 0, destTime = 2.5f;
        GameObject go;
        for (int i = 0; i < 5; i++)
        {
            go = battleManager.battleCharacters[i].CharacterData.charBattlePrefab;
            characterPrefabs[i] = Instantiate(go, positions[i].Position - Vector3.right * 12, Quaternion.identity, characterContainer);
        }
        yield return null;

        while (pastTime < destTime)
        {
            for (int i = 0; i < 5; i++)
            {
                characterPrefabs[i].transform.localPosition = Vector3.Lerp(positions[i].Position - Vector3.right * 12, positions[i].Position, pastTime / destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }

        //for (int i = 0; i < 5; i++)
        //{
        //    battleManager.HPBar[i].transform.localPosition = positions[i].Position + Vector3.up * 5;
        //    battleManager.HPBar[i].gameObject.SetActive(true);
        //}

        isPreparing = false;
        battleManager.BattleStart();
    }

    public IEnumerator CharacterAttackCoroutine(float diceWeighting)
    {
        float pastTime, destTime = 0.5f;

        List<BattleCharacter> battleCharacters = battleManager.battleCharacters;

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
            DealDamage(battleManager.Enemy, damage / 10);
            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(attackPosition, firstPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }

            if (battleManager.TestEnemy.IsDead == true)
            {
                //쓰러지는 애니메이션 있으면 좋을듯
                battleManager.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
                Debug.Log("몬스터 쓰러짐");
                battleManager.BattleEnd();
                StopCharacterAttack();
                //결과창
            }
            yield return null;
        }
        battleManager.stateMachine.ChangeState(battleManager.enemyTurnState);
    }

    public void DealDamage(IDamagable target, int damage)
    {
        target.TakeDamage(damage);
    }
}
