using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PredictedDice;
using System;

public class BattleSpawner : MonoBehaviour
{
    BattleManager battleManager;
    bool isPreparing;
    bool isActive;
    const int numFIve = 5;
    private Vector3 spawnDetach = Vector3.right * 12;
    //private Vector3[] characterDestPos = new Vector3[5];
    [SerializeField] List<FormationVector> formationVec;
    private Vector3[] curFormationVec;

    IEnumerator enumeratorSpawn;

    [SerializeField] Transform characterContainer;
    [SerializeField] Transform enemyContainer;
    [SerializeField] Transform diceContainer;
    [SerializeField] Transform fakeDiceContainer;
    [SerializeField] int diceLayer;
    [SerializeField] int fakeDiceLayer;
    public void SpawnCharacters()
    {
        if (BattleManager.Instance.IsBattle == false)
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
        isActive = false;
        battleManager = BattleManager.Instance;

        List<BattleCharacter> battleCharacters = battleManager.BattleGroup.BattleCharacters;

        isPreparing = true;

        curFormationVec = formationVec[(int)battleManager.BattleGroup.CurrentFormationType].formationVec;

        GameObject go;
        SpawnedCharacter spawnedCharacter;

        for (int i = 0; i < numFIve; i++)
        {
            go = Instantiate(battleCharacters[i].CharacterData.charBattlePrefab, curFormationVec[i] - spawnDetach, Quaternion.identity, characterContainer);

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
        isActive = true;

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
            go.transform.localPosition = curFormationVec[i] - spawnDetach;
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
                battleGroup.CharacterPrefabs[i].transform.localPosition = Vector3.Lerp(curFormationVec[i] - spawnDetach, curFormationVec[i], pastTime / destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }        
        
        if(!isActive)
        {
            LoadCharacterHP(battleGroup);
        }
        else
        {
            ActiveCharacterHP(battleGroup);
        }

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
            battleGroup.CharacterPrefabs[i].transform.localPosition = curFormationVec[i];
        }

        if (!isActive)
        {
            LoadCharacterHP(battleGroup);
        }
        else
        {
            ActiveCharacterHP(battleGroup);
        }

        BattleStart();
    }

    private void BattleStart()
    {
        for (int i = 0; i < numFIve; i++)
        {
            battleManager.BattleGroup.CharacterHPBars[i].SetActive(true);
        }
        battleManager.Enemy.EnemyHPBars.SetActive(true);

        battleManager.StateMachine.ChangeState(battleManager.I_PlayerTurnState);
    }

    private void LoadCharacterHP(BattleCharGroup battleGroup)
    {
        battleManager.BattleUIHP.SpawnCharacterHP();
    }

    public void DeactiveCharacterHP(BattleCharGroup battleGroup)
    {
        for (int i = 0; i < numFIve; i++)
        {
            battleGroup.LayoutGroups[i].childControlWidth = true;
            battleGroup.CharacterHPs[i].gameObject.SetActive(false);
        }
    }

    private void ActiveCharacterHP(BattleCharGroup battleGroup)
    {
        for (int i = 0; i < numFIve; i++)
        {
            battleGroup.CharacterHPs[i].gameObject.SetActive(true);
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

            DiceManager.Instance.Dices[i] = dice;
            DiceManager.Instance.FakeDices[i] = fakeDice;
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
        BattleManager.Instance.BattleUIHP.SpawnMonsterHP();        
    }
}

[Serializable]
public class FormationVector
{
    public Vector3[] formationVec;
}
