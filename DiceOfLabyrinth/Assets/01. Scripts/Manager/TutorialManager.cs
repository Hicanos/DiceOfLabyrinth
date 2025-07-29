using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    static public TutorialManager Instance { get; private set; }
    [SerializeField] private bool isTutorialCompleted = false; // 튜토리얼이 완료되면 이 매니저를 파괴해도 됩니다.

    [Header("UI References")]
    [SerializeField, TextArea] private TMP_Text tutorialText;
    [SerializeField] private GameObject tutorialBg;
    [SerializeField] private TMP_Text nextText;

    [Header("Tutorial Settings")]
    [SerializeField, Range(0, 5)] private int lobbyTutorialSteps;
    [SerializeField, Range(0, 5)] private int gameTutorialSteps;

    [SerializeField] private RectTransform messageContainer;

    [SerializeField] private List<LobbyTutorial> lobbyTutorials;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}

[System.Serializable]
public class LobbyTutorial
{
    public string description;
    public GameObject button;
    public void Show()
    {
        if (button != null)
        {
            if(button.GetComponent<CanvasGroup>() == null)
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
            if(button.GetComponent<CanvasGroup>() == null)
            {
                button.AddComponent<CanvasGroup>();
            }
            button.GetComponent<CanvasGroup>().alpha = 0.5f;
        }
    }
}
