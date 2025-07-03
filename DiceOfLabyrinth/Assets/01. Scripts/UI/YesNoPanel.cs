using System;
using TMPro;
using UnityEngine;

public class YesNoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text msgText;

    private Action _onConfirm;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Open(string msg, Action onConfirm)
    {
        msgText.text = msg;
        _onConfirm = onConfirm;
        gameObject.SetActive(true);
    }

    public void OnYes()
    {
        _onConfirm?.Invoke();
        Close();
    }

    // public void OnNo()

    public void Close()
    {
        gameObject.SetActive(false);
        _onConfirm = null;
    }
}
