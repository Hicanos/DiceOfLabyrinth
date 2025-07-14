using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BattleCoroutine : MonoBehaviour
{
    BattleManager battleManager;

    bool isPreparing = false;
    IEnumerator enumeratorSpawn;
    IEnumerator enumeratorAttack;
    private Vector3 spawnDetach = Vector3.right * 12;
    private Vector3[] characterDestPos = new Vector3[5];
    
    [SerializeField] Transform characterContainer;
    //[SerializeField] Transform ContainerForSelect;
    private GameObject[] characterPrefabs = new GameObject[5];

    public void CharacterSpawn()
    {
        battleManager = BattleManager.Instance;

        isPreparing = true;
        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = StageManager.Instance.chapterData.chapterIndex[chapterIndex].stageData.PlayerFormations[iFormation].PlayerPositions;

        GameObject go;
        SpawnedCharacter spawnedCharacter;

        for (int i = 0; i < 5; i++)
        {
            go = battleManager.battleCharacters[i].CharacterData.charBattlePrefab;
            characterDestPos[i] = positions[i].Position;

            characterPrefabs[i] = Instantiate(go, characterDestPos[i] - spawnDetach, Quaternion.identity, characterContainer);
            spawnedCharacter = characterPrefabs[i].GetComponent<SpawnedCharacter>();
            spawnedCharacter.SetCharacterID(battleManager.battleCharacters[i].CharacterData.charID);
        }

        enumeratorSpawn = CharacterSpawnCoroutine();
        StartCoroutine(enumeratorSpawn);
    }

    public void CharacterActive()
    {
        isPreparing = true;
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            characterPrefabs[i].SetActive(true);
        }

        enumeratorSpawn = CharacterSpawnCoroutine();
        StartCoroutine(enumeratorSpawn);
    }

    public void CharacterDeActive()
    {
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            characterPrefabs[i].SetActive(false);
            characterPrefabs[i].transform.localPosition = characterDestPos[i] - spawnDetach;
        }
    }

    IEnumerator CharacterSpawnCoroutine()
    {
        float pastTime = 0, destTime = 2.5f;

        while (pastTime < destTime)
        {
            for (int i = 0; i < 5; i++)
            {
                characterPrefabs[i].transform.localPosition = Vector3.Lerp(characterDestPos[i] - spawnDetach, characterDestPos[i], pastTime / destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }
        GameObject go;
        //Camera cam = DiceManager.Instance.diceCamera;
        for (int i = 0; i < 5; i++)
        {
            //Vector3 vec2 = cam.WorldToScreenPoint(characterPrefabs[i].transform.position);
            //PositionCharacterHPBars(i, vec2);
            //battleManager.UIValueChanger.ChangeCharacterHpRatio((HPEnumCharacter)i);
            go = battleManager.CharacterHPPrefab;
            battleManager.CharacterHPBars[i] = Instantiate(go, characterPrefabs[i].transform);            
        }
        battleManager.GetUIs();

        isPreparing = false;
        battleManager.BattleStart();
    }

    public void SkipCharacterSpwan(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (isPreparing == false) return;
        isPreparing = false;
        StopCoroutine(enumeratorSpawn);

        GameObject go;
        Camera cam = DiceManager.Instance.diceCamera;
        for (int i = 0; i < 5; i++)
        {
            characterPrefabs[i].transform.localPosition = characterDestPos[i];

            //Vector3 vec2 = cam.WorldToScreenPoint(characterPrefabs[i].transform.position);
            //PositionCharacterHPBars(i, vec2);
            //battleManager.UIValueChanger.ChangeCharacterHpRatio((HPEnumCharacter)i);
            go = battleManager.CharacterHPPrefab;
            battleManager.CharacterHPBars[i] = Instantiate(go, characterPrefabs[i].transform);
        }
        battleManager.GetUIs();
    
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
            DealDamage(battleManager.Enemy, damage);
            battleManager.iEnemy.TakeDamage();
            pastTime = 0;
            while (pastTime < destTime)
            {
                characterPrefabs[i].transform.position = Vector3.Lerp(attackPosition, firstPosition, pastTime / destTime);

                pastTime += Time.deltaTime;
                yield return null;
            }

            if (battleManager.Enemy.IsDead)
            {
                //쓰러지는 애니메이션 있으면 좋을듯
                battleManager.battlePlayerTurnState.ChangePlayerTurnState(PlayerTurnState.BattleEnd);
                battleManager.isWon = true;
                battleManager.BattleEnd();
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

        battleManager.UIValueChanger.ChangeEnemyHpRatio(HPEnumEnemy.enemy);
    }

    //public void SpawnInDungeonSelect()
    //{
    //    int curPhaseIndex = StageManager.Instance.stageSaveData.currentPhaseIndex;
    //    battleManager = BattleManager.Instance;

    //    if (curPhaseIndex == 0)
    //    {
    //        GameObject go;
    //        int count = 0;
    //        Vector3[] pos = new Vector3[4];

    //        for (int i = 0; i < 5; i++)
    //        {
    //            go = StageManager.Instance.stageSaveData.battleCharacters[i].CharacterData.charBattlePrefab;
    //            if (i == 0) //리더인덱스로 수정
    //            {
    //                Vector3 leaderPos;
    //                RectTransform rectTransform = battleManager.Stages.transform.GetChild(curPhaseIndex).GetComponent<RectTransform>();
    //                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
    //                RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPos, Camera.main, out leaderPos);

    //                //Vector3 leaderPos = battleManager.Stages.transform.GetChild(curPhaseIndex).transform.position;
    //                leaderPos = Camera.main.ScreenToWorldPoint(leaderPos);
    //                prefabsForSelect[i] = Instantiate(go, leaderPos, Quaternion.identity, ContainerForSelect);
    //            }
    //            else
    //            {
    //                prefabsForSelect[i] = Instantiate(go, pos[count], Quaternion.identity, ContainerForSelect);
    //                count++;
    //            }                
    //        }
    //    }
    //    else
    //    {
    //        CharacterActive(false);
    //    }
    //}

    //IEnumerator MoveInDungeonSelect()
    //{
    //    float pastTime = 0, destTime = 0.5f;
    //    Vector3 startPosition = characterPrefabs[0].transform.position;
    //    Vector3 destPosition = Vector3.zero;

    //    while(pastTime < destTime)
    //    {
    //        characterPrefabs[0].transform.position = Vector3.Lerp(startPosition, destPosition, pastTime / destTime);

    //        pastTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    StageManager.Instance.battleUIController.OnClickStageNextButton();
    //}
}
