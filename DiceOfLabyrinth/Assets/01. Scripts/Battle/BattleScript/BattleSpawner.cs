using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using PredictedDice;

public class BattleSpawner : MonoBehaviour
{
    BattleManager battleManager;
    bool isPreparing = false;
    const int numFIve = 5;
    private Vector3 spawnDetach = Vector3.right * 12;
    private Vector3[] characterDestPos = new Vector3[5];

    IEnumerator enumeratorSpawn;

    [SerializeField] Transform characterContainer;
    [SerializeField] Transform enemyContainer;
    [SerializeField] Transform diceContainer;
    [SerializeField] Transform fakeDiceContainer;
    [SerializeField] int diceLayer;
    [SerializeField] int fakeDiceLayer;
    public void SpawnCharacters()
    {
        if (StageManager.Instance.stageSaveData.currentPhaseIndex == 1)
        {
            CharacterSpawn();
        }
        else
        {
            CharacterActive();
        }
    }

    private void CharacterSpawn()
    {
        battleManager = BattleManager.Instance;
        StageManager stageManager = StageManager.Instance;
        List<BattleCharacter> battleCharacters = battleManager.BattleGroup.BattleCharacters;

        isPreparing = true;
        int chapterIndex = stageManager.stageSaveData.currentChapterIndex;
        int iFormation = ((int)stageManager.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = stageManager.chapterData.chapterIndex[chapterIndex].stageData.PlayerFormations[iFormation].PlayerPositions;

        GameObject go;
        SpawnedCharacter spawnedCharacter;

        for (int i = 0; i < numFIve; i++)
        {
            go = Instantiate(battleCharacters[i].CharacterData.charBattlePrefab, characterDestPos[i] - spawnDetach, Quaternion.identity, characterContainer);
            characterDestPos[i] = positions[i].Position;

            battleManager.BattleGroup.CharacterPrefabs[i] = go;
            spawnedCharacter = go.GetComponent<SpawnedCharacter>();
            spawnedCharacter.SetCharacterID(battleCharacters[i].CharacterData.charID);
        }
        SpawnDice(battleCharacters);
        enumeratorSpawn = CharacterSpawnCoroutine();
        StartCoroutine(enumeratorSpawn);
    }

    private void CharacterActive()
    {
        isPreparing = true;
        
        for (int i = 0; i < numFIve; i++)
        {
            battleManager.BattleGroup.CharacterPrefabs[i].SetActive(true);
        }

        enumeratorSpawn = CharacterSpawnCoroutine();
        StartCoroutine(enumeratorSpawn);
    }

    public void CharacterDeActive()
    {
        GameObject go;
        for (int i = 0; i < numFIve; i++)
        {
            go = battleManager.BattleGroup.CharacterPrefabs[i];
            go.SetActive(false);
            go.transform.localPosition = characterDestPos[i] - spawnDetach;
        }
    }

    IEnumerator CharacterSpawnCoroutine()
    {
        BattleCharGroup battleGroup = battleManager.BattleGroup;

        float pastTime = 0, destTime = 2.5f;

        while (pastTime < destTime)
        {
            for (int i = 0; i < numFIve; i++)
            {
                battleGroup.CharacterPrefabs[i].transform.localPosition = Vector3.Lerp(characterDestPos[i] - spawnDetach, characterDestPos[i], pastTime / destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }        
        
        LoadCharacterHP(battleGroup);
        
        isPreparing = false;
        BattleStart();
    }    

    public void SkipCharacterSpwan()
    {        
        if (isPreparing == false) return;

        BattleCharGroup battleGroup = battleManager.BattleGroup;
        isPreparing = false;
        StopCoroutine(enumeratorSpawn);

        for (int i = 0; i < numFIve; i++)
        {
            battleGroup.CharacterPrefabs[i].transform.localPosition = characterDestPos[i];
        }
        LoadCharacterHP(battleGroup);

        BattleStart();
    }

    private void BattleStart()
    {
        for (int i = 0; i < numFIve; i++)
        {
            battleManager.BattleGroup.CharacterHPBars[i].SetActive(true);
        }
        battleManager.Enemy.EnemyHPBar.SetActive(true);

        battleManager.I_PlayerTurnState.Enter();
    }

    private void LoadCharacterHP(BattleCharGroup battleGroup)
    {
        GameObject go;
        for (int i = 0; i < numFIve; i++)
        {
            go = Instantiate(battleManager.CharacterHPPrefab, battleGroup.CharacterPrefabs[i].transform);
            battleGroup.CharacterHPBars[i] = go;

            battleGroup.CharacterHPs[i] = go.GetComponentsInChildren<RectTransform>()[3];
            battleGroup.CharacterHPTexts[i] = go.GetComponentInChildren<TextMeshProUGUI>();
            battleManager.UIValueChanger.ChangeCharacterHpRatio((HPEnumCharacter)i);
        }
    }

    private void SpawnDice(List<BattleCharacter> character)
    {
        GameObject go;
        GameObject dice;
        GameObject fakeDice;
        for (int i = 0; i < numFIve; i++)
        {
            go = character[i].CharacterData.charDicePrefab;
            dice = Instantiate(go, diceContainer);
            fakeDice = Instantiate(go, fakeDiceContainer).gameObject;

            dice.layer = diceLayer;
            DiceManager.Instance.RollDiceSynced.diceAndOutcomeArray[i].dice = dice.GetComponent<Dice>();
            fakeDice.SetActive(false);
            fakeDice.layer = fakeDiceLayer;
        }        
    }

    public void SpawnEnemy()
    {
        BattleEnemy enemy = battleManager.Enemy;
        GameObject enemyGO = enemy.Data.EnemyPrefab;

        enemy.EnemyPrefab = Instantiate(enemyGO, new Vector3(3, -1, -4), enemy.Data.EnemySpawnRotation, enemyContainer);
        enemy.iEnemy = enemy.EnemyPrefab.GetComponent<IEnemy>();
        enemy.iEnemy.Init();

        LoadEnemyHP(enemy);
    }

    private void LoadEnemyHP(BattleEnemy enemy)
    {
        GameObject go = Instantiate(battleManager.EnemyHPPrefab, enemy.EnemyPrefab.transform);
        RectTransform rect = go.GetComponent<RectTransform>();

        Quaternion quaternion = Quaternion.identity;
        quaternion = Quaternion.Euler(0, -enemy.Data.EnemySpawnRotation.y, 0);

        rect.rotation = quaternion;

        enemy.EnemyHPBar = go;
        enemy.EnemyHP = go.GetComponentsInChildren<RectTransform>()[2];
        enemy.EnemyHPText = go.GetComponentInChildren<TextMeshProUGUI>();
        battleManager.UIValueChanger.ChangeEnemyHpUI(HPEnumEnemy.enemy);
        battleManager.BattleUIHP.GetEnmeyHPRotation(enemy);
    }
}
