using NUnit.Compatibility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    static public TutorialManager Instance { get; private set; }

    [Header("Tutorial Completion Flags")]
    public bool isLobbyTutorialCompleted = false;
    public bool isGameTutorialCompleted = false;
    [Header("Tutorial Popup")]
    [SerializeField] private GameObject tutorialPopup;

    [Header("UI References")]
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialBg;
    [SerializeField] private TMP_Text nextText;
    [SerializeField] private GameObject lobbyBg;
    [SerializeField] private GameObject[] lobbyTutorialImage = new GameObject[2];

    [Header("Tutorial Settings")]
    [SerializeField, Range(0, 5)] private int lobbyTutorialSteps;
    [SerializeField, Range(0, 5)] private int gameTutorialSteps;

    [Header("Tutorial Data")]
    [SerializeField] private List<LobbyTutorial> lobbyTutorials;

    private Coroutine nextTextBlinkCoroutine;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void Start()
    {
        // 초기화
        lobbyBg.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // 기본 배경색 설정
        tutorialBg.SetActive(false); // Hide tutorial background initially
        for (int i = 0; i < lobbyTutorials.Count; i++)
        {
            lobbyTutorials[i].Show(); // Show all tutorials initially
        }
        tutorialPopup.SetActive(false); // Hide tutorial popup initially
        lobbyTutorialImage[0].SetActive(false); // Hide first tutorial image initially
        lobbyTutorialImage[1].SetActive(false); // Hide second tutorial image initially
    }
    public void OnEnable()
    {
        StartNextTextBlink();
    }
    public void OnDisable()
    {
        StopNextTextBlink();
    }
    private void StartNextTextBlink()
    {
        if (nextTextBlinkCoroutine != null)
            StopCoroutine(nextTextBlinkCoroutine);
        nextTextBlinkCoroutine = StartCoroutine(BlinkNextText());
    }
    private void StopNextTextBlink()
    {
        if (nextTextBlinkCoroutine != null)
        {
            StopCoroutine(nextTextBlinkCoroutine);
            nextTextBlinkCoroutine = null;
        }
        if (nextText != null)
        {
            var color = nextText.color;
            color.a = 1f;
            nextText.color = color;
        }
    }

    private IEnumerator BlinkNextText()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time, 1f); // 0~1 반복
            float alpha = Mathf.Lerp(0.5f, 1f, t);   // 0.5~1 선형보간
            if (nextText != null)
            {
                var color = nextText.color;
                color.a = alpha;
                nextText.color = color;
            }
            yield return null;
        }
    }
    public void StartLobbyTutorial()
    {
        if ("LobbyScene" != SceneManager.GetActiveScene().name||isLobbyTutorialCompleted)
        {
            return;
        }
        tutorialPopup.SetActive(true);
        if (isLobbyTutorialCompleted)
        {
            lobbyBg.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // Reset background color
            tutorialBg.SetActive(false);
            return;
        }
        lobbyBg.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f); // Set semi-transparent background
        tutorialBg.SetActive(true);
        lobbyTutorialSteps = 0; // Reset tutorial steps
        ShowLobbyTutorial(0);
    }
    public void ShowLobbyTutorial(int step)
    {
        if (step < 0 || step >= lobbyTutorials.Count)
        {
            Debug.LogError("Invalid tutorial step index: " + step);
            return;
        }
        LobbyTutorial currentTutorial = lobbyTutorials[step];
        tutorialText.text = currentTutorial.description;
        for (int i = 0; i < 2; i++)
        {
                if (currentTutorial.sprites[i] != null)
                {
                    lobbyTutorialImage[i].GetComponent<Image>().sprite = currentTutorial.sprites[i];
                lobbyTutorialImage[i].SetActive(true);
                }
                else
                {
                lobbyTutorialImage[i].SetActive(false);
                }
        }
        for (int i = 0; i < lobbyTutorials.Count; i++)
        {
            if (i == step)
            {
                lobbyTutorials[i].Show();
            }
            else
            {
                lobbyTutorials[i].Hide();
            }
        }
    }
    public void OnClickNextButton()
    {
        if ("LobbyScene" == SceneManager.GetActiveScene().name)
        {
            if (lobbyTutorialSteps <= 4)
            {
                lobbyTutorialSteps++;
                ShowLobbyTutorial(lobbyTutorialSteps);
            }
            else
            {
                EndLobbyTutorial();
            }
        }
    }
    public void OnClickSkipButton()
    {
        UIManager.Instance.messagePopup.Open("튜토리얼을 건너 뛰겠습니까?",
            () =>
            {
                EndLobbyTutorial();
            },
            () =>
            {
                UIManager.Instance.messagePopup.Close();
            });
    }
    public void EndLobbyTutorial()
    {
        isLobbyTutorialCompleted = true;
        tutorialBg.SetActive(false);
        lobbyBg.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // Reset background color
        if ("LobbyScene" == SceneManager.GetActiveScene().name)
        {
            for (int i = 0; i < lobbyTutorials.Count; i++)
            {
                lobbyTutorials[i].Show();
            }
        }
        tutorialPopup.SetActive(false); // Hide tutorial popup
    }

    [System.Serializable]
    class LobbyTutorial
    {
        [TextArea] public string description;
        public GameObject button;
        // 첨부 이미지 리스트
        public Sprite[] sprites = new Sprite[2];
        public void Show()
        {
            if (button != null)
            {
                if (button.GetComponent<CanvasGroup>() == null)
                {
                    button.AddComponent<CanvasGroup>();
                }
                button.GetComponent<CanvasGroup>().alpha = 1f;
            }
        }
        public void Hide()
        {
            if (button != null)
            {
                if (button.GetComponent<CanvasGroup>() == null)
                {
                    button.AddComponent<CanvasGroup>();
                }
                button.GetComponent<CanvasGroup>().alpha = 0.2f;
            }
        }
    }
}
