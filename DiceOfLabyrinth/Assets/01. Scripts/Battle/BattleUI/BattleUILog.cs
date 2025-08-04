using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BattleUILog : MonoBehaviour
{
    [SerializeField] float padding;
    [SerializeField] float spacing;
    [SerializeField] RectTransform content;
    [SerializeField] Scrollbar scrollbar;
    [SerializeField] List<GameObject> logs;
    [SerializeField] int numOfInitialLogs;

    private int currentLogIndex;
    private int maxLogIndex;
    private float contentHeight;
    private bool isWriting;
    private List<string> stashedLogs = new List<string>();

    private IEnumerator writeLogCoroutine;

    public void Start()
    {
        Vector2 size = content.sizeDelta;
        size.y += padding * 2;
        content.sizeDelta = size;
        maxLogIndex = numOfInitialLogs;
    }

    public void MakeLogPool()
    {
        GameObject go;
        for (int i = 0; i < numOfInitialLogs; i++)
        {
            go = Instantiate(UIManager.Instance.BattleUI.BattleLogPrefab, content);

            go.SetActive(false);
            logs.Add(go);
        }
    }

    public void WriteBattleLog(bool isCharacterTurn)
    {
        if (currentLogIndex == maxLogIndex)
        {
            MakeNewLog();
        }

        if (isCharacterTurn)
        {
            writeLogCoroutine = WriteLogCoroutine("플레이어 턴");
            StartCoroutine(writeLogCoroutine);
        }
        else
        {
            writeLogCoroutine = WriteLogCoroutine("에너미 턴");
            StartCoroutine(writeLogCoroutine);
        }
    }

    public void WriteBattleLog(string logSubject, string logObject, int damage, bool isCharacterAttack)
    {
        if (currentLogIndex == maxLogIndex)
        {
            MakeNewLog();
        }

        string logString = MakeLogString(logSubject, logObject, damage, isCharacterAttack);

        if(isWriting == true)
        {
            stashedLogs.Add(logString);
            return;
        }

        writeLogCoroutine = WriteLogCoroutine(logString);
        StartCoroutine(writeLogCoroutine);
    }

    private string MakeLogString(string logSubject, string logObject, int damage, bool isCharacterAttack)
    {
        string logString;

        if (isCharacterAttack)
        {
            logString = $"<color=green>{logSubject}</color> : <color=red>{logObject}</color>에게 데미지 <color=yellow>{damage}</color>";
        }
        else
        {
            logString = $"<color=red>{logSubject}</color> : <color=green>{logObject}</color>에게 데미지 <color=yellow>{damage}</color>";
        }
        
        return logString;
    }    

    IEnumerator WriteLogCoroutine(string logString = null)
    {
        isWriting = true;
        float width;
        GameObject log;
        TextMeshProUGUI logText;
        RectTransform rectTransform;
        Vector2 contentSize;
        Vector2 textSize;
        ContentSizeFitter contentSizeFitter;

        log = logs[currentLogIndex];
        log.SetActive(true);
        rectTransform = log.GetComponent<RectTransform>();
        contentSizeFitter = log.GetComponent<ContentSizeFitter>();
        logText = log.GetComponentInChildren<TextMeshProUGUI>();
        logText.richText = true;

        yield return new WaitForEndOfFrame();

        width = rectTransform.sizeDelta.x;
        textSize = logText.rectTransform.sizeDelta;
        textSize.x = width;
        logText.rectTransform.sizeDelta = textSize;

        logText.text = logString;
        yield return new WaitForEndOfFrame();
        contentHeight = rectTransform.sizeDelta.y;

        contentSize = content.sizeDelta;
        contentSize.y += (contentHeight + spacing);
        content.sizeDelta = contentSize;

        scrollbar.value = 0;
        currentLogIndex++;
        isWriting = false;
        CheckStashedLog();
    }

    private void MakeNewLog()
    {
        GameObject go;
        go = Instantiate(UIManager.Instance.BattleUI.BattleLogPrefab, content);

        go.SetActive(false);
        logs.Add(go);
        maxLogIndex++;
    }

    public void TurnOffAllLogs()
    {
        currentLogIndex = 0;
        Vector2 contentSize;
        contentSize = content.sizeDelta;
        contentSize.y = 0;
        content.sizeDelta = contentSize;
        foreach (GameObject go in logs)
        {
            go.SetActive(false);
        }
    }

    private void CheckStashedLog()
    {
        if(stashedLogs.Count > 0)
        {
            StopCoroutine(writeLogCoroutine);

            writeLogCoroutine = WriteLogCoroutine(stashedLogs[0]);
            stashedLogs.RemoveAt(0);
            StartCoroutine(writeLogCoroutine);
        }
    }
}
