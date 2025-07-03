using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCoroutine : MonoBehaviour
{
    bool isPreparing = false;
    IEnumerator enumerator;
    GameObject[] characterGOs = new GameObject[5];

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StopPrepareBattle();
        }
    }

    public void StartPrepareBattle()
    {
        enumerator = BattlePrepare();
        StartCoroutine(enumerator);
    }

    public void StopPrepareBattle()
    {
        if (isPreparing == false) return;
        isPreparing = false;
        StopCoroutine(enumerator);

        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        chapterIndex = 0; //임시
        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = StageManager.Instance.chapterManager.chapterData.chapterIndex[chapterIndex].stageData.PlayerFormations[iFormation].PlayerPositions;

        for (int i = 0; i < 5; i++)
        {
            BattleManager.Instance.characterPrefabs[i].transform.localPosition = positions[i].Position;
        }

        BattleManager.Instance.BattleStart();
    }

    IEnumerator BattlePrepare()
    {
        isPreparing = true;
        int chapterIndex = StageManager.Instance.stageSaveData.currentChapterIndex;
        chapterIndex = 0; //임시
        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = StageManager.Instance.chapterManager.chapterData.chapterIndex[chapterIndex].stageData.PlayerFormations[iFormation].PlayerPositions;

        float pastTime = 0, destTime = 2.5f;

        for(int i = 0; i < 5; i++)
        {
            characterGOs[i] = BattleManager.Instance.battleCharacters[i].CharacterData.charBattlePrefab;
            BattleManager.Instance.characterPrefabs[i] = Instantiate(characterGOs[i], positions[i].Position - Vector3.right* 12, Quaternion.identity, BattleManager.Instance.characterContainer);
        }
        yield return null;

        while (pastTime < destTime)
        {
            for (int i = 0; i < 5; i++)
            {
                BattleManager.Instance.characterPrefabs[i].transform.localPosition = Vector3.Lerp(positions[i].Position - Vector3.right * 12, positions[i].Position, pastTime/destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }

        isPreparing = false;        
        BattleManager.Instance.BattleStart();
    }
}
