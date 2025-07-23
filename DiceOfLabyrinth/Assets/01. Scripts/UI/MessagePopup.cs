using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class MessagePopup : MonoBehaviour
{
    public static MessagePopup Instance { get; private set; }
    [SerializeField] private TMP_Text msgText;
    [SerializeField] private GameObject bottomUI;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;
    [SerializeField] private CanvasGroup cgFade;

    [SerializeField] float fadeIn = .25f;
    [SerializeField] float fadeOut = .20f;
    [SerializeField] float popScale = 1.1f;

    private Action _onYes, _onNo;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (!cgFade)
        {
            cgFade = GetComponent<CanvasGroup>();
        }
    }

    public void Open(string msg = null, Action onYes = null, Action onNo = null)
    {
        switch (onYes, onNo)
        {
            case (null, null):
                bottomUI.SetActive(false);
                break;
            case (null, _):
                bottomUI.SetActive(true);
                yesButton.SetActive(false);
                noButton.SetActive(true);
                break;
            case (_, null):
                bottomUI.SetActive(true);
                yesButton.SetActive(true);
                noButton.SetActive(false);
                break;
            default:
                bottomUI.SetActive(true);
                yesButton.SetActive(true);
                noButton.SetActive(true);
                break;
        }

        msgText.text = msg;
        _onYes = onYes;
        _onNo = onNo;
        gameObject.SetActive(true);

#if DOTWEEN
        cgFade.alpha = 0;
        cgFade.DOFade(1, fadeIn);

        transform.localScale = Vector3.one * popScale;
        transform.DOScale(1, fadeIn).SetEase(Ease.OutBack);
#endif
    }

    public void OnClickYes()
    {
        _onYes?.Invoke();
        Close();
    }

    public void OnClickNo()
    {
        _onNo?.Invoke(); // 필요하면 사용
        Close();
    }

    public void Close()
    {
#if DOTWEEN
        cgFade.DOFade(0, fadeOut).OnComplete(() => gameObject.SetActive(false));
#else
        gameObject.SetActive(false);
#endif
        _onYes = _onNo = null;
    }
}
