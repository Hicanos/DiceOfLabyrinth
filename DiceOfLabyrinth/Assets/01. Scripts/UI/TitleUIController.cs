using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup blackOverlay;
    [SerializeField] private GameObject companyLogoPanel;
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject publicUIController;
    [SerializeField] private TextMeshProUGUI tapToStartText;

    [Header("Settings")]
    [SerializeField] private float fadeTime = 1.0f; // 화면이 다 어두워지는데 걸리는 시간
    [SerializeField] private float waitTime = 1.0f; // 얼마나 기다렸다 어두워지기 시작할지
    [SerializeField] private float blinkTime = 0.5f; // 텍스트 깜빡임 속도

    private bool canStart = false;

    private void Start()
    {
        // 초기 상태
        publicUIController.SetActive(false);
        companyLogoPanel.SetActive(true);
        titlePanel.SetActive(false);
        blackOverlay.alpha = 0;
        blackOverlay.gameObject.SetActive(true);

        // 도트윈 시퀸스
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(waitTime);
        seq.Append(blackOverlay.DOFade(1, fadeTime));
        seq.AppendCallback(() =>
        {
            // 회사 로고 숨기기, 어두운 패널 비활성화
            companyLogoPanel.SetActive(false);
            blackOverlay.gameObject.SetActive(false);

            // 타이틀 UI 활성화
            titlePanel.SetActive(true);
            publicUIController.SetActive(true);

            // 텍스트 깜빡임 시작
            if (tapToStartText != null)
            {
                tapToStartText.alpha = 1f;
                tapToStartText.DOFade(0f, blinkTime).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            }

            canStart = true;
        });
        SoundManager.Instance.PlayBGM(SoundManager.SoundType.BGM_Title);
    }

    private void Update()
    {
        if (!canStart) return;

        // 마우스 클릭(PC), 터치 입력(모바일) 테스트를 위해 PC도 해놓음 나중에 지울것
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchPressed = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        if (mousePressed || touchPressed)
        {
            SceneManagerEx.Instance.LoadScene("LobbyScene");
        }
    }
}
