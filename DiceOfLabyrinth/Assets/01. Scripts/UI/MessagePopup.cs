using System;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class MessagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text msgText;
    [SerializeField] private GameObject yesButton;
    [SerializeField] private GameObject noButton;

    private Action _onYes, _onNo;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open(string msg = null, Action onYes = null, Action onNo = null)
    {
        if (_onYes == null && _onNo == null)
        {
            yesButton.SetActive(false);
            noButton.SetActive(false);
        }
        else
        {
            yesButton.SetActive(true);
            noButton.SetActive(true);
        }

        msgText.text = msg;
        _onYes = onYes;
        _onNo = onNo;
        gameObject.SetActive(true);
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
        _onYes = null;
        _onNo = null;
        gameObject.SetActive(false);
    }
}
