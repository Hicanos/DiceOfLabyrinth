using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

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

    public void GetMonsterPattern()
    {
        StartCoroutine(BlinkUI(BattleManager.Instance.patternDisplay));
    }

    IEnumerator BlinkUI(Image image, float time = 1f)
    {
        Color color = image.color;
        TextMeshProUGUI text = image.transform.GetComponentInChildren<TextMeshProUGUI>();
        Color textColor = text.color;

        for (float f = 1; f > 0.25f; f -= Time.deltaTime / time)
        {
            color.a = f;
            textColor.a = f;
            image.color = color;
            text.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);
        BattleManager.Instance.LoadMonsterPattern.PrepareSkill();
        yield return new WaitForSeconds(0.15f);

        for(float f = 0.25f; f <= 1; f += Time.deltaTime / time)
        {
            color.a = f;
            textColor.a = f;
            image.color = color;
            text.color = textColor;
            yield return null;
        }
    }
}
