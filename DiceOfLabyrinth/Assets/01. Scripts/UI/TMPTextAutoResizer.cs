using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TMPTextAutoResizer : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private RectTransform targetRect;
    [SerializeField] private float padding = 20f;

    private void Awake()
    {
        if (targetRect == null)
        {
            targetRect = GetComponent<RectTransform>();
        }

        if (targetText == null)
        {
            targetText = GetComponent<TMP_Text>();
        }
    }

    private void LateUpdate()
    {
        AdjustHeight();
    }

    private void AdjustHeight()
    {
        if (targetText == null || targetRect == null)
        {
            return;
        }

        float textHeight = targetText.preferredHeight;

        Vector2 size = targetRect.sizeDelta;
        size.y = textHeight + padding;
        targetRect.sizeDelta = size;
    }
}
