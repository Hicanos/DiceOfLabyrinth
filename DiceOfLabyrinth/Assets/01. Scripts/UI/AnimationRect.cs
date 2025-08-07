using UnityEngine;
#if DOTWEEN
using DG.Tweening;
#endif

namespace Helios.GUI {
    public class AnimationRect : MonoBehaviour {
        public float timeAnimScale = 0.3f;
        public float timeDelayScale = 0.05f;

        public float timeAnimRight = 0.3f;
        public float timeDelayRight = 0.05f;
        public float timeDelayRightNext = 0f;

        public float timeAnimLeft = 0.3f;
        public float timeDelayLeft = 0.05f;
        public float timeDelayLeftNext = 0f;

        public float timeAnimTop = 0.3f;
        public float timeDelayTop = 0.05f;
        public float timeDelayTopNext = 0f;

        public float timeAnimBot = 0.3f;
        public float timeDelayBot = 0.05f;
        public float timeDelayBotNext = 0f;

        [Header("팝업이 화면 밖으로 나가는 거리")]
        public float leftOutDistance = 1500f;
        public float rightOutDistance = 1500f;

        public RectTransform[] rectAnimScale;
        public RectTransform[] rectAnimRight;
        public RectTransform[] rectAnimLeft;
        public RectTransform[] rectAnimTop;
        public RectTransform[] rectAnimBot;

        private Vector2[] originPosLeft;
        private Vector2[] originPosRight;

        private bool originSaved = false;
        Vector3 scaleStart = new Vector3(0.0f, 0.0f, 0.0f);

        // 애니메이션 중 클릭 방지
        private bool isAnimating = false;
        public bool IsAnimating => isAnimating; // 외부 접근용


#if DOTWEEN
    private void SaveOriginPositions()
    {
        if (originSaved) return;
        originSaved = true;

        if (rectAnimLeft != null)
        {
            originPosLeft = new Vector2[rectAnimLeft.Length];
            for (int i = 0; i < rectAnimLeft.Length; i++)
            {
                if (rectAnimLeft[i] != null)
                    originPosLeft[i] = rectAnimLeft[i].anchoredPosition;
            }
        }

        if (rectAnimRight != null)
        {
            originPosRight = new Vector2[rectAnimRight.Length];
            for (int i = 0; i < rectAnimRight.Length; i++)
            {
                if (rectAnimRight[i] != null)
                    originPosRight[i] = rectAnimRight[i].anchoredPosition;
            }
        }
    }

    public void PlayAllIn()
    {
        SaveOriginPositions();
        AnimScaleIn();
        AnimRightIn();
        AnimLeftIn();
        AnimTopIn();
        AnimBotIn();
    }

    public void PlayAllWithLock()
    {
        isAnimating = true;
        PlayAllIn();
        Invoke(nameof(ReleaseLock), GetMaxInAnimTime());
    }

    private void ReleaseLock()
    {
        isAnimating = false;
    }

    public float GetMaxInAnimTime()
    {
        int maxCount = Mathf.Max(
            rectAnimScale?.Length ?? 0,
            rectAnimRight?.Length ?? 0,
            rectAnimLeft?.Length ?? 0,
            rectAnimTop?.Length ?? 0,
            rectAnimBot?.Length ?? 0
        );

        float maxDelay = Mathf.Max(timeDelayScale, timeDelayRight, timeDelayLeft, timeDelayTop, timeDelayBot);
        float maxAnim = Mathf.Max(timeAnimScale, timeAnimRight, timeAnimLeft, timeAnimTop, timeAnimBot);
        float maxDelayNext = Mathf.Max(timeDelayRightNext, timeDelayLeftNext, timeDelayTopNext, timeDelayBotNext);

        return maxDelay + maxDelayNext * maxCount + maxAnim;
    }

    void AnimScaleIn() {
    for(int i = 0; i < rectAnimScale.Length; i++) {
        if(rectAnimScale[i] == null) continue;
        rectAnimScale[i].localScale = scaleStart;
        rectAnimScale[i].DOScale(Vector3.one, timeAnimScale).SetEase(Ease.OutBack).SetDelay(timeDelayScale + timeDelayScale * i);
    }
    }

    public void AnimRightIn() {
        if (originPosRight == null || originPosRight.Length == 0) return;

        for(int i = 0; i < rectAnimRight.Length; i++) {
            if(rectAnimRight[i] == null) continue;
            Vector2 target = originPosRight[i];
            rectAnimRight[i].anchoredPosition = new Vector2(target.x + 1000, target.y);
            rectAnimRight[i].DOAnchorPosX(target.x, timeAnimRight).SetEase(Ease.OutCubic).SetDelay(timeDelayRight + timeDelayRightNext * i);
        }
    }

