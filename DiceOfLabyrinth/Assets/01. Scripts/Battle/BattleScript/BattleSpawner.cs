using PredictedDice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSpawner : MonoBehaviour
{
    BattleManager battleManager;
    bool isPreparing;
    bool isActive;
    const int numFIve = 5;
    [SerializeField] GameObject defaultDIce;
    [SerializeField] Vector3 spawnDetach;
    [SerializeField] float spawnDestTime;
    public List<FormationVector> formationVec;
    [SerializeField] Vector3 enemyVec;
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
        CharacterSpawn();
        //if (BattleManager.Instance.InBattleStage == false)
        //{
        //    CharacterSpawn();
        //}
        //else
        //{
        //    CharacterActive();
        //}
    }

    private void CharacterSpawn()
    {
        isActive = false;

        battleManager = BattleManager.Instance;
        BattleCharacterInBattle[] characters = battleManager.PartyData.Characters;

        isPreparing = true;

        curFormationVec = formationVec[(int)battleManager.PartyData.CurrentFormationType].formationVec;

        GameObject go;
        SpawnedCharacter spawnedCharacter;

        for (int i = 0; i < numFIve; i++)
        {
            go = Instantiate(characters[i].character.CharacterData.charBattlePrefab, curFormationVec[i] - spawnDetach, Quaternion.identity, characterContainer);

            characters[i].Prefab = go;
            spawnedCharacter = go.GetComponent<SpawnedCharacter>();
            spawnedCharacter.SetCharacterID(characters[i].character.CharacterData.charID);
            
        }
        SpawnDice(characters);
        enumeratorSpawn = CharacterSpawnCoroutine();
        StartCoroutine(enumeratorSpawn);
    }

    //private void CharacterActive()
    //{
    //    isPreparing = true;
    //    isActive = true;

    //    for (int i = 0; i < numFIve; i++)
    //    {
    //        battleManager.PartyData.Characters[i].Prefab.SetActive(true);
    //    }

    //    enumeratorSpawn = CharacterSpawnCoroutine();
    //    StartCoroutine(enumeratorSpawn);
    //}

    //public void CharacterDeActive()
    //{
    //    GameObject go;
    //    for (int i = 0; i < numFIve; i++)
    //    {
    //        go = battleManager.PartyData.Characters[i].Prefab;
    //        go.SetActive(false);
    //        go.transform.localPosition = curFormationVec[i] - spawnDetach;
    //    }
    //}

    public void DestroyCharacters()
    {
        BattleManager battleManager = this.battleManager;

        BattleCharacterInBattle[] characters = battleManager.PartyData.Characters;
        for (int i = 0;i < numFIve;i++)
        {
            Destroy(characters[i].Prefab);
        }
    }

    IEnumerator CharacterSpawnCoroutine()
    {
        BattlePartyData partyData = battleManager.PartyData;

        float pastTime = 0, destTime = spawnDestTime;

        while (pastTime < destTime)
        {
            for (int i = 0; i < numFIve; i++)
            {
                partyData.Characters[i].Prefab.transform.localPosition = Vector3.Lerp(curFormationVec[i] - spawnDetach, curFormationVec[i], pastTime / destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }        
        
        if(!isActive)
        {
            LoadCharacterHP(partyData);
        }
        else
        {
            ActiveCharacterHP(partyData);
        }

        isPreparing = false;
        BattleStart();
    }    

    public void SkipCharacterSpwan()
    {        
        if (isPreparing == false) return;

        BattlePartyData partyData = battleManager.PartyData;
        isPreparing = false;
        StopCoroutine(enumeratorSpawn);

        for (int i = 0; i < numFIve; i++)
        {
            partyData.Characters[i].Prefab.transform.localPosition = curFormationVec[i];
        }

        if (!isActive)
        {
            LoadCharacterHP(partyData);
        }
        else
        {
            ActiveCharacterHP(partyData);
        }

        BattleStart();
    }

    private void BattleStart()
    {
        for (int i = 0; i < numFIve; i++)
        {
            battleManager.PartyData.Characters[i].CharacterHPBars.SetActive(true);
        }
        battleManager.Enemy.EnemyHPBars.SetActive(true);

        battleManager.StateMachine.ChangeState(battleManager.I_PlayerTurnState);
    }

    private void LoadCharacterHP(BattlePartyData partyData)
    {
        battleManager.BattleUIHP.SpawnCharacterHP();
    }

    public void DeactiveCharacterHP(BattlePartyData partyData)
    {
        for (int i = 0; i < numFIve; i++)
        {
            partyData.Characters[i].LayoutGroups.childControlWidth = true;
            partyData.Characters[i].CharacterHPs.gameObject.SetActive(false);
        }
    }

    private void ActiveCharacterHP(BattlePartyData partyData)
    {
        for (int i = 0; i < numFIve; i++)
        {
            partyData.Characters[i].CharacterHPs.gameObject.SetActive(true);
        }
    }

    private void SpawnDice(BattleCharacterInBattle[] characters)
    {
        Debug.Log("스폰 다이스");
        GameObject go;
        GameObject dice;
        GameObject fakeDice;
        
        for(int i = 0; i < numFIve; i++)
        {
            go = characters[i].character.CharacterData.charDicePrefab;
            dice = Instantiate(go, diceContainer);
            fakeDice = Instantiate(go, fakeDiceContainer).gameObject;

            dice.layer = diceLayer;
            DiceManager.Instance.RollDiceSynced.diceAndOutcomeArray[i].dice = dice.GetComponent<Dice>();
            fakeDice.SetActive(false);
            fakeDice.layer = fakeDiceLayer;

            DiceManager.Instance.Dices[i] = dice;
            DiceManager.Instance.FakeDices[i] = fakeDice;
        }

        //UIManager.Instance.BattleUI.FakeDiceHolding.SpawnFakeDices(character);
    }

    public void DestroyDices()
    {
        Debug.Log("주사위 파괴");
        for (int i = diceContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(diceContainer.GetChild(i).gameObject);
            Destroy(fakeDiceContainer.GetChild(i).gameObject);
        }
    }
    public void SpawnEnemy()
    {
        BattleEnemy enemy = battleManager.Enemy;
        GameObject enemyGO = enemy.Data.EnemyPrefab;

        enemy.EnemyPrefab = Instantiate(enemyGO, enemyVec, enemy.Data.EnemySpawnRotation, enemyContainer);
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
