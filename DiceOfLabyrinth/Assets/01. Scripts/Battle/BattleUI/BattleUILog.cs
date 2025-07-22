using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BattleUILog : MonoBehaviour
{
    [SerializeField] float padding;
    [SerializeField] float spacing;
    [SerializeField] RectTransform content;
    [SerializeField] Scrollbar scrollbar;
    float contentHeight;

    public void Start()
    {
        Vector2 size = content.sizeDelta;
        size.y += padding * 2;
        content.sizeDelta = size;
    }

    public void MakeBattleLog(bool isCharacterTurn)
    {
        if(isCharacterTurn)
        {
            StartCoroutine(MakeLogCoroutine("플레이어 턴"));
        }
        else
        {
            StartCoroutine(MakeLogCoroutine("에너미 턴"));
        }
    }

    public void MakeBattleLog(string logSubject, string logObject, int damage, bool isCharacterAttack)
    {
        string logString = MakeLogString(logSubject, logObject, damage, isCharacterAttack);
        StartCoroutine(MakeLogCoroutine(logString));
    }

    IEnumerator MakeLogCoroutine(string logString = null)
    {
        float width;
        TextMeshProUGUI logText;
        GameObject go;
        RectTransform rectTransform;
        Vector2 contentSize;
        Vector2 textSize;
        ContentSizeFitter contentSizeFitter;

        go = Instantiate(UIManager.Instance.BattleUI.BattleLogPrefab, content);
        rectTransform = go.GetComponent<RectTransform>();
        contentSizeFitter = go.GetComponent<ContentSizeFitter>();
        logText = go.GetComponentInChildren<TextMeshProUGUI>();
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

    public void ResetLog()
    {

    }
}
