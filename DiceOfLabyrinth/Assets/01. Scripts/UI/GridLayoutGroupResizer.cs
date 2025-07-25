using UnityEngine;
using UnityEngine.UI;

public class GridLayoutGroupResizer : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup grid; // 카드 정렬에 쓰이는 GridLayoutGroup
    [SerializeField] private RectTransform rect; // 그룹 자체의 RectTransform

    private void Awake()
    {
        // 같은 오브젝트에 붙어있는 GridLayoutGroup, RectTransform 자동참조
        grid = GetComponent<GridLayoutGroup>();
        rect = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        UpdateHeight();
    }

    private void UpdateHeight()
    {
        // 자식 개수
        int childCount = 0;
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (grid.transform.GetChild(i).gameObject.activeSelf)
            {
                childCount++;
            }
        }

        // GridLayoutGroup 설정 기반으로 필요한 행(Row) 개수 계산
        int columns = grid.constraintCount; // 가로로 몇 개씩 배치되는지
        int rows = Mathf.CeilToInt((float)childCount / columns);

        // 계산된 높이: 패딩 + (셀 높이 * 행 수) + (간격 * (행 - 1))
        float height = grid.padding.top + grid.padding.bottom +
                       rows * grid.cellSize.y +
                       (rows - 1) * grid.spacing.y;

        // 실제 RectTransform 높이 적용
        Vector2 size = rect.sizeDelta;
        size.y = height;
        rect.sizeDelta = size;

        // Content 갱신
        Transform parentContent = rect.parent;
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentContent as RectTransform);
    }
}