    public void AnimLeftIn() {
        if (originPosLeft == null || originPosLeft.Length == 0) return;
        for(int i = 0; i < rectAnimLeft.Length; i++) {
            if(rectAnimLeft[i] == null) continue;
            Vector2 target = originPosLeft[i];
            rectAnimLeft[i].anchoredPosition = new Vector2(target.x - 1000, target.y);
            rectAnimLeft[i].DOAnchorPosX(target.x, timeAnimLeft).SetEase(Ease.OutCubic).SetDelay(timeDelayLeft + timeDelayLeftNext * i);
        }
    }

    void AnimTopIn() {
        for(int i = 0; i < rectAnimTop.Length; i++) {
            if(rectAnimTop[i] == null) continue;
            Vector2 vector2 = rectAnimTop[i].anchoredPosition;
            rectAnimTop[i].anchoredPosition = new Vector2(vector2.x, vector2.y + 1000);
            rectAnimTop[i].DOAnchorPosY(vector2.y, timeAnimTop).SetEase(Ease.OutCubic).SetDelay(timeDelayTop + timeDelayTopNext * i);
        }
    }

    void AnimBotIn() {
        for(int i = 0; i < rectAnimBot.Length; i++) {
            if(rectAnimBot[i] == null) continue;
            Vector2 vector2 = rectAnimBot[i].anchoredPosition;
            rectAnimBot[i].anchoredPosition = new Vector2(vector2.x, vector2.y - 1000);
            rectAnimBot[i].DOAnchorPosY(vector2.y, timeAnimBot).SetEase(Ease.OutCubic).SetDelay(timeDelayBot + timeDelayBotNext * i);
        }
    }

    public void AnimLeftOut(System.Action onComplete = null)
    {
        for (int i = 0; i < rectAnimLeft.Length; i++)
        {
            if (rectAnimLeft[i] == null) continue;

            Vector2 vector2 = rectAnimLeft[i].anchoredPosition;
            rectAnimLeft[i].anchoredPosition = new Vector2(vector2.x, vector2.y);

            DG.Tweening.Tween tween = rectAnimLeft[i].DOAnchorPosX(vector2.x - leftOutDistance, timeAnimLeft)
                    .SetEase(Ease.OutCubic)
                    .SetDelay(timeDelayLeft + timeDelayLeftNext * i * 2);
            if (i == rectAnimLeft.Length - 1 && onComplete != null)
            {
                tween.OnComplete(() => onComplete());
            }
        }
    }

    public void AnimRightOut(System.Action onComplete = null)
    {
        for (int i = 0; i < rectAnimRight.Length; i++)
        {
            if (rectAnimRight[i] == null) continue;

            Vector2 vector2 = rectAnimRight[i].anchoredPosition;
            rectAnimRight[i].anchoredPosition = new Vector2(vector2.x, vector2.y);

            DG.Tweening.Tween tween = rectAnimRight[i].DOAnchorPosX(vector2.x + rightOutDistance, timeAnimRight)
                    .SetEase(Ease.OutCubic)
                    .SetDelay(timeDelayRight + timeDelayRightNext * i * 2);
            if (i == rectAnimRight.Length - 1 && onComplete != null)
            {
                    tween.OnComplete(() => onComplete());
            }
        }
    }

    public void CloseWithCallback(System.Action onComplete)
    {
        int completedCount = 0;
        int total = 0;

        // 애니메이션 대상 개수 카운트
        for (int i = 0; i < rectAnimLeft.Length; i++)
        {
            if (rectAnimLeft[i] != null) total++;
        }
        for (int i = 0; i < rectAnimRight.Length; i++)
        {
            if (rectAnimRight[i] != null) total++;
        }

        // 모든 애니메이션이 끝났는지 확인하는 콜백
        System.Action checkComplete = () =>
        {
            completedCount++;
            if (completedCount >= total)
            {
                // 닫기 애니메이션 후 원래 위치로 복귀
                for (int i = 0; i < rectAnimLeft.Length; i++)
                    if (rectAnimLeft[i] != null)
                        rectAnimLeft[i].anchoredPosition = originPosLeft[i];

                for (int i = 0; i < rectAnimRight.Length; i++)
                    if (rectAnimRight[i] != null)
                        rectAnimRight[i].anchoredPosition = originPosRight[i];

                onComplete?.Invoke(); // 전부 끝났을 때만 호출
            }
        };

        // 왼쪽 애니메이션
        for (int i = 0; i < rectAnimLeft.Length; i++)
        {
            RectTransform rect = rectAnimLeft[i];
            if (rect == null) continue;

            Vector2 original = rect.anchoredPosition;
            rect.DOAnchorPosX(original.x - leftOutDistance, timeAnimLeft)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => checkComplete());
        }

        // 오른쪽 애니메이션
        for (int i = 0; i < rectAnimRight.Length; i++)
        {
            RectTransform rect = rectAnimRight[i];
            if (rect == null) continue;

            Vector2 original = rect.anchoredPosition;
            rect.DOAnchorPosX(original.x + rightOutDistance, timeAnimRight)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => checkComplete());
        }
    }
#endif
    }
}