using DG.Tweening;
using UnityEngine;

public class StagePanel : MonoBehaviour
{
    [SerializeField] private RectTransform[] flags; // 깃발 오브젝트
    [SerializeField] private float targetY = 110f; // 깃발이 올라올 목표 y값
    [SerializeField] private float duration = 0.5f; // 깃발이 올라오는 시간

    private void OnEnable()
    {
        int currentPhaseIndex = StageManager.Instance.stageSaveData.currentPhaseIndex;
        UpdateFlags(currentPhaseIndex);
    }

    // 깃발 상태 업데이트
    public void UpdateFlags(int currentPhaseIndex)
    {
        for (int i = 0; i < currentPhaseIndex; i++)
        {
            SetFlagInstant(flags[i]);
        }

        if (currentPhaseIndex >= 0 && currentPhaseIndex < flags.Length)
        {
            AnimateFlag(currentPhaseIndex);
        }

        for (int i = currentPhaseIndex + 1; i < flags.Length; i++)
        {
            ResetFlag(flags[i]);
        }
    }

    // 깃발을 애니메이션으로 위로 올린다.
    public void AnimateFlag(int index)
    {
        if (index < 0 || index >= flags.Length)
        {
            return;
        }

        flags[index].DOAnchorPosY(targetY, duration).SetEase(Ease.OutBack);
    }

    // 깃발을 올라간 상태로 만든다. (애니메이션 없음)
    private void SetFlagInstant(RectTransform flag)
    {
        Vector2 pos = flag.anchoredPosition;
        pos.y = targetY;
        flag.anchoredPosition = pos;
    }

    // 깃발을 내려간 상태로 되돌린다.
    private void ResetFlag(RectTransform flag)
    {
        Vector2 pos = flag.anchoredPosition;
        pos.y = 0f;
        flag.anchoredPosition = pos;
    }
}
