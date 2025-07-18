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

        //StartCoroutine(MakeLogCoroutine());
    }

    public void MakeBattleLog(string logString)
    {
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
        logText = go.GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = go.GetComponent<RectTransform>();
        contentSizeFitter = go.GetComponent<ContentSizeFitter>();

        yield return new WaitForEndOfFrame();

        width = rectTransform.sizeDelta.x;
        textSize = logText.rectTransform.sizeDelta;
        textSize.x = width;
        logText.rectTransform.sizeDelta = textSize;

        logText.text = logString;
        contentHeight = rectTransform.sizeDelta.y;

        contentSize = content.sizeDelta;
        contentSize.y += (contentHeight + spacing);
        content.sizeDelta = contentSize;

        scrollbar.value = 0;
    }

    public void ResetLog()
    {

    }
}
