using System;
using TMPro;
using UnityEngine;

public class MessagePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text msgText;

    private Action _onYes, _onNo;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open(string msg, Action onYes = null, Action onNo = null)
    {
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
