using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleCoroutine : MonoBehaviour
{
    bool isPreparing = false;
    IEnumerator enumerator;

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

        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = StageManager.Instance.stageData.PlayerFormations[iFormation].PlayerPositions;

        for (int i = 0; i < 5; i++)
        {
            BattleManager.Instance.entryCharacters[i].transform.localPosition = positions[i].Position;
        }

        BattleManager.Instance.BattleStart();
    }

    IEnumerator BattlePrepare()
    {
        isPreparing = true;

        int iFormation = ((int)StageManager.Instance.stageSaveData.currentFormationType);
        List<PlayerPositions> positions = StageManager.Instance.stageData.PlayerFormations[iFormation].PlayerPositions;

        float pastTime = 0, destTime = 2.5f;

        while (pastTime < destTime)
        {
            for (int i = 0; i < 5; i++)
            {
                BattleManager.Instance.entryCharacters[i].transform.localPosition = Vector3.Lerp(positions[i].Position - Vector3.right * 12, positions[i].Position, pastTime/destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }

        isPreparing = false;
        Debug.Log("작동");
        BattleManager.Instance.BattleStart();
    }
}
