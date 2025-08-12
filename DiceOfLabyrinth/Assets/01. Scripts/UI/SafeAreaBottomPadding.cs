using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class SafeAreaBottomPadding : MonoBehaviour
{
    [SerializeField] bool useLayoutGroupPadding = false;
    [SerializeField] float extraBottomMargin = 24f;

    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;
    private RectTransform rectTransform;
    private HorizontalOrVerticalLayoutGroup layoutGroup;
    private Canvas rootCanvas;

    void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        layoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
        ApplyBottomPadding();
    }

    void Update()
    {
        Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);
        Rect currentSafeArea = Screen.safeArea;

        if (currentScreenSize != lastScreenSize || currentSafeArea != lastSafeArea)
        {
            ApplyBottomPadding();
        }
    }

    void ApplyBottomPadding()
    {
        if (rectTransform == null)
        {
            return;
        }

        Rect safeArea = Screen.safeArea;
        float canvasScale = (rootCanvas ? rootCanvas.scaleFactor : 1f);
        float bottomInsetUnits = safeArea.yMin / canvasScale + extraBottomMargin;

        if (useLayoutGroupPadding && layoutGroup != null)
        {
            RectOffset padding = layoutGroup.padding;
            padding.bottom = Mathf.RoundToInt(bottomInsetUnits);
            layoutGroup.padding = padding;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        else
        {
            Vector2 offsetMin = rectTransform.offsetMin;
            offsetMin.y = bottomInsetUnits;
            rectTransform.offsetMin = offsetMin;
        }

        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        lastSafeArea = safeArea;
    }
}
