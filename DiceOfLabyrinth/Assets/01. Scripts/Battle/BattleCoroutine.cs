using UnityEngine;
using System.Collections;

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

        Vector3[] formation = BattleManager.Instance.tempFormation;
        for (int i = 0; i < 5; i++)
        {
            BattleManager.Instance.entryCharacters[i].gameObject.transform.localPosition = formation[i];
        }

        BattleManager.Instance.BattleStart();
    }

    IEnumerator BattlePrepare()
    {
        isPreparing = true;
        Vector3[] formation = BattleManager.Instance.tempFormation;
        float pastTime = 0, destTime = 2.5f;

        while (pastTime < destTime)
        {
            for (int i = 0; i < 5; i++)
            {
                BattleManager.Instance.entryCharacters[i].gameObject.transform.localPosition = Vector3.Lerp(formation[i] - Vector3.right * 12, formation[i], pastTime/destTime);
            }

            pastTime += Time.deltaTime;
            yield return null;
        }

        isPreparing = false;
        Debug.Log("작동");
        BattleManager.Instance.BattleStart();
    }
}
