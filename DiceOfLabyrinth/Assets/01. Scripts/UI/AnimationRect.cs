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

        public RectTransform[] rectAnimScale;
        public RectTransform[] rectAnimRight;
        public RectTransform[] rectAnimLeft;
        public RectTransform[] rectAnimTop;
        public RectTransform[] rectAnimBot;

        private Vector2[] originPosLeft;
        private Vector2[] originPosRight;

        Vector3 scaleStart = new Vector3(0.0f, 0.0f, 0.0f);

        private void OnEnable() {
#if DOTWEEN
        AnimScaleIn();
        AnimRightIn();
        AnimLeftIn();
        AnimTopIn();
        AnimBotIn();
#else
            enabled = false;
#endif
        }

#if DOTWEEN

    private void Awake()
    {
        // 원래 위치 저장 (시작 시)
        originPosLeft = new Vector2[rectAnimLeft.Length];
        for (int i = 0; i < rectAnimLeft.Length; i++)
        {
            if (rectAnimLeft[i] != null)
                originPosLeft[i] = rectAnimLeft[i].anchoredPosition;
        }

        originPosRight = new Vector2[rectAnimRight.Length];
        for (int i = 0; i < rectAnimRight.Length; i++)
        {
            if (rectAnimRight[i] != null)
                originPosRight[i] = rectAnimRight[i].anchoredPosition;
        }
    }

    void AnimScaleIn() {
        for(int i = 0; i < rectAnimScale.Length; i++) {
            if(rectAnimScale[i] == null) continue;
            rectAnimScale[i].localScale = scaleStart;
            rectAnimScale[i].DOScale(Vector3.one, timeAnimScale).SetEase(Ease.OutBack).SetDelay(timeDelayScale + timeDelayScale * i);
        }
    }

    public void AnimRightIn() {
        for(int i = 0; i < rectAnimRight.Length; i++) {
            if(rectAnimRight[i] == null) continue;
            Vector2 target = originPosRight[i];
            rectAnimRight[i].anchoredPosition = new Vector2(target.x + 1000, target.y);
            rectAnimRight[i].DOAnchorPosX(target.x, timeAnimRight).SetEase(Ease.OutCubic).SetDelay(timeDelayRight + timeDelayRightNext * i);
        }
    }

    public void AnimLeftIn() {
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

            DG.Tweening.Tween tween = rectAnimLeft[i].DOAnchorPosX(vector2.x - 1500, timeAnimLeft)
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

            DG.Tweening.Tween tween = rectAnimRight[i].DOAnchorPosX(vector2.x + 1500, timeAnimRight)
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
                onComplete?.Invoke(); // 전부 끝났을 때만 호출
            }
        };

        // 왼쪽 애니메이션
        for (int i = 0; i < rectAnimLeft.Length; i++)
        {
            RectTransform rect = rectAnimLeft[i];
            if (rect == null) continue;

            Vector2 original = rect.anchoredPosition;
            rect.DOAnchorPosX(original.x - 1500, timeAnimLeft)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => checkComplete());
        }

        // 오른쪽 애니메이션
        for (int i = 0; i < rectAnimRight.Length; i++)
        {
            RectTransform rect = rectAnimRight[i];
            if (rect == null) continue;

            Vector2 original = rect.anchoredPosition;
            rect.DOAnchorPosX(original.x + 1500, timeAnimRight)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => checkComplete());
        }
    }
#endif
    }
}